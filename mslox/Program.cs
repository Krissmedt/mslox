
using mslox;

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


static void RunFile(String filePath)
{
    using (var reader = new StreamReader(filePath))
    {
        while (!reader.EndOfStream)
        {
            Run(reader.ReadLine()!);
        }
    }
    
    if (Lox.HadError)
    {
        Lox.Error(35, "Compilation failed");
        throw new ApplicationException("Lox Error");
    }
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
        {
            running = false;
        }
        else
        {
            Run(line);
        }
    }

    if (Lox.HadError)
    {
        throw new ApplicationException("Lox Error");
    }
}

static void Run(String source)
{
    Console.WriteLine("Compiling:");
    Console.WriteLine($"{source}");
    Lox.HadError = true;
}

