namespace CanteenParser;

public static class JsonReader
{
    public static string ReadJsonFile(string path)
    {
        var json = File.ReadAllText(path);
        return json;
    }
}