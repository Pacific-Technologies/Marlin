namespace Marlin.Orders.Util;

/// <summary>
/// Descending decimal comparer
/// </summary>
public class DescendingDecimalComparer : IComparer<decimal>
{
    public int Compare(decimal x, decimal y) =>
        decimal.Compare(x, y) * -1;

    // public int Compare(object x, object y)
    // {
    //     if (x == y)
    //     {
    //         return 0;
    //     }
    //
    //     if (x == null)
    //     {
    //         return -1;
    //     }
    //
    //     if (y == null)
    //     {
    //         return 1;
    //     }
    //
    //     if (x is decimal a
    //         && y is decimal b)
    //     {
    //         return Compare(a, b);
    //     }
    //
    //     throw new ArgumentException("Must be decimal", nameof(x));
    // }
}