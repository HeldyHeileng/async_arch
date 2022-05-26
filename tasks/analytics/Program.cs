using analytics.Controllers;
using analytics.Kafka;
using analytics.Context;
using analytics;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(mc => mc.AddProfile(new MappingProfile()));
builder.Services.AddControllers();
builder.Services.AddScoped<AccountController>();
builder.Services.AddScoped<TransactionController>();
builder.Services.AddHostedService<EventConsumer>();
builder.Services.AddDbContext<ApplicationContext>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapDefaultControllerRoute();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Run();
