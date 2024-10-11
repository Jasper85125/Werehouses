using System;

public class BaseCS
{
    public BaseCS()
    {
    }

    public static string GetTimestampCS()
    {
        return DateTime.UtcNow.ToString("o");
    }
}
