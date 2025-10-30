namespace mslox.Expression;

public class Logical : IExpr
{
    public IExpr Left;
    public Token Operator;
    public IExpr Right;

    public Logical(IExpr left, Token oper, IExpr right)
    {
        Left = left;
        Operator = oper;
        Right = right;
    }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}