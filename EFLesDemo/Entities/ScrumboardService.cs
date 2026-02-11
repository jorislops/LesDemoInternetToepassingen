namespace EFLesDemo.Entities;

public class ScrumboardService
{
    public static void AddCardEndOfColumn(int columnId, Card card)   
    {
        //It's better to only use repo methods inside a service
        
        var db = new ScrumboardDbContext();
        var maxOrder = db.Cards.Where(x => x.ScrumboardColumnId == columnId)
            .Max(x => x.Order);
        card.Order = maxOrder + 1;
        
        db.Cards.Add(card);
        db.SaveChanges();
    }

    public static void AddCardToOrder(int columnId, int order, Card card)
    {
        var db =  new ScrumboardDbContext();

        var cardsWithHigherOrder = 
            db.Cards.Where(x => x.ScrumboardColumnId == columnId
                                    && x.Order >= order
                        )
            .OrderBy(x => x.Order)
            .ToList();
        
        cardsWithHigherOrder.ForEach(x => x.Order = x.Order + 1);
        //no need to update the changes!!!!
        //EF tracks entities

        card.Order = order;
        
        db.Cards.Add(card);
        db.SaveChanges();

    }
}