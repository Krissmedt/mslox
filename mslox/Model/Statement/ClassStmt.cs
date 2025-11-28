namespace mslox.Statement;

public class ClassStmt : IStmt
{
    public Token name;
    public List<Function> methods;

    public ClassStmt(Token name, List<Function> methods)
    {
        this.name = name;
        this.methods = methods;
    }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}