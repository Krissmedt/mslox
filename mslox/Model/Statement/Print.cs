namespace mslox.Statement;

public class Print : IStmt
{
    public Expression.IExpr expression;

    public Print(Expression.IExpr value)
    {
        this.expression = value;
    }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}