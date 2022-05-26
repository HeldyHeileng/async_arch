using accounting.Controllers;
using accounting.Kafka;
using accounting.Context;
using accounting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(mc => mc.AddProfile(new MappingProfile()));
builder.Services.AddControllers();
builder.Services.AddScoped<AccountController>();
builder.Services.AddScoped<TaskEventHandlers>();
builder.Services.AddScoped<EventProducer>();
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
