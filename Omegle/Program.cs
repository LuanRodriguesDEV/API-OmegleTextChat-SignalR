using Omegle.Hubs;
using Omegle.Programs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCorsPolicy();
builder.Services.AddSignalR().AddHubOptions<TextHUB>(config => config.EnableDetailedErrors = true);
builder.Services.AddSignalR(conf =>
{
    conf.MaximumReceiveMessageSize = null;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseAuthorization();
app.UseCors("CorsPolicy");

app.MapControllers();
app.MapHub<TextHUB>("/text");


app.Run();
