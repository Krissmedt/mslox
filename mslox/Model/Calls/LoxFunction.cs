namespace mslox;

public class LoxFunction : LoxCallable
{
    private Environment closure;
    private Statement.Function declaration;

    public LoxFunction(Statement.Function declaration, Environment closure)
    {
        this.declaration = declaration;
        this.closure = closure;
    }

    public int Arity()
    {
        return declaration.Params.Count;
    }

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        var environment = new Environment(closure);
        for (var i = 0; i < declaration.Params.Count; i++)
        {
            environment.Define(declaration.Params[i].Lexeme, arguments[i]);
        }

        try
        {
            interpreter.ExecuteBlock(declaration.Body, environment);
        }
        catch (ReturnUnwind returnThrow)
        {
            return returnThrow.Value;
        }

        return null;
    }
    
    public LoxFunction Bind(LoxInstance instance)
    {
        var environment = new Environment(closure);
        environment.Define("this", instance);

        return new LoxFunction(declaration, environment);
    }

    public override string ToString()
    {
        return $"<fn {declaration.Name.Lexeme}>";
    }
}