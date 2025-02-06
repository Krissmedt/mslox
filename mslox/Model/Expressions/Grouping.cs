namespace mslox;

public class Grouping : IExpr
{
    public required IExpr Expression { get; init; }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}