using mslox;
using mslox.Service;

try
{
    if (args.Length > 1)
    {
        Console.WriteLine("Usage: mslox [script]");
        return 64;
    }

    if (args.Length == 1)
    {
        RunFile(args[0]);
        return 0;
    }

    RunPrompt();
    return 0;
}
catch (Exception ex)
{
    return 70;
}


static void RunFile(string filePath)
{
    var sourceString = "";
    using (var reader = new StreamReader(filePath))
    {
        sourceString = reader.ReadToEnd();
    }

    Run(sourceString);

    if (Lox.HadError) throw new ApplicationException("Lox Error");
}

static void RunPrompt()
{
    Console.WriteLine("This is Lox");

    var running = true;
    while (running)
    {
        Console.Write("> ");
        var line = Console.ReadLine();
        if (line == null)
            running = false;
        else
            Run(line);
    }

    if (Lox.HadError) throw new ApplicationException("Lox Error");
}

static void Run(string source)
{
    var scanner = new Scanner { Source = source };
    var tokens = scanner.Scan();
    
    var parser = new Parser { Tokens = tokens };
    var expression = parser.Parse();

    // Stop if there was a syntax error.
    if (Lox.HadError) return;

    Console.WriteLine(new AstPrinter().Print(expression));
}