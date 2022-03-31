using Hangfire;
using Microsoft.Extensions.Options;
using ScheduleJobs.Interfaces;
using ScheduleJobs.Services;
using ScheduleJobs.Settings;

namespace ScheduleJobs
{
    public static class Startup
    {
        public static WebApplication InitApp(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);

            var app = builder.Build();
            Configure(app);
            return app;
        }
        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));
            builder.Services.AddScoped<IMailSettings>(sp => sp.GetRequiredService<IOptions<MailSettings>>().Value);
            builder.Services.AddScoped<IMailService, MailService>();
            builder.Services.AddHangfire(options =>
                options.UseSqlServerStorage(builder.Configuration.GetConnectionString("Default")));
            builder.Services.AddHangfireServer();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        private static void Configure(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.UseHangfireDashboard("/hf-dashboard");
        }
    }
}
