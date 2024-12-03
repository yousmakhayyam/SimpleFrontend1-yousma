using System.ComponentModel.DataAnnotations;
using Warehouse.Domain.Models.Enums;

namespace Warehouse.Domain.Models.Common;

public class Money
{
    [Range(0, int.MaxValue, ErrorMessage = "Amount must be a non-negative integer.")]
    public int Amount { get; set; }

    [Range(0, 99, ErrorMessage = "Cents must be between 0 and 99.")]
    public int Cents { get; set; }

    [Required]
    public Currency Currency { get; set; }

    public Money(int amount, int cents, Currency currency)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be non-negative.");
        if (cents < 0 || cents > 99) throw new ArgumentOutOfRangeException(nameof(cents), "Cents must be between 0 and 99.");
        Amount = amount;
        Cents = cents;
        Currency = currency;
    }

    public override string ToString()
    {
        return $"{Amount}.{Cents:D2} {Currency}";
    }
}