using EFLesDemo.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFLesDemo;

public class ScrumboardRepository(ScrumboardDbContext _db)
{
    public Scrumboard? LoadScrumboardWithColumnAndCardsById(int scrumboardId)
    {
        var scrumboard = _db.Scrumboards
            .Include(scrumboard => scrumboard.Columns.OrderBy(x => x.Order))
            .ThenInclude(column => column.Cards.OrderBy(x => x.Order))
            .FirstOrDefault(x => x.Id == scrumboardId);
        
        return scrumboard;
    }

    public List<Scrumboard> LoadScrumboardWithColumnAndCards()
    {
        var scrumboard = _db.Scrumboards
            .Include(scrumboard => scrumboard.Columns.OrderBy(x => x.Order))
                .ThenInclude(column => column.Cards.OrderBy(x => x.Order))
            .ToList();
        
        return scrumboard;
    }

    public bool DeleteScrumboard(int scrumboardId)
    {
        // not working (only the scrumboard is loaded not the children (cols, cards)
        // var scrumboard = db.Scrumboards.Find(scrumboardId);
        
        //when the relationships are loaded, the children (cols, card) are deleted as well!
        var scrumboard = _db.Scrumboards.Include(x => x. Columns)
            .ThenInclude(cols => cols.Cards)
            .FirstOrDefault(x => x.Id == scrumboardId);
        
        if (scrumboard is null)
            return false;
        
        _db.Scrumboards.Remove(scrumboard);
        
        return _db.SaveChanges() > 0;
    }
    
    public Card? DeleteScrumCard(int scrumCardId)
    {
        var card =  _db.Cards.Find(scrumCardId);
        if (card is null)
        {
            return null;
        }
        _db.Cards.Remove(card);
        _db.SaveChanges();
        return card;
    }
    
    public  Card AddCard(Card card)
    {
        _db.Cards.Add(card);
        _db.SaveChanges();
        return card;
    }
    
    
}