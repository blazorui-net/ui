using System.Runtime.CompilerServices;

namespace BlazorBlueprint.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        DiffTools.UseOrder(DiffTool.None);
    }
}
