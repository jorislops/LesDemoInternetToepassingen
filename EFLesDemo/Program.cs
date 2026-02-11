// See https://aka.ms/new-console-template for more information

using EFLesDemo.Entities;
using Microsoft.EntityFrameworkCore;

{
    var db = new ScrumboardDbContext();

    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();

    db.Scrumboards.Add(new Scrumboard()
    {
        Name = "My Scrumboard",
        Columns = new List<ScrumboardColumn>()
        {
            new ScrumboardColumn
            {
                Name = "Col 1",
                Cards = new List<Card>()
                {
                    new Card
                    {
                        Name = "Card 1",
                    },
                    new Card
                    {
                        Name = "Card 2",
                    }
                },
            }
        }
    });

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
        Console.WriteLine(scrumboard.Name);
        foreach (var column in scrumboard.Columns)
        {
            Console.WriteLine($"  {column.Name}");
            foreach (var card in column.Cards)
            {
                Console.WriteLine($"    {card.Name}");
            }
        }
    }
}