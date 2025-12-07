namespace mslox.Statement;

public class ClassStmt : IStmt
{
    public Token name;
    public Expression.Variable superclass;
    public List<Function> methods;

    public ClassStmt(Token name, Expression.Variable superclass, List<Function> methods)
    {
        this.name = name;
        this.superclass = superclass;
        this.methods = methods;
    }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}