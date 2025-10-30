namespace mslox.Statement;
using mslox;

public class If : IStmt
{
    public Expression.IExpr Condition;
    public IStmt IfBranch;
    public IStmt? ElseBranch;

    public If(Expression.IExpr condition, IStmt ifBranch, IStmt? elseBranch)
    {
        this.Condition = condition;
        this.IfBranch = ifBranch;
        this.ElseBranch = elseBranch;
    }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}