using EFLesDemo;
using EFLesDemo.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Test.EFLesDemo;

public class Tests
{
    private PostgreSqlContainer _container;
    private DbContextOptionsBuilder<ScrumboardDbContext> _contextOptionsBuilder;

    
    [SetUp]
    public async Task Setup()
    {
        var postgresContainerBuilder = new PostgreSqlBuilder("postgres:16");
        _container = postgresContainerBuilder.Build();
        await _container.StartAsync();
        
        _contextOptionsBuilder = new DbContextOptionsBuilder<ScrumboardDbContext>();
        _contextOptionsBuilder.UseNpgsql(_container.GetConnectionString());
        _contextOptionsBuilder.EnableSensitiveDataLogging();
        _contextOptionsBuilder.EnableDetailedErrors();
        // _contextOptionsBuilder.LogTo(async void (line) =>
        // {
        //     await TestContext.Progress.WriteLineAsync(line);
        //     await TestContext.Progress.FlushAsync();            
        // }, new[]
        // {
        //     Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuted
        // });
        // _contextOptionsBuilder.LogTo(
        //     TestContext.Out.WriteLine,
        //     new[] { DbLoggerCategory.Database.Command.Name, DbLoggerCategory.Query.Name },
        //     LogLevel.Information,
        //     DbContextLoggerOptions.SingleLine);

        CreateDatabaseAndSeed(new ScrumboardDbContext(_contextOptionsBuilder.Options));
    }

    [TearDown]
    public async Task TearDown()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }

    private static void CreateDatabaseAndSeed(ScrumboardDbContext db)
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        
        //seed database
        var scrumboards = DBSeeder.Seed();
        db.Scrumboards.AddRange(scrumboards);
        db.SaveChanges();
    }

    [Test]
    public void CardsAreOrderedByPosition()
    {
        //Arrange
        var db = new ScrumboardDbContext(_contextOptionsBuilder.Options);
        var scrumboardRepository = new ScrumboardRepository(db);
        
        //Act
        var scrumboards = scrumboardRepository.LoadScrumboardWithColumnAndCards();
        
        //Assert
        scrumboards.Should().NotBeNullOrEmpty();
        scrumboards.Should().HaveCount(5);

        scrumboards.Should().AllSatisfy(scrumboard =>
        {
            scrumboard.Columns.Should().BeInAscendingOrder(column => column.Order);
            scrumboard.Columns.Should().AllSatisfy(column =>
                column.Cards.Should().BeInAscendingOrder(card => card.Order)
            );
        });
    }


    [Test]
    public void WhenCardIsAddedAtTheEndOfAColumnThenCardsAreOrderedByPosition()
    {
        var scrumboardId = 1;
        
        //Arrange
        var db = new ScrumboardDbContext(_contextOptionsBuilder.Options);
        var scrumboardRepository = new ScrumboardRepository(db);
        var scrumboardService = new ScrumboardService(db);

        var scrumboard = scrumboardRepository.LoadScrumboardWithColumnAndCardsById(scrumboardId);
        scrumboard.Should().NotBeNull();
        var columnId = scrumboard.Columns.First().Id;
        
        //Act
        scrumboardService.AddCardEndOfColumn(columnId, new Card
        {
            Name = "test",
        });
        
        //Assert
        var scrumboardToCheck = scrumboardRepository.LoadScrumboardWithColumnAndCardsById(1);

        scrumboardToCheck.Should().NotBeNull();

        scrumboard.Columns.Should().BeInAscendingOrder(x => x.Order);
        scrumboard.Columns.Should().AllSatisfy(column =>
        {
            column.Cards.Should().BeInAscendingOrder(card => card.Order);
        }); 
    }
    
    [Test]
    public void WhenCardIsAddedAtAPositionThenCardsAreOrderedByPosition()
    {
        var scrumboardId = 1;
        var position = 5;
        
        //Arrange
        var db = new ScrumboardDbContext(_contextOptionsBuilder.Options);
        var scrumboardRepository = new ScrumboardRepository(db);
        var scrumboardService = new ScrumboardService(db);

        var scrumboard = scrumboardRepository.LoadScrumboardWithColumnAndCardsById(scrumboardId);
        scrumboard.Should().NotBeNull();
        var columnId = scrumboard.Columns.First().Id;
        
        //Act
        scrumboardService.AddCardToOrder(columnId, position,  new Card
        {
            Name = "test",
        });
        
        //Assert
        scrumboard = scrumboardRepository.LoadScrumboardWithColumnAndCardsById(1);

        scrumboard.Should().NotBeNull();

        scrumboard.Columns.Should().BeInAscendingOrder(x => x.Order);
        scrumboard.Columns.Should().AllSatisfy(column =>
        {
            column.Cards.Should().BeInAscendingOrder(card => card.Order);
        }); 
    }
    
    [Test]
    public void WhenCardIsAddedAtAPositionThenCardsAreOrderedByPosition2()
    {
        var scrumboardId = 1;
        var position = 5;
        
        //Arrange
        var db = new ScrumboardDbContext(_contextOptionsBuilder.Options);
        var scrumboardRepository = new ScrumboardRepository(db);
        var scrumboardService = new ScrumboardService(db);

        var scrumboard = scrumboardRepository.LoadScrumboardWithColumnAndCardsById(scrumboardId);
        scrumboard.Should().NotBeNull();
        var columnId = scrumboard.Columns.First().Id;
        
        //Act
        scrumboardService.AddCardToOrder(columnId, position,  new Card
        {
            Name = "test",
        });
        
        //Assert
        scrumboard = scrumboardRepository.LoadScrumboardWithColumnAndCards().FirstOrDefault();

        scrumboard.Should().NotBeNull();

        scrumboard.Columns.Should().BeInAscendingOrder(x => x.Order);
        scrumboard.Columns.Should().AllSatisfy(column =>
        {
            column.Cards.Should().BeInAscendingOrder(card => card.Order);
        }); 
    }


}