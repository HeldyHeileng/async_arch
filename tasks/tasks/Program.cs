
using Confluent.Kafka;
using tasks.Context;
using tasks.Controllers;
using tasks.Kafka;

var builder = WebApplication.CreateBuilder(args); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddTransient<AccountController>();
builder.Services.AddDbContext<ApplicationContext>();
builder.Services.AddHostedService<EventConsumer>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapDefaultControllerRoute();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Run();
