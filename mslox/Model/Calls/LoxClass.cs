using mslox;

public class LoxClass : LoxCallable
{
    public String name;
    public Dictionary<String, LoxFunction> methods;

    public LoxClass(String name, Dictionary<String, LoxFunction> methods)
    {
        this.name = name;
        this.methods = methods;
    }

    public int Arity()
    {
        return 0;
    }

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        LoxInstance instance = new LoxInstance(this);
        return instance;
    }

    public LoxFunction FindMethod(String name)
    {
        LoxFunction output = null;
        methods.TryGetValue(name, out output);
        
        return output;
    }

    public override String ToString()
    {
        return name;
    }
}