using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GibUserRepository : IGibUserRepository
{
    private readonly GibUserDbContext _db;

    public GibUserRepository(GibUserDbContext db)
    {
        _db = db;
    }

    public async Task Add(IEnumerable<GibUser> entities)
    {
        await _db.AddRangeAsync(entities);
    }

    public void Remove(List<GibUser> entities)
    {
        _db.RemoveRange(entities);
    }

    public async Task Save()
    {
        await _db.SaveChangesAsync();
    }

    public async Task<GibUser?> GetGibUser(string identifier, DocType documentType, Unit unit)
    {
        return await _db.GibUsers
            .AsNoTracking()
            .Where(x =>
            x.Identifier == identifier &&
            x.DocumentType == documentType &&
            x.Unit == unit &&
            x.IsActive)
            .FirstOrDefaultAsync();
    }

    public async Task<List<GibUser>> GetGibUser(Unit unit)
    {
        return await _db.GibUsers
            .AsNoTracking()
            .Where(x => x.Unit == unit)
            .ToListAsync();
    }
}