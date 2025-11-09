namespace mslox.Statement;
using mslox;

public class Function : IStmt
{
    public required Token Name { get; init; }
    public required List<Token> Params { get; init; }
    public required List<IStmt> Body { get; init; }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}