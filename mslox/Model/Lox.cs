namespace mslox;

public class Lox
{
    public static Boolean HadError = false;
    
    public static void Error(Int64 line, String message)
    {
        Report(line, "", message);
    }

    static void Report(Int64 line, String where, String message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
    }
}