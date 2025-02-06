namespace mslox;

public class Grouping : IExpr
{
    public required IExpr Expression;
    
    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}