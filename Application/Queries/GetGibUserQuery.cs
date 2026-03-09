using Domain.Enums;

namespace Application.Queries;

public record GetGibUserQuery(string Identifier, DocType DocumentType, Unit Unit);