namespace MessageSender.Utils.ActionWrapper
{
    public class DialogOptions
    {
        public string Title { get; private set; } = string.Empty;

        public required string Message { get; init; } = string.Empty;
    }
}
