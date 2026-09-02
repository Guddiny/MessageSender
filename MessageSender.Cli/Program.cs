using System.CommandLine;
using System.Text.Json;
using MessageSender.Cli.Models;
using MessageSender.Services.Interaction;

namespace MessageSender.Cli;

class Program
{
    static int Main(string[] args)
    {
        var inlineCommand = BuildInlineCommand();
        var fromConfigCommand = BuildFromConfigCommand();

        RootCommand rootCommand = new("Send message to IotHub as a device");
        rootCommand.Subcommands.Add(inlineCommand);
        rootCommand.Subcommands.Add(fromConfigCommand);

        ParseResult parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }

    private static Command BuildFromConfigCommand()
    {
        Option<string> messagingConfigOption = new("-file")
        {
            Description = "Path to config file",
            Required = true,
        };

        Command fromConfigCommand = new("--cfg")
        {
            messagingConfigOption
        };

        fromConfigCommand.SetAction(async (parseResult, ct) =>
        {
            var cfgFilePath = parseResult.GetValue(messagingConfigOption);
            if (!File.Exists(cfgFilePath))
            {
                throw new FileNotFoundException($"File {cfgFilePath} not found");
            }

            await using var fileStream = File.OpenRead(cfgFilePath);
            var config = await JsonSerializer.DeserializeAsync<MessagingConfig>(fileStream, cancellationToken: ct);
            
            // TODO 
            throw new NotImplementedException($"{nameof(fromConfigCommand)} is not implemented)");
        });
        
        return fromConfigCommand;
    }

    private static Command BuildInlineCommand()
    {
        Option<string> deviceIdOption = new("-id")
        {
            Description = "Device ID",
            Required = true,
        };

        Option<string> hostOption = new("-host")
        {
            Description = "IotHub host",
            Required = true,
        };

        Option<string> deviceKeyOption = new("-key")
        {
            Description = "Device Key",
            Required = true,
        };
        
        Option<string> deviceTypeOption = new("-deviceType", "-dt")
        {
            Description = "Device Type",
            Required = false,
        };

        Option<string> payloadOption = new("-data")
        {
            Description = "Payload",
            Required = true,
        };

        Option<string> headersOption = new("-headers")
        {
            Description = "Headers",
            Required = true,
        };
        
        Option<TimeSpan?> waitDeviceResponseOption = new("-wait")
        {
            Description = "Wait device response: [TimeSpan]",
            Required = false,
        };

        Command inlineCommand = new("--send")
        {
            deviceIdOption,
            hostOption,
            deviceKeyOption,
            deviceTypeOption,
            payloadOption,
            headersOption,
            waitDeviceResponseOption
        };

        inlineCommand.SetAction(async (parseResult, ct) =>
        {
            var deviceId = parseResult.GetValue(deviceIdOption)!;
            var host = parseResult.GetValue(hostOption)!;
            var deviceKey = parseResult.GetValue(deviceKeyOption)!;
            var deviceType = parseResult.GetValue(deviceTypeOption);
            var payload = parseResult.GetValue(payloadOption)!;
            var headers = parseResult.GetValue(headersOption)!;
            var waitDeviceResponse = parseResult.GetValue(waitDeviceResponseOption);

            var parsedHeaders = JsonSerializer.Deserialize<Dictionary<string, string>>(headers)!;

            var mqtt = new MqttService();
            await mqtt.Connect(deviceId, deviceKey, host);
            mqtt.MessageReceived += (sender, message) =>
            {
                Console.WriteLine($"<= {deviceId} {deviceType}");
            };

            var message = new DeviceMessage
            {
                DeviceId = deviceId,
                Direction = MessageDirection.Out,
                Payload = payload,
                Properties = parsedHeaders
            };

            await mqtt.SendMessage(message);
            
            Console.WriteLine($"=> {deviceId} {deviceType}");

            if (waitDeviceResponse != null)
            {
                Console.WriteLine($"Waiting for device response....");
                await Task.Delay(TimeSpan.FromSeconds(waitDeviceResponse.Value.TotalSeconds), ct);
            }
        });
        
        return inlineCommand;
    }
}