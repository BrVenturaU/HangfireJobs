namespace ScheduleJobs.Interfaces
{
    public interface IMailService
    {
        Task SendMail(string email, string subject, string body);
    }
}
