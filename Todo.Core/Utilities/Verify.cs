using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Todo.Core.Utilities
{
    public static class Verify
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void NotNullOrWhiteSpace([NotNull] string? str, [CallerArgumentExpression(nameof(str))] string? paramName = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(str, paramName);
        }
    }
}
