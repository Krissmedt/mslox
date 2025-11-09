namespace mslox;

public interface LoxCallable
{
    public int Arity();
    public Object Call(Interpreter interpreter, List<Object> arguments);
}