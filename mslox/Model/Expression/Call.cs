namespace mslox.Expression;

public class Call : IExpr
{
    public required IExpr Callee { get; init; }
    public required Token Paren { get; init; }
    public required List<IExpr> Arguments { get; init; }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}