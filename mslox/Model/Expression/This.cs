namespace mslox.Expression;

public class This : IExpr
{
    public required Token keyword { get; init; }
    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}