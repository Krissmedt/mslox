namespace mslox;

public class Unary : IExpr
{
    public required IExpr Right;
    public required Token Operator;
    
    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}