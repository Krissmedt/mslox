namespace mslox.Expression;

public class Super : IExpr
{
    public required Token keyword { get; init; }
    public required Token method { get; init; }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}