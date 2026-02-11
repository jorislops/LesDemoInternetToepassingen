// See https://aka.ms/new-console-template for more information

using EFLesDemo.Entities;
using Microsoft.EntityFrameworkCore;

{
    var db = new ScrumboardDbContext();

    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();

    var scrumboard = DBSeeder.Seed();
    db.Scrumboards.AddRange(scrumboard);
    db.SaveChanges();
}

{
    var db2 = new ScrumboardDbContext();
    var scrumboards = db2.Scrumboards
        .Include(x => x.Columns)
        .ThenInclude(x => x.Cards)
        .ToList();

    foreach (var scrumboard in scrumboards)
    {
        Console.WriteLine($"Board: {scrumboard.Name}");
        foreach (var column in scrumboard.Columns)
        {
            Console.WriteLine($"    Col:    {column.Name}");
            foreach (var card in column.Cards)
            {
                Console.WriteLine($"        Card:        {card.Name}");
            }
        }
    }
}