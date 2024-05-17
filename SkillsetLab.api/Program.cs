using Application.Features.Features.Account;
using Application.Infrastructure.AutoMapper;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillsetLab.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Login).Assembly);
});

services.Configure<AdminUser>(builder.Configuration.GetSection("AdminUser"));
services.AddDbContext<MyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));
services.AddTransient<IMyDbContext, MyDbContext>();

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