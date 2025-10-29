using mslox;
using mslox.Expression;

public class Variable : IExpr
{
    public Token Name;

    public Variable(Token name)
    {
        this.Name = name;
    }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}