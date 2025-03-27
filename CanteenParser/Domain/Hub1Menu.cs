namespace CanteenParser.Domain;

public class Hub1Menu
{
    public WeeklyMenu Kays { get; set; }
    public WeeklyMenu Globetrotter { get; set; }
    public WeeklyMenu Homebound { get; set; }
}

public class WeeklyMenu
{
    public DayMenu Monday { get; set; }
    public DayMenu Tuesday { get; set; }
    public DayMenu Wednesday { get; set; }
    public DayMenu Thursday { get; set; }
    public DayMenu Friday { get; set; }
}

public class DayMenu
{
    public string MainDish { get; set; }
    public string Vegetarian { get; set; }
}