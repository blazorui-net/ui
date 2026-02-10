namespace BlazorBlueprint.Tests.ApiSurface;

public class ComponentsApiSurfaceTests
{
    [Fact]
    public Task ComponentsApiSurfaceMatchesBaseline()
    {
        var assembly = typeof(BlazorBlueprint.Components.Button.Button).Assembly;
        var apiSurface = ApiSurfaceGenerator.Generate(assembly);
        return Verify(apiSurface);
    }
}
