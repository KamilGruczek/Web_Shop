using System.Diagnostics.CodeAnalysis;

namespace Web_Shop.Application.Extensions;

public static class StringExtensions
{
    public static bool IsNotNull([NotNullWhen(true)]this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
    
    public static bool IsNulL([NotNullWhen(false)]this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }
}