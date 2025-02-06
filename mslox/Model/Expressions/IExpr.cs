namespace mslox;

public interface IExpr
{
    T Accept<T>(IVisitor<T> visitor);
}