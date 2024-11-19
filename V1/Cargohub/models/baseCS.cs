using System;

public class BaseCS
{
    public BaseCS()
    {
    }

    public static string GetTimestamp()
    {
        return DateTime.UtcNow.ToString("u");
    }

    public static DateTime GetTimestampCS()
    {
        return DateTime.UtcNow;
    }
}
