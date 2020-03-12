namespace VoukoderManager.Notify
{
    public interface INotification
    {
        string Message { get; }
        string Title { get; }
    }
}