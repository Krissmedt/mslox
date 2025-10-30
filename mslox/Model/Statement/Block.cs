using mslox.Statement;

public class Block : IStmt
{
    public List<IStmt> Statements;

    public Block(List<IStmt> stmts)
    {
        this.Statements = stmts;
    }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}