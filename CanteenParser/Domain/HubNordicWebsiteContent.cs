namespace CanteenParser.Domain;

public class HubNordicWebsiteContent
{
    public required Dictionary<string, string> KaysWeekMenu { get; set; }

    public required Dictionary<string, string> WorldWeekMenu { get; set; }
}