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

    public Object Get(Token name)
    {
        if (values.TryGetValue(name.Lexeme, out var value))
        {
            return value;
        }

        if (enclosing != null) return enclosing.Get(name);

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }
}