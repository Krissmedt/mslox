namespace mslox.Statement;

using mslox;

public class Var : IStmt
{
    public Token Name;
    public Expression.IExpr Initializer;

    public Var(Token name, Expression.IExpr initializer)
    {
        this.Name = name;
        this.Initializer = initializer;
    }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}