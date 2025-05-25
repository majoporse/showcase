namespace Contracts.Queries;

public record GetAvailableCourierQuery()
{
    public record Result(Guid CourierId);
}