using EFLesDemo.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFLesDemo;

public class ScrumboardRepository
{
    public static Scrumboard? LoadScrumboardWithColumnAndCardsById(int scrumboardId)
    {
        var db = new ScrumboardDbContext();
        var scrumboard = db.Scrumboards
            .Include(scrumboard => scrumboard.Columns.OrderBy(x => x.Order))
                .ThenInclude(column => column.Cards.OrderBy(x => x.Order))
            .FirstOrDefault(x => x.Id == scrumboardId);
        
        return scrumboard;
    }

    public static List<Scrumboard> LoadScrumboardWithColumnAndCards()
    {
        var db = new ScrumboardDbContext();
        var scrumboard = db.Scrumboards
            .Include(scrumboard => scrumboard.Columns.OrderBy(x => x.Order))
                .ThenInclude(column => column.Cards.OrderBy(x => x.Order))
            .ToList();
        
        return scrumboard;
    }

    public static bool DeleteScrumboard(int scrumboardId)
    {
        var db = new ScrumboardDbContext();
        // not working (only the scrumboard is loaded not the children (cols, cards)
        // var scrumboard = db.Scrumboards.Find(scrumboardId);
        
        //when the relationships are loaded, the children (cols, card) are deleted as well!
        var scrumboard = db.Scrumboards.Include(x => x. Columns)
            .ThenInclude(cols => cols.Cards)
            .FirstOrDefault(x => x.Id == scrumboardId);

        if (scrumboard is null)
            return false;
        
        db.Scrumboards.Remove(scrumboard);
        
        return db.SaveChanges() > 0;
    }
    
    public static Card? DeleteScrumCard(int scrumCardId)
    {
        var  db = new ScrumboardDbContext();
        var card =  db.Cards.Find(scrumCardId);
        if (card is null)
        {
            return null;
        }
        db.Cards.Remove(card);
        db.SaveChanges();
        return card;
    }
    
    public static Card AddCard(Card card)
    {
        var db = new ScrumboardDbContext();
        db.Cards.Add(card);
        db.SaveChanges();
        return card;
    }
    
    
}