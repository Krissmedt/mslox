using mslox.Expression;

namespace mslox.Statement;

public class While : IStmt
{
    public IExpr Condition;
    public IStmt Body;

    public While(IExpr condition, IStmt body)
    {
        Condition = condition;
        Body = body;
    }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}