namespace HauteCouture.Example.HostSide.Public.WebApi.Endpoints;

/// <summary>
///     Defines route templates for the <c>Something</c> endpoints.
/// </summary>
public static class Routes
{
    public const string SomethingBase = "/something";

    public const string CreateSomething = $"{SomethingBase}";

    public const string UpdateSomethingDescription = $"{SomethingBase}/{{somethingId:guid}}/description";

    public const string DeleteSomething = $"{SomethingBase}/{{somethingId:guid}}";

    public const string GetSomething = $"{SomethingBase}/{{somethingId:guid}}";
}