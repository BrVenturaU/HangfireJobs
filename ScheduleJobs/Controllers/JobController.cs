using Hangfire;
using Microsoft.AspNetCore.Mvc;
using ScheduleJobs.Interfaces;
using ScheduleJobs.Requests;

namespace ScheduleJobs.Controllers
{
    [ApiController]
    [Route("api/jobs")]
    public class JobController : ControllerBase
    {
        public JobController()
        {
        }

        [HttpPost("fire-forget")]
        public async Task<ActionResult> Welcome([FromBody] WelcomeRequest welcomeRequest)
        {
            var jobId = BackgroundJob.Enqueue<IMailService>(
                em => em.SendMail(welcomeRequest.Email, welcomeRequest.Subject, welcomeRequest.Body)
            );

            return Ok($"Job created with id: {jobId}");
        }

        [HttpPost("delayed")]
        public async Task<ActionResult> WelcomeDelayed([FromBody] WelcomeRequest welcomeRequest)
        {
            var futureTime = DateTime.Now.AddSeconds(20);
            var actualTime = DateTime.Now;
            var jobId = BackgroundJob.Schedule<IMailService>(
                em => em.SendMail(welcomeRequest.Email, welcomeRequest.Subject, welcomeRequest.Body),
                futureTime
            );

            return Ok(new {
                futureTime,
                actualTime,
                message = $"Job created with id: {jobId}"
            });
        }

        [HttpPost("recurrent")]
        public async Task<ActionResult> WelcomeRecurring([FromBody] WelcomeRequest welcomeRequest)
        {
            RecurringJob.AddOrUpdate<IMailService>(
                em => em.SendMail(welcomeRequest.Email, welcomeRequest.Subject, welcomeRequest.Body),
                Cron.Minutely()
            );

            return Ok($"Executes the job every minute: {Cron.Minutely()}");
        }

        [HttpPost("continuation")]
        public async Task<ActionResult> WelcomeContinuation([FromBody] WelcomeRequest welcomeRequest)
        {
            var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("Notifying admin..."));
            var childJobId = BackgroundJob.ContinueJobWith<IMailService>(jobId,
                em => em.SendMail(welcomeRequest.Email, welcomeRequest.Subject, welcomeRequest.Body));

            return Ok($"Executes the job: {jobId} first. Then executes the child job id: {childJobId}");
        }
    }
}
