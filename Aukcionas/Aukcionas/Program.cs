using Aukcionas.Data;
using Aukcionas.Services;
using Aukcionas.Utils.ConfigOptions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<GCSConfigOptions>(builder.Configuration);// loading config file to start app.
builder.Services.AddSingleton<ICloudStorageService, CloudStorageService>();
builder.Services.AddCors(options => options.AddPolicy(name: "AuctionPolicy",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
            policy.WithOrigins("http://localhost:7187").AllowAnyMethod().AllowAnyHeader();
        }
    ));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AuctionPolicy");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
