namespace mslox.Expression;

public interface IVisitor<T>
{
    T Visit(Unary expr);
    T Visit(Call expr);
    T Visit(Get expr);
    T Visit(Binary expr);
    T Visit(Grouping expr);
    T Visit(Literal expr);
    T Visit(Variable expr);
    T Visit(Assign expr);
    T Visit(Logical expr);
    T Visit(Set expr);
    T Visit(This expr);
    T Visit(Super expr);
}