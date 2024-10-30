using FlutterWave;
using Ordering.API;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Data.Extensions;
using Paystack;
using sendsprint.ecommerce.Common.PaymentGateway;
using sendsprint.ecommerce.Common.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddSwaggerService(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("Gateway", client =>
{
    client.Timeout = TimeSpan.FromSeconds(90);
});
builder.Services.AddSingleton<IGatewayManager, GatewayManager>();
builder.Services.AddFlutterWave(builder.Configuration);
builder.Services.AddPaystack(builder.Configuration);
builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.UseSwaggerService(builder.Configuration);
app.UseApiServices();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}

app.Run();
