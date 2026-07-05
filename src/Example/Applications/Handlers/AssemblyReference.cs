using System.Reflection;

namespace HauteCouture.Example.Applications.Handlers;

/// <summary>
///     Reference to the assembly containing the Example module's CQS handlers.
/// </summary>
public static class AssemblyReference
{
    /// <summary>Assembly reference.</summary>
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}