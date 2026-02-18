using EFLesDemo;
using EFLesDemo.Entities;

namespace EFLesDemo;

public class ScrumboardService(ScrumboardDbContext _db)
{
    
    public void AddCardEndOfColumn(int columnId, Card card)   
    {
        //It's better to only use repo methods inside a service
        
        var maxOrder = _db.Cards.Where(x => x.ScrumboardColumnId == columnId)
            .Max(x => x.Order);
        card.Order = maxOrder + 1;
        
        card.ScrumboardColumnId = columnId;
        
        _db.Cards.Add(card);
        _db.SaveChanges();
    }

    public void AddCardToOrder(int columnId, int order, Card card)
    {
        var cardsWithHigherOrder = 
            _db.Cards.Where(x => x.ScrumboardColumnId == columnId
                                    && x.Order >= order
                        )
            .OrderBy(x => x.Order)
            .ToList();
        
        cardsWithHigherOrder.ForEach(x => x.Order = x.Order + 1);
        //no need to update the changes!!!!
        //EF tracks entities (change tracker)

        card.Order = order;
        
        card.ScrumboardColumnId = columnId;
        
        _db.Cards.Add(card);
        _db.SaveChanges();
    }
}