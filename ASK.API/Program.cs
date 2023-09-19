using ASK.API.Extensions;
using ASK.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


//Configure serilog
builder.ConfigureSerilog();

builder.WebHost.UseUrls("http://0.0.0.0:", "https://0.0.0.0:5001");

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetService<AskDbContext>();

    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseSwaggerDocumentation();

app.MapControllers();

app.UseApplicationServices(builder.Environment);

app.Run();
