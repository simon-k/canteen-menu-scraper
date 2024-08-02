using System.Text.RegularExpressions;
using CanteenParser.Domain;

namespace CanteenParser;

public static class MenuParser
{
    private const string VEGITARIAN_DISH = "Green dish of the day";
    private const string MEAT_DISH = "Dish of the day";


    public static List<Dish> GetAllDishes(int days, WebsiteContent websiteContent)
    {
        var dishes = new List<Dish>();
        for(var i = 0; i < days; i++)
        {
            try
            {
                var vegetarianDish = GetVegetarianDish(websiteContent, i);
                if (vegetarianDish != null)
                {
                    Console.WriteLine($"Vegetarian menu: {vegetarianDish}");
                    dishes.Add(vegetarianDish);
                }

                var meatDish = GetDish(websiteContent, i);
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
    
    public static Dish? GetVegetarianDish(WebsiteContent websiteContent, int dateOffset)
    {
        var date = GetTodaysDateInUnixTimestamp(dateOffset);
        return GetMenu(websiteContent, VEGITARIAN_DISH, date);
    }
    
    public static Dish? GetDish(WebsiteContent websiteContent, int dateOffset)
    {
        var date = GetTodaysDateInUnixTimestamp(dateOffset);
        return GetMenu(websiteContent, MEAT_DISH, date);
    }
    
    private static Dish? GetMenu(WebsiteContent websiteContent, string itemName, string dateUnixSeconds)
    {
        var items = FindItemByName(websiteContent, itemName);
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
            Kind = itemName.Equals(VEGITARIAN_DISH) ? "Vegetarian" : GetKind(todaysMenu.Menu.Name)
        };
    }
    
    private static string GetTodaysDateInUnixTimestamp(double offset = 0)
    {
        var today = DateTime.Today.AddDays(offset);
        var todayDateTomeOffset = new DateTimeOffset(today.Year, today.Month, today.Day, 0, 0, 0, TimeSpan.Zero);
        var todayUnixSeconds = todayDateTomeOffset.ToUnixTimeSeconds();
        return todayUnixSeconds.ToString();        
    }
    
    private static Item FindItemByName(WebsiteContent websiteContent, string name)
    {
        var item = websiteContent.Offers
            .SelectMany(offer => offer.Value.Items)
            .FirstOrDefault(item => item.Name == name);

        return item!;
    }
    
    private static string GetKind(string text)
    {
        if (text.Contains("fish", StringComparison.CurrentCultureIgnoreCase))
        {
            return "Fish";
        }

        if (text.Contains("pork", StringComparison.CurrentCultureIgnoreCase))
        {
            return "Pork";
        }

        if (text.Contains("chicken", StringComparison.CurrentCultureIgnoreCase))
        {
            return "Chicken";
        }
       
        return "Meat";
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