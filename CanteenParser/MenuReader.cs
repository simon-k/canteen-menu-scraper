using CanteenParser.Domain;

namespace CanteenParser;

public static class MenuReader
{
    public static Dish GetVegetarianDish(Frontend frontend, int dateOffset)
    {
        var date = GetTodaysDateInUnicTimestamp(dateOffset);
        return GetMenu(frontend, "Green dish of the day", date);
    }
    
    public static Dish GetDish(Frontend frontend, int dateOffset)
    {
        var date = GetTodaysDateInUnicTimestamp(dateOffset);
        return GetMenu(frontend, "Dish of the day", date);
    }
    
    public static Dish GetTodaysVegetarianDish(Frontend frontend)
    {
        return GetTodaysMenu(frontend, "Green dish of the day");
    }
    
    public static Dish GetTodaysDish(Frontend frontend)
    {
        return GetTodaysMenu(frontend, "Dish of the day");
    }

    public static Dish GetTomorrowsVegetarianDish(Frontend frontend)
    {
        return GetTomorrowsMenu(frontend, "Green dish of the day");
    }
    
    public static Dish GetTomorrowsDish(Frontend frontend)
    {
        return GetTomorrowsMenu(frontend, "Dish of the day");
    }
    
    private static Dish GetTodaysMenu(Frontend frontend, string itemName)
    {
        var todaysDate = GetTodaysDateInUnicTimestamp();
        return GetMenu(frontend, itemName, todaysDate);
    }

    private static Dish GetTomorrowsMenu(Frontend frontend, string itemName)
    {
        var tomorrowsDate = GetTodaysDateInUnicTimestamp(1);
        return GetMenu(frontend, itemName, tomorrowsDate);
    }
    
    private static Dish GetMenu(Frontend frontend, string itemName, string dateUnixSeconds)
    {
        var items = FindItemByName(frontend, itemName);
        var todaysMenu = items.Dates[dateUnixSeconds];
        return new Dish
        {
            Name = todaysMenu.Menu.Name,
            Description = todaysMenu.Menu.Description,
            Date = DateTimeOffset.FromUnixTimeSeconds(todaysMenu.Menu.DateSeconds)
        };
    }
    
    private static string GetTodaysDateInUnicTimestamp(double offset = 0)
    {
        var today = DateTime.Today.AddDays(offset);
        var dto = new DateTimeOffset(today.Year, today.Month, today.Day, 0, 0, 0, TimeSpan.Zero);
        var todayUnixSeconds = dto.ToUnixTimeSeconds();
        return todayUnixSeconds.ToString();        
    }
    
    private static Item FindItemByName(Frontend frontend, string name)
    {
        var item = frontend.Offers
            .SelectMany(offer => offer.Value.Items)
            .FirstOrDefault(item => item.Name == name);

        return item;
    }
}