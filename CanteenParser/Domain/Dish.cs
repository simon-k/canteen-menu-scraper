namespace CanteenParser.Domain;

public class Dish
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset Date { get; set; }
    
    public override string ToString()
    {
        var text = Name == string.Empty ? "No dish found" : Name;
        return $"{Date.ToString("dd-MM-yyyy")}: {text}";
    }
}