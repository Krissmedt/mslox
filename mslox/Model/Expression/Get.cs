namespace mslox.Expression;

public class Get : IExpr
{
    public required IExpr Object { get; init; }
    public required Token Name { get; init; }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}