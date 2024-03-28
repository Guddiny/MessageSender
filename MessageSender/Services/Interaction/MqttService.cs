using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;


namespace MessageSender.Services.Interaction;

public class MqttService : IMessagingService
{
    private string _deviceId = string.Empty;
    private string _deviceKey = string.Empty;
    private string _host = string.Empty;
    private IMqttClient? _mqttClient;
    private MqttFactory? _mqttFactory;
    private MqttClientOptions? _mqttClientOptions;

    public async Task Connect(string deviceId, string deviceKey, string host)
    {
        if (_mqttClient is { IsConnected: true })
        {
            return;
        }

        _deviceId = deviceId;
        _deviceKey = deviceKey;
        _host = host;

        _mqttClient?.Dispose();

        _mqttFactory = new MqttFactory();
        _mqttClient = _mqttFactory.CreateMqttClient();

        _mqttClient.ConnectedAsync += (arg) =>
        {
            ConnectedToMessageSource?.Invoke(this, "Connected");
            return Task.CompletedTask;
        };

        _mqttClient.DisconnectedAsync += (arg) =>
        {
            DisconnectedFromMessageSource?.Invoke(this, "Disconnected");
            return Task.CompletedTask;
        };

        _mqttClient.ApplicationMessageReceivedAsync += (arg) =>
        {
            var payload = arg.ApplicationMessage.ConvertPayloadToString();
            MessageReceived?.Invoke(this,
                new DeviceMessage
                {
                    Payload = payload, DeviceId = string.Empty, Timestamp = DateTimeOffset.Now,
                    Properties = GetUserProperties(arg)
                });
            return Task.CompletedTask;
        };

        var sasToken = CreateSasToken($"{_host}.azure-devices.net/devices/{_deviceId}", _deviceKey);

        _mqttClientOptions = new MqttClientOptionsBuilder()
            .WithClientId(_deviceId)
            .WithTcpServer($"{_host}.azure-devices.net", 8883)
            .WithCredentials($"{_host}.azure-devices.net/{_deviceId}/?api-version=2021-04-12", sasToken)
            .WithTlsOptions(opt =>
            {
                opt.UseTls();
                opt.WithIgnoreCertificateChainErrors();
                opt.WithIgnoreCertificateRevocationErrors();
                opt.WithAllowUntrustedCertificates();
            })
            .WithCleanSession(false)
            .Build();

        await _mqttClient!.ConnectAsync(_mqttClientOptions, CancellationToken.None);

        await SubscribeToTopics();
    }

    public async Task Disconnect()
    {
        if (_mqttClient is { IsConnected: true })
        {
            await _mqttClient.UnsubscribeAsync(new MqttClientUnsubscribeOptions
            {
                TopicFilters =
                [
                    $"devices/{_deviceId}/messages/devicebound/#"
                ]
            })!;
            await _mqttClient.DisconnectAsync()!;
        }

        _mqttClient?.Dispose();
    }

    public bool IsConnected => _mqttClient?.IsConnected ?? false;

    public event Func<DeviceMessage, Task>? SomeMessageReceived;

    public EventHandler<DeviceMessage>? MessageReceived { get; set; }

    public EventHandler<string>? ConnectedToMessageSource { get; set; }

    public EventHandler<string>? DisconnectedFromMessageSource { get; set; }

    public async Task SendMessage(DeviceMessage deviceMessage)
    {
        var sendMessageTopic = $"devices/{deviceMessage.DeviceId}/messages/events/";

        MqttApplicationMessage message = new MqttApplicationMessageBuilder()
            .WithTopic(GetTopicWithUserProps(deviceMessage, sendMessageTopic))
            .WithPayload(deviceMessage.Payload)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        await _mqttClient!.PublishAsync(message);
    }

    public void Dispose()
    {
        Disconnect().GetAwaiter().GetResult();
    }

    private async Task SubscribeToTopics()
    {
        var mqttSubscribeOptions = _mqttFactory!.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(
                f => { f.WithTopic($"devices/{_deviceId}/messages/devicebound/#"); })
            .Build();

        await _mqttClient!.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
    }

    private static string CreateSasToken(string resourceUri, string key)
    {
        var sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
        var weekInSeconds = TimeSpan.FromDays(7).TotalSeconds;
        var expiry = Convert.ToString((int)sinceEpoch.TotalSeconds + weekInSeconds, CultureInfo.InvariantCulture);

        var stringToSign = $"{HttpUtility.UrlEncode(resourceUri)}\n{expiry}";
        var hmac = new HMACSHA256(Convert.FromBase64String(key));
        var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

        var encodedResourceUri = HttpUtility.UrlEncode(resourceUri);
        var encodedSignature = HttpUtility.UrlEncode(signature);

        return $"SharedAccessSignature sr={encodedResourceUri}&sig={encodedSignature}&se={expiry}";
    }

    private Dictionary<string, string> GetUserProperties(MqttApplicationMessageReceivedEventArgs args)
    {
        string topicData = args.ApplicationMessage.Topic
            .Split("deviceBound&")
            [1]
            .Replace("%24.ct=text%2Fplain%3B%20charset%3DUTF-8&%24.ce=utf-8", "");

        string[] propertyList = topicData
            .Split('&', StringSplitOptions.RemoveEmptyEntries);

        return propertyList
            .Select(i => i.Split("="))
            .ToDictionary(keyData => keyData[0], keyData => keyData[1]);
    }

    private string GetTopicWithUserProps(DeviceMessage deviceMessage, string topic)
    {
        StringBuilder sb = new();
        sb.Append(topic);
        sb.Append("$.ct=application%2Fjson&$.ce=utf-8");

        foreach (var i in deviceMessage.Properties)
        {
            sb.Append($"&{i.Key}={i.Value}");
        }

        var result = sb.ToString();
        return result;
    }
}