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

    Run(sourceString, new Interpreter());

    if (Lox.HadError) throw new ApplicationException("Lox Error");
    if (Lox.HadRuntimeError) throw new ApplicationException("Lox Error");
}

static void RunPrompt()
{
    var sessionInterpreter = new Interpreter();
    Console.WriteLine("This is Lox");

    var running = true;
    while (running)
    {
        Console.Write("> ");
        var line = Console.ReadLine();
        if (line == null)
            running = false;
        else
            Run(line, sessionInterpreter);
    }

    if (Lox.HadError) throw new ApplicationException("Lox Error");
}

static void Run(string source, Interpreter interpreter)
{
    var scanner = new Scanner { Source = source };
    var tokens = scanner.Scan();

    var parser = new Parser { Tokens = tokens };
    var program = parser.Parse();

    // Stop if there was a syntax error.
    if (Lox.HadError)
    {
        Lox.HadError = false;
        return;
    }

    var resolver = new Resolver(interpreter);
    resolver.Resolve(program);

    // Stop if there was a resolution error.
    if (Lox.HadError)
    {
        Lox.HadError = false;
        return;
    }

    interpreter.Interpret(program);
}