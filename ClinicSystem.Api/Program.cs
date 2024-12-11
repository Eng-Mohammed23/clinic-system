using ClinicSystem.Api;
using ClinicSystem.Api.Hubs;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Stripe;
using WhatsAppCloudApi.Extensions;
using WhatsAppCloudApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDependencies(builder.Configuration);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddWhatsAppApiClient(builder.Configuration);

// езого Stripe API Key
StripeConfiguration.ApiKey = "sk_test_51QLW1kFjBI0YVQW51rwB0WVkKgWj0iStWI01AwYp49dZxPmofKfVmFiROiW33xpX3Lct025MPrS4DIesfTmFLRrD00OWNJZsDH";

builder.Services.AddSignalR();

var app = builder.Build();
//var hangfireTasks = new HangfireTasks(dbContext, webHostEnvironment, whatsAppClient,
//emailBodyBuilder, emailSender);

//RecurringJob.AddOrUpdate(() => hangfireTasks.PrepareExpirationAlert(), "0 14 * * *");
//RecurringJob.AddOrUpdate(() => hangfireTasks.RentalsExpirationAlert(), "0 14 * * *");

///////////
//var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
//using var scope = scopeFactory.CreateScope();
//var notificationService = scope.ServiceProvider.GetRequiredService<IBookService>();

//RecurringJob.AddOrUpdate("SendNotification", () => notificationService.SendNotification(), Cron.Daily);



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");

app.UseHttpsRedirection();


app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    Authorization =
    [
        new HangfireCustomBasicAuthenticationFilter
        {
            User = app.Configuration.GetValue<string>("HangfireSettings:Username"),
            Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
        }
    ],
    DashboardTitle = "Clinic Dashboard",
    //IsReadOnlyFunc = (DashboardContext  context) => true
});
//app.UseHangfireDashboard("/jobs");

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chatHub");

app.Run();
