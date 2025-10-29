using Microsoft.VisualBasic;
using mslox;

public class Environment
{
    private Dictionary<String, Object> values = new();

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

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public Object Get(Token name)
    {
        if (values.TryGetValue(name.Lexeme, out var value))
        {
            return value;
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }
}