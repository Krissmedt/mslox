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
        var initializer = FindMethod("init");
        if (initializer == null) return 0;
        return initializer.Arity();
    }

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        LoxInstance instance = new LoxInstance(this);
        var initializer = FindMethod("init");
        if (initializer != null)
        {
            initializer.Bind(instance).Call(interpreter, arguments);
        }

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