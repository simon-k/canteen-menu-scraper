namespace CanteenParser.Utils;

public class KindParser
{
    public static string GetKind(string text)
    {
        if (text.Contains("salmon", StringComparison.CurrentCultureIgnoreCase) ||
             text.Contains("laks", StringComparison.CurrentCultureIgnoreCase) ||
             text.Contains("seafood", StringComparison.CurrentCultureIgnoreCase) ||
             text.Contains("fish", StringComparison.CurrentCultureIgnoreCase) ||
             text.Contains("Catch of the day", StringComparison.CurrentCultureIgnoreCase))
        {
            return "Fish";
        }

        if (text.Contains("chicken", StringComparison.CurrentCultureIgnoreCase) ||
            text.Contains("turkey", StringComparison.CurrentCultureIgnoreCase) ||
            text.Contains("kylling", StringComparison.CurrentCultureIgnoreCase))
        {
            return "Chicken";
        }

        if (text.Contains("pork", StringComparison.CurrentCultureIgnoreCase) ||
            text.Contains("pig", StringComparison.CurrentCultureIgnoreCase) || 
            text.Contains("bacon", StringComparison.CurrentCultureIgnoreCase))
        {
            return "Pork";
        }
        
        return "Meat";
    }
}