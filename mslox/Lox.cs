namespace mslox;

public class Lox
{
    public static bool HadError;
    public static bool HadRuntimeError;

    public static void Error(long line, String message)
    {
        HadError = true;
        Report(line, "", message);
    }
    
    public static void Error(Token token, String message)
    {
        HadError = true;
        if (token.Type == TokenType.Eof)
        {
            Report(token.Line, " at end", message);
        }
        else
        {
            Report(token.Line, $" at '{token.Lexeme}'", message);
        }
    }

    public static void RuntimeError(RuntimeError error)
    {
        var sw = new StreamWriter(Console.OpenStandardError());
        Console.SetError(sw);
        Console.WriteLine($"{error.Message}\n[line {error.token.Line}]");
        HadRuntimeError = true;
        }

    private static void Report(long line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
    }
}