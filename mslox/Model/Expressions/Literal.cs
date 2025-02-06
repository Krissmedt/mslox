namespace mslox;

public class Literal : IExpr
{
    public required Object Value;
    
    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}