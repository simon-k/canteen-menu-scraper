namespace CanteenParser.Domain;

public class Dish
{
    public required DateTimeOffset Date { get; set; }
    public string When => Date.AddHours(11).ToString("dd-MM-yyyy HH:mm");
    public long ExpireUnixMs => Date.AddHours(14).ToUnixTimeMilliseconds();
    public required string Kind { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    
    public override string ToString()
    {
        var text = Name == string.Empty ? "No dish found" : Name;
        return $"{Date.ToString("dd-MM-yyyy")}: {text}";
    }
}