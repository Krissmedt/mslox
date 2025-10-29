namespace mslox.Expression;

public interface IExpr
{
    T Accept<T>(IVisitor<T> visitor);
}