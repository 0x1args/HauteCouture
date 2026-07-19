using HauteCouture.TenantManagement.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents a validated monetary value with an ISO 4217 currency code.
/// </summary>
public readonly partial record struct Money
{
    private static readonly Regex Pattern = CurrencyRegex();

    /// <summary>
    ///     The maximum allowed monetary amount.
    /// </summary>
    public const decimal MaxAmount = 100_000m;

    /// <summary>
    ///     The exact length of an ISO 4217 currency code.
    /// </summary>
    public const int CurrencyLength = 3;

    /// <summary>
    ///     The monetary amount.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    ///     The ISO 4217 currency code.
    /// </summary>
    public string Currency { get; }

    private Money(decimal amount, string currency)
        => (Amount, Currency) = (amount, currency);

    /// <summary>
    ///     Creates a <see cref="Money"/> instance from the specified amount and currency.
    /// </summary>
    /// <param name="amount">The monetary amount (must be greater than zero).</param>
    /// <param name="currency">The ISO 4217 currency code.</param>
    /// <returns>The validated value object.</returns>
    public static Money Of(decimal amount, string currency)
    {
        if (amount <= 0)
        {
            throw new SubscriptionPlanException("Price must be greater than zero.");
        }

        if (amount > MaxAmount)
        {
            throw new SubscriptionPlanException($"Price cannot exceed {MaxAmount}.");
        }

        var normalizedCurrency = NormalizeCurrency(currency);
        return new(amount, normalizedCurrency);
    }

    /// <summary>
    ///     Creates a zero amount <see cref="Money"/> instance for a given currency.
    /// </summary>
    /// <param name="currency">The ISO 4217 currency code.</param>
    /// <returns>A zero-value monetary object.</returns>
    public static Money Zero(string currency)
    {
        var normalizedCurrency = NormalizeCurrency(currency);
        return new(0, normalizedCurrency);
    }

    private static string NormalizeCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new SubscriptionPlanException("Currency cannot be empty.");
        }

        var normalizedCurrency = currency.Trim().ToUpperInvariant();

        if (normalizedCurrency.Length != CurrencyLength)
        {
            throw new SubscriptionPlanException($"Currency must be exactly {CurrencyLength} characters.");
        }
        if (!Pattern.IsMatch(normalizedCurrency))
        {
            throw new SubscriptionPlanException("Currency must be a valid ISO 4217 currency code.");
        }

        return normalizedCurrency;
    }

    [GeneratedRegex("^[A-Z]{3}$")]
    private static partial Regex CurrencyRegex();
}