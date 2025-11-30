using Microsoft.VisualBasic;
using mslox;

public class Environment
{
    private Environment? enclosing;
    private Dictionary<String, Object> values = new();

    public Environment()
    {
        this.enclosing = null;
    }

    public Environment(Environment enclosing)
    {
        this.enclosing = enclosing;
    }

    public void Define(String name, Object value)
    {
        values[name] = value;
    }

    public void Assign(Token name, Object value)
    {
        if (values.TryGetValue(name.Lexeme, out var ignored))
        {
            values[name.Lexeme] = value;
            return;
        }

        if (enclosing != null)
        {
            enclosing.Assign(name, value);
            return;
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void AssignAt(int distance, Token name, Object value)
    {
        Ancestor(distance).Assign(name, value);
    }

    public Object Get(Token name)
    {
        if (values.TryGetValue(name.Lexeme, out var value))
        {
            return value;
        }

        if (enclosing != null) return enclosing.Get(name);

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public Object GetAt(int distance, Token name)
    {
        return Ancestor(distance).Get(name);
    }

    public Object Get(String name)
    {
        if (values.TryGetValue(name, out var value))
        {
            return value;
        }

        if (enclosing != null) return enclosing.Get(name);

        throw new RuntimeError(null, $"Undefined variable '{name}'.");
    }

    public Object GetAt(int distance, String name)
    {
        return Ancestor(distance).Get(name);
    }

    public Environment Ancestor(int distance)
    {
        var environment = this;
        for (var i = 0; i < distance; i++)
        {
            environment = environment.enclosing;
        }

        return environment;
    }
}