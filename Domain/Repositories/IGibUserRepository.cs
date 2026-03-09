using Domain.Entities;
using Domain.Enums;

namespace Domain.Repositories;

public interface IGibUserRepository
{
    Task Add(IEnumerable<GibUser> entities);
    void Remove(List<GibUser> entities);
    Task Save();
    Task<GibUser?> GetGibUser(string identifier, DocType documentType, Unit unit);
    Task<List<GibUser>> GetGibUser(Unit unit);
}