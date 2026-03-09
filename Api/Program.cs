using FluentValidation;
using Api.Validators;
using Application.Queries;
using Application.Requests;
using Domain.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using Domain.Enums;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<GibUserDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IGibUserRepository, GibUserRepository>();

builder.Services.AddScoped<GetGibUserHandler>();

builder.Services.AddProblemDetails();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddValidatorsFromAssemblyContaining<GetGibUserRequestValidator>();

builder.Services.AddHealthChecks().AddDbContextCheck<GibUserDbContext>("SQL Server");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
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

app.UseExceptionHandler();

app.UseStatusCodePages();

app.MapHealthChecks("/health/live");

app.MapHealthChecks("/health/ready");

app.MapGet("/gib-users", async (
    [AsParameters] GetGibUserRequest request,
    IValidator<GetGibUserRequest> validator,
    GetGibUserHandler handler) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(
            validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                ));
    }

    var query = new GetGibUserQuery(
        request.Identifier,
        Enum.Parse<DocType>(request.DocumentType, true),
        Enum.Parse<Unit>(request.Unit, true));

    var result = await handler.HandleAsync(query);

    return result is null
        ? Results.NotFound()
        : Results.Ok(result);
});

app.Run();
