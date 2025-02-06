namespace mslox;

public class Binary : IExpr
{
    public required IExpr Left;
    public required IExpr Right;
    public required Token Operator;
    
    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}