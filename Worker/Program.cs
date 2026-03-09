using Application.DTOs;
using Application.Queries;
using Domain.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<GibUserDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<GibUserOptions>(
    builder.Configuration.GetSection("AppSettings"));

builder.Services.AddScoped<GibUserService>();

builder.Services.AddScoped<IGibUserRepository, GibUserRepository>();

builder.Services.AddQuartz(q =>
{
    var job1 = new JobKey("gibuser-updater");
    q.AddJob<GibUserUpdaterJob>(opts => opts.WithIdentity(job1));
    q.AddTrigger(opts => opts
    .ForJob(job1)
    .WithIdentity("gibuser-updater-trigger")
    .WithCronSchedule("* * */2 * * ?"));
});

builder.Services.AddQuartzHostedService(q =>
{
    q.WaitForJobsToComplete = true;
});

builder.Logging.ClearProviders();

builder.Logging.AddConsole();

var app = builder.Build();

if (args.Contains("migrate"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<GibUserDbContext>();
    Console.WriteLine("Running database migrations...");
    await db.Database.MigrateAsync();
    Console.WriteLine("Migration completed.");
    return;
}
app.Run();
