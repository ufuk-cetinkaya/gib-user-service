using Application.DTOs;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Queries;

public class GetGibUserHandler
{
    private readonly IGibUserRepository _repository;

    public GetGibUserHandler(IGibUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<GibUserDto?> HandleAsync(GetGibUserQuery query)
    {
        var user = await _repository.GetGibUser(
            query.Identifier,
            query.DocumentType,
            query.Unit);

        if (user is null)
            return null;

        return user is null ? null : Map(user);
    }

    private static GibUserDto Map(GibUser user)
    {
        return new GibUserDto(
            user.Identifier,
            user.Title,
            user.UserType.ToString(),
            user.AccountType.ToString(),
            user.FirstCreationTime,
            user.DocumentType.ToString(),
            user.Unit.ToString(),
            user.Alias,
            user.AliasCreationTime);
    }
}