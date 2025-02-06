namespace mslox;

public class Binary : IExpr
{
    public required IExpr Left { get; init; }
    public required IExpr Right { get; init; }
    public required Token Operator { get; init; }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}