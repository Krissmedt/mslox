using mslox;

public class LoxInstance
{
    private LoxClass klass;
    private Dictionary<String, Object> fields = [];

    public LoxInstance(LoxClass klass)
    {
        this.klass = klass;
    }

    public object Get(Token name)
    {
        if (fields.ContainsKey(name.Lexeme))
        {
            return fields[name.Lexeme];
        }

        throw new RuntimeError(name, $"Undefined property {name.Lexeme}.");
    }

    public void Set(Token name, object value)
    {
        fields.Add(name.Lexeme, value);
    }

    public override string ToString()
    {
        return klass.name + " instance";
    }
}