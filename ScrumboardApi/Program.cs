using EFLesDemo.Entities;
using EFLesDemo.Repositories;
using EFLesDemo.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



builder.Services.AddDbContext<ScrumboardDbContext>(options =>
    //TODO get connection string from config
    options.UseNpgsql("Server=127.0.0.1;Port=5432;Database=scrumboardlesdemo;User Id=postgres;Password=postgres;")
);

builder.Services.AddScoped<IScrumboardRepository, ScrumboardRepository>();
builder.Services.AddScoped<IScrumCardRepository, ScrumCardRepository>();

builder.Services.AddScoped<IScrumboardService, ScrumboardService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using var  scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ScrumboardDbContext>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
    var scrumboards = DBSeeder.Seed();
    db.Scrumboards.AddRange(scrumboards);
    db.SaveChanges();
    
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "api/{controller}/{action}");

app.Run();