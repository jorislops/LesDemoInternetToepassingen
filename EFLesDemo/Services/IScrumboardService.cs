using EFLesDemo.Entities;

namespace EFLesDemo.Services;

public interface IScrumboardService
{
    ScrumboardCard InsertOrUpdateCardPreserveOrder(ScrumboardCard card);
    // ScrumboardCard AddCardEndOfColumn(int columnId, ScrumboardCard scrumboardCard);
    // void AddCardPreserveOrder(int columnId, int order, ScrumboardCard scrumboardCard);
}