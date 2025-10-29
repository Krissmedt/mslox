namespace mslox.Expression;

public class Assign : IExpr
{
    public Token Name;
    public IExpr Value;

    public Assign(Token name, IExpr value)
    {
        this.Name = name;
        this.Value = value;
    }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}