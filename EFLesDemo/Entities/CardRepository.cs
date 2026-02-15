using Ardalis.Specification.EntityFrameworkCore;

namespace EFLesDemo.Entities;

public class CardRepository(ScrumboardDbContext db) : RepositoryBase<Card>(db);