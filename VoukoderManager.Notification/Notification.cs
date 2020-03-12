namespace VoukoderManager.Notify
{
    public class Notification : INotification
    {
        public string Message { get; }

        public string Title { get; }

        public Notification(string title, string message)
        {
            Message = message;
            Title = title;
        }
    }
}