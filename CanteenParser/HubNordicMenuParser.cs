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
                Kind = day.Equals("Onsdag") ?  KindParser.GetKind(dish, "Vegetarian") : KindParser.GetKind(dish),
                Name = "Kays",
                Description = dish
            };
            dishes.Add(newDish);
        }

        //Add World Sprout Menu
        var sproutMenu = websiteContent.WorldWeekMenu["SPROUT MENU"];
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Mandag"), Kind = KindParser.GetKind(sproutMenu, "Vegetarian"), Name = "Sprout", Description = sproutMenu });
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Tirsdag"), Kind = KindParser.GetKind(sproutMenu, "Vegetarian"), Name = "Sprout", Description = sproutMenu });
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Onsdag"), Kind = KindParser.GetKind(sproutMenu, "Vegetarian"), Name = "Sprout", Description = sproutMenu });
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Torsdag"), Kind = KindParser.GetKind(sproutMenu, "Vegetarian"), Name = "Sprout", Description = sproutMenu });
        dishes.Add(new Dish{ Date = GetDateFromWeekday("Fredag"), Kind = KindParser.GetKind(sproutMenu, "Vegetarian"), Name = "Sprout", Description = sproutMenu });

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
        
        var filteretDishes =  dishes.Where(d => d.Date >= DateTimeOffset.UtcNow.Date ).OrderBy(d => d.Date).ToList();

        foreach (var filteretDish in filteretDishes)
        {
            Console.WriteLine($"menu: {filteretDish.Date} {filteretDish.Name} {filteretDish.Description}");
        }
        
        return filteretDishes;
    }
    
    private static DateTimeOffset GetDateFromWeekday(string weekday)
    {
        var dayOfWeek = WeekdayToDayOfWeek(weekday);
        return GetDateFromWeekday(dayOfWeek);
    }

    private static DateTimeOffset GetDateFromWeekday(DayOfWeek dayOfWeek)
    {
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

    public static List<Dish> GetAllDishes(Hub1Menu menu)
    {
        var dishes = new List<Dish>();

        //Kays
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Monday),    Kind = KindParser.GetKind(menu.Kays.Monday.MainDish),   Name = "Kays", Description = menu.Kays.Monday.MainDish });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Tuesday),   Kind = KindParser.GetKind(menu.Kays.Tuesday.MainDish),  Name = "Kays", Description = menu.Kays.Tuesday.MainDish });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Thursday),  Kind = KindParser.GetKind(menu.Kays.Thursday.MainDish), Name = "Kays", Description = menu.Kays.Thursday.MainDish });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Friday),    Kind = KindParser.GetKind(menu.Kays.Friday.MainDish),   Name = "Kays", Description = menu.Kays.Friday.MainDish });
        
        //Kays - Vegetarian
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Monday),    Kind = "Vegetarian", Name = "Kays", Description = menu.Kays.Monday.Vegetarian });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Tuesday),   Kind = "Vegetarian", Name = "Kays", Description = menu.Kays.Tuesday.Vegetarian });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Wednesday), Kind = "Vegetarian", Name = "Kays", Description = menu.Kays.Wednesday.Vegetarian });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Thursday),  Kind = "Vegetarian", Name = "Kays", Description = menu.Kays.Thursday.Vegetarian });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Friday),    Kind = "Vegetarian", Name = "Kays", Description = menu.Kays.Friday.Vegetarian });
        
        //Homebound
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Monday),    Kind = KindParser.GetKind(menu.Homebound.Monday.MainDish),   Name = "Homebound", Description = menu.Homebound.Monday.MainDish });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Tuesday),   Kind = KindParser.GetKind(menu.Homebound.Tuesday.MainDish),  Name = "Homebound", Description = menu.Homebound.Tuesday.MainDish });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Wednesday), Kind = "Vegetarian",                                         Name = "Homebound", Description = menu.Homebound.Wednesday.MainDish });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Thursday),  Kind = KindParser.GetKind(menu.Homebound.Thursday.MainDish), Name = "Homebound", Description = menu.Homebound.Thursday.MainDish });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Friday),    Kind = KindParser.GetKind(menu.Homebound.Friday.MainDish),   Name = "Homebound", Description = menu.Homebound.Friday.MainDish });
        
        //Globetrotter
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Monday),    Kind = KindParser.GetKind(menu.Globetrotter.Monday.MainDish),   Name = "Globetrotter", Description = menu.Globetrotter.Monday.MainDish });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Tuesday),   Kind = KindParser.GetKind(menu.Globetrotter.Tuesday.MainDish),  Name = "Globetrotter", Description = menu.Globetrotter.Tuesday.MainDish });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Wednesday), Kind = "Vegetarian",                                            Name = "Globetrotter", Description = menu.Globetrotter.Wednesday.MainDish });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Thursday),  Kind = KindParser.GetKind(menu.Globetrotter.Thursday.MainDish), Name = "Globetrotter", Description = menu.Globetrotter.Thursday.MainDish });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Friday),    Kind = KindParser.GetKind(menu.Globetrotter.Friday.MainDish),   Name = "Globetrotter", Description = menu.Globetrotter.Friday.MainDish });
        
        //Sprout
        /*dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Monday),    Kind = "Vegetarian", Name = "Sprout", Description = "Salatbar" });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Tuesday),   Kind = "Vegetarian", Name = "Sprout", Description = "Salatbar" });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Wednesday), Kind = "Vegetarian", Name = "Sprout", Description = "Salatbar" });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Thursday),  Kind = "Vegetarian", Name = "Sprout", Description = "Salatbar" });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Friday),    Kind = "Vegetarian", Name = "Sprout", Description = "Salatbar" });*/
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Monday),    Kind = KindParser.GetKind(menu.Sprout.Monday.MainDish),   Name = "Sprout", Description = menu.Sprout.Monday.MainDish });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Tuesday),   Kind = KindParser.GetKind(menu.Sprout.Tuesday.MainDish),  Name = "Sprout", Description = menu.Sprout.Tuesday.MainDish });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Wednesday), Kind = "Vegetarian",                                      Name = "Sprout", Description = menu.Sprout.Wednesday.MainDish });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Thursday),  Kind = KindParser.GetKind(menu.Sprout.Thursday.MainDish), Name = "Sprout", Description = menu.Sprout.Thursday.MainDish });
        dishes.Add(new Dish { Date = GetDateFromWeekday(DayOfWeek.Friday),    Kind = KindParser.GetKind(menu.Sprout.Friday.MainDish),   Name = "Sprout", Description = menu.Sprout.Friday.MainDish });
        
        var filteretDishes =  dishes.Where(d => d.Date >= DateTimeOffset.UtcNow.Date ).OrderBy(d => d.Date).ToList();
        
        foreach (var filteretDish in filteretDishes)
        {
            Console.WriteLine($"menu: {filteretDish.Date} {filteretDish.Name} {filteretDish.Description}");
        }
        
        return filteretDishes;
    }
}