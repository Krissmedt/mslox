namespace mslox.Statement;

public class ExpressionStmt : IStmt
{
    public Expression.IExpr expression;

    public ExpressionStmt(Expression.IExpr expr)
    {
        this.expression = expr;
    }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}