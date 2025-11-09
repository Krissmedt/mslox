namespace mslox;
using mslox.Statement;

public class Return : IStmt
{
    public required Token Keyword { get; init; }
    public required Expression.IExpr Value { get; init; }
    
    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}