using FluentValidation;
using MeterReadings.Core.Models;
using MeterReadings.Core.Repositories;
using MeterReadings.Core.Services;
using MeterReadings.Core.Validators;
using MeterReadings.Data;
using MeterReadings.Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddControllers();
builder.Services.AddTransient<MeterReadingProcessor>();
builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddTransient<IMeterReadingRepository, MeterReadingRepository>();
builder.Services.AddTransient<IValidator<MeterReadingEntry>, MeterReadingEntryValidator>();
builder.Services.AddDbContext<MeterReadingsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MeterReadingsDatabase")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
using (var dbContext = scope.ServiceProvider.GetRequiredService<MeterReadingsDbContext>())
    dbContext.Database.EnsureCreated();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapControllers();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.Run();
