using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Todo.Core.Utilities
{
    public static class Verify
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNullOrWhiteSpace([NotNull] string? str, [CallerArgumentExpression(nameof(str))] string? paramName = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(str, paramName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull([NotNull] object? obj, [CallerArgumentExpression(nameof(obj))] string? paramName = null)
        {
            ArgumentNullException.ThrowIfNull(obj, paramName);
        }
    }
}
