using Api.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();
app.UseApiConfiguration();

app.Run();
