using CanteenParser.Domain;
using CanteenParser.Utils;

namespace CanteenParser;

public class HubNordicMenuParser
{
    public static List<Dish> GetAllDishes(HubNordicWebsiteContent websiteContent)
    {
        var dishes = new List<Dish>();

        foreach (var (day, dish) in websiteContent.KaysWeekMenu)
        {
            var newDish = new Dish
            {
                Date = GetDateFromWeekday(day),
                Kind = KindParser.GetKind(dish),
                Name = "Kays",
                Description = dish
            };
            dishes.Add(newDish);
        }

        //Add World Sprout Menu
        var sproutMenu = websiteContent.WorldWeekMenu["SPROUT MENU"];
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Mandag"), Kind = "Vegetarian", Name = "Sprout", Description = sproutMenu });
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Tirsdag"), Kind = "Vegetarian", Name = "Sprout", Description = sproutMenu });
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Onsdag"), Kind = "Vegetarian", Name = "Sprout", Description = sproutMenu });
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Torsdag"), Kind = "Vegetarian", Name = "Sprout", Description = sproutMenu });
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Fredag"), Kind = "Vegetarian", Name = "Sprout", Description = sproutMenu });

        var globetrotterMenu = websiteContent.WorldWeekMenu["GLOBETROTTER MENU"];
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Mandag"), Kind = KindParser.GetKind(globetrotterMenu), Name = "Globetrotter", Description = globetrotterMenu });
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Tirsdag"), Kind = KindParser.GetKind(globetrotterMenu), Name = "Globetrotter", Description = globetrotterMenu });
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Torsdag"), Kind = KindParser.GetKind(globetrotterMenu), Name = "Globetrotter", Description = globetrotterMenu });
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Fredag"), Kind = KindParser.GetKind(globetrotterMenu), Name = "Globetrotter", Description = globetrotterMenu });

        var globetrotterVegeMenu = websiteContent.WorldWeekMenu["ONSDAG I GLOBETROTTER (VEGETAR)"];
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Onsdag"), Kind = "Vegetarian", Name = "Globetrotter", Description = globetrotterVegeMenu });

        var homeboundMenu = websiteContent.WorldWeekMenu["HOMEBOUND MENU"];
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Mandag"), Kind = KindParser.GetKind(homeboundMenu), Name = "Homebound", Description = homeboundMenu });
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Tirsdag"), Kind = KindParser.GetKind(homeboundMenu), Name = "Homebound", Description = homeboundMenu });
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Torsdag"), Kind = KindParser.GetKind(homeboundMenu), Name = "Homebound", Description = homeboundMenu });
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Fredag"), Kind = KindParser.GetKind(homeboundMenu), Name = "Homebound", Description = homeboundMenu });
        
        var homeboundVegeMenu = websiteContent.WorldWeekMenu["ONSDAG I HOMEBOUND (VEGETAR)"];
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Onsdag"), Kind = "Vegetarian", Name = "Homebound", Description = homeboundVegeMenu });
        
        return dishes.Where(d => d.Date >= DateTimeOffset.UtcNow.Date ).OrderBy(d => d.Date).ToList();
    }
    
    private static DateTimeOffset GetDateFromWeekday(string weekday)
    {
        var dayOfWeek = WeekdayToDayOfWeek(weekday);
        var todaysDate = DateTime.Today;
        var daysUntil = ((int)dayOfWeek - (int)todaysDate.DayOfWeek);
        var date = todaysDate.AddDays(daysUntil);
        return date;
    }
    
    private static DayOfWeek WeekdayToDayOfWeek(string weekday)
    {
        return weekday switch
        {
            "Mandag" => DayOfWeek.Monday,
            "Tirsdag" => DayOfWeek.Tuesday,
            "Onsdag" => DayOfWeek.Wednesday,
            "Torsdag" => DayOfWeek.Thursday,
            "Fredag" => DayOfWeek.Friday,
            "Lørdag" => DayOfWeek.Saturday,
            "Søndag" => DayOfWeek.Sunday,
            _ => throw new ArgumentException("Weekday not found")
        };
    }
}