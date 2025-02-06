namespace mslox;

public class Lox
{
    public static bool HadError;

    public static void Error(long line, string message)
    {
        HadError = true;
        Report(line, "", message);
    }

    private static void Report(long line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
    }
}