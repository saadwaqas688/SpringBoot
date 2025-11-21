namespace Shared.Utils;

public static class DateTimeHelper
{
    public static DateTime GetUtcNow()
    {
        return DateTime.UtcNow;
    }

    public static string ToIsoString(DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
}

