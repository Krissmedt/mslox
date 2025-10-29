namespace mslox.Expression;

public interface IVisitor<T>
{
    T Visit(Unary expr);
    T Visit(Binary expr);
    T Visit(Grouping expr);
    T Visit(Literal expr);
    T Visit(Variable expr);
    T Visit(Assign expr);
}