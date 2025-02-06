namespace mslox;

public class Literal : IExpr
{
    public required object? Value { get; init; }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}