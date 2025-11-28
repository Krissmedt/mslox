namespace mslox.Expression;

public class Set : IExpr
{
    public required IExpr Object { get; init; }
    public required Token Name { get; init; }
    public required IExpr Value { get; init; }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}