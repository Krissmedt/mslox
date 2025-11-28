using mslox;

public class LoxClass : LoxCallable
{
    public String name;

    public LoxClass(String name)
    {
        this.name = name;
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

    public override String ToString()
    {
        return name;
    }
}