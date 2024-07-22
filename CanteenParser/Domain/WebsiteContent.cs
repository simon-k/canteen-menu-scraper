namespace CanteenParser.Domain;

public class WebsiteContent
{
    public bool AllowMobilePay { get; set; }
    public bool AllowSwish { get; set; }
    public Dictionary<string, Offer> Offers { get; set; } = new();
}

public class Offer
{
    public List<Item> Items { get; set; } = new();
}

public class Item
{
    public int UnitPrice { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Dictionary<string, Date> Dates { get; set; } = new();
}

public class Date
{
    public bool Available { get; set; }
    public Menu Menu { get; set; } = new();
}

public class Menu
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long DateSeconds { get; set; }
}