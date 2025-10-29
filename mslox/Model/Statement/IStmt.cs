namespace mslox.Statement;

public interface IStmt
{
    T Accept<T>(IVisitor<T> visitor);
}