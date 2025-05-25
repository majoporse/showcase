namespace Contracts.Commands;

public record RoutePackagesCommand(List<string> Destinations, string origin)
{
    public record Result(List<string> Destinations);
}
