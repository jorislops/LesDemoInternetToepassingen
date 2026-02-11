using Bogus;

namespace EFLesDemo.Entities;

public class DBSeeder
{
    static DBSeeder()
    {
        Randomizer.Seed = new Random(1024);
    }
    
    public static List<Scrumboard> Seed(int numOfScrumboards = 5) =>
        new Faker<Scrumboard>()
            .RuleFor(x => x.Name, f => f.Lorem.Sentence(f.Random.Int(2, 5)))
            .RuleFor(x => x.Columns, f => SeedScrumboardColumns(f.Random.Int(3, 5)))
            .Generate(numOfScrumboards);

    private static List<ScrumboardColumn> SeedScrumboardColumns(int numOfScrumboards) =>
        new Faker<ScrumboardColumn>()
            .RuleFor(x => x.Name, f => f.Lorem.Sentence(f.Random.Int(2, 5)))
            .RuleFor(x => x.Cards, f => SeedCards(f.Random.Int(3, 8)))
            .RuleFor(x => x.Order, f => f.IndexFaker)
            .Generate(numOfScrumboards);

    private static List<Card> SeedCards(int numOfCards) =>
        new Faker<Card>()
            .RuleFor(x => x.Name, f => f.Lorem.Sentence(f.Random.Int(2, 5)))
            .RuleFor(x => x.Order, f => f.IndexFaker)
            .Generate(numOfCards);
}