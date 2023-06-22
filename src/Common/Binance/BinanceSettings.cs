namespace Marlin.Common.Binance;

public class BinanceSettings
{
    public string Key { get; set; } = default!;
    public string Secret { get; set; } = default!;
    public string BaseAddress { get; set; } = default!;
    public List<string> Symbols { get; set; } = new()
    {
        "FLMUSDT"
    };
}