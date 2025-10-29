namespace mslox.Statement;

public interface IVisitor<T>
{
    // T Visit(Block stmt);
    // T Visit(Class stmt);
    T Visit(ExpressionStmt stmt);
    // T Visit(Function stmt);
    // T Visit(If stmt);
    T Visit(Print stmt);
    // T Visit(Return stmt);
    T Visit(Var stmt);
    // T Visit(While stmt);
}