using Blip.Dealer.Desk.Manager.Facades.Extensions;
using Blip.Dealer.Desk.Manager.Models.AppSettings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDependecyInjection();
builder.Services.AddRestEaseClients();

builder.Services.Configure<GoogleSheetsSettings>(builder.Configuration.GetSection(nameof(GoogleSheetsSettings)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
