using System.Text.RegularExpressions;
using CanteenParser.Domain;
using CanteenParser.Utils;

namespace CanteenParser;

public static class KanplaMenuParser
{
    private const string VEGITARIAN_DISH = "Green dish of the day";
    private const string MEAT_DISH = "Dish of the day";

    public static List<Dish> GetAllDishes(int days, KanplaWebsiteContent kanplaWebsiteContent)
    {
        var dishes = new List<Dish>();
        for(var i = 0; i < days; i++)
        {
            try
            {
                var vegetarianDish = GetVegetarianDish(kanplaWebsiteContent, i);
                if (vegetarianDish != null)
                {
                    Console.WriteLine($"Vegetarian menu: {vegetarianDish}");
                    dishes.Add(vegetarianDish);
                }

                var meatDish = GetDish(kanplaWebsiteContent, i);
                if (meatDish != null)
                {
                    Console.WriteLine($"Menu: {meatDish}");
                    dishes.Add(meatDish);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not find the days menu. {e.Message}");
            }
        }
        return dishes;
    }
    
    public static Dish? GetVegetarianDish(KanplaWebsiteContent kanplaWebsiteContent, int dateOffset)
    {
        var date = GetTodaysDateInUnixTimestamp(dateOffset);
        return GetMenu(kanplaWebsiteContent, VEGITARIAN_DISH, date);
    }
    
    public static Dish? GetDish(KanplaWebsiteContent kanplaWebsiteContent, int dateOffset)
    {
        var date = GetTodaysDateInUnixTimestamp(dateOffset);
        return GetMenu(kanplaWebsiteContent, MEAT_DISH, date);
    }
    
    private static Dish? GetMenu(KanplaWebsiteContent kanplaWebsiteContent, string itemName, string dateUnixSeconds)
    {
        var items = FindItemByName(kanplaWebsiteContent, itemName);
        var todaysMenu = items.Dates.GetValueOrDefault(dateUnixSeconds);          
        
        if (todaysMenu == null || string.IsNullOrWhiteSpace(todaysMenu.Menu.Name))
        {
            Console.WriteLine($"Menu not found for unix timestamp {dateUnixSeconds}");
            return null;
        }
        
        return new Dish
        {
            Name = TrimUnwantedCharacters(todaysMenu.Menu.Name),
            Description = TrimUnwantedCharacters(todaysMenu.Menu.Description),
            Date = DateTimeOffset.FromUnixTimeSeconds(todaysMenu.Menu.DateSeconds),
            Kind = itemName.Equals(VEGITARIAN_DISH) ? "Vegetarian" : KindParser.GetKind(todaysMenu.Menu.Name)
        };
    }
    
    private static string GetTodaysDateInUnixTimestamp(double offset = 0)
    {
        var today = DateTime.Today.AddDays(offset);
        var todayDateTomeOffset = new DateTimeOffset(today.Year, today.Month, today.Day, 0, 0, 0, TimeSpan.Zero);
        var todayUnixSeconds = todayDateTomeOffset.ToUnixTimeSeconds();
        return todayUnixSeconds.ToString();        
    }
    
    private static Item FindItemByName(KanplaWebsiteContent kanplaWebsiteContent, string name)
    {
        var item = kanplaWebsiteContent.Offers
            .SelectMany(offer => offer.Value.Items)
            .FirstOrDefault(item => item.Name == name);

        return item!;
    }
    
    private static string TrimUnwantedCharacters(string input)
    {
        //REmove newlines, tabs and multiple spaces
        var trimmed = input
            .Replace("\n", "")
            .Replace("\r", "")
            .Replace("\t", "")
            .Replace(" ,", ",")
            .Trim();
        
        var regex = new Regex("[ ]{2,}", RegexOptions.None);     
        trimmed = regex.Replace(trimmed, " ");

        return trimmed;
    }
}
