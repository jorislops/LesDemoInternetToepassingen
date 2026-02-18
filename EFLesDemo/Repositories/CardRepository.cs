using Ardalis.Specification.EntityFrameworkCore;
using EFLesDemo.Entities;

namespace EFLesDemo.Repositories;

public class CardRepository(ScrumboardDbContext db) : RepositoryBase<Card>(db), ISCardRepository
{
}