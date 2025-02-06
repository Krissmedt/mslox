namespace mslox.Service;

public class AstPrinter : IVisitor<string>
{
    public string Visit(Unary expr)
    {
        return Parenthesize(expr.Operator.Lexeme, [expr.Right]);
    }

    public string Visit(Binary expr)
    {
        return Parenthesize(expr.Operator.Lexeme, [expr.Left, expr.Right]);
    }

    public string Visit(Grouping expr)
    {
        return Parenthesize("group", [expr.Expression]);
    }

    public string Visit(Literal expr)
    {
        return expr.Value == null ? "nil" : expr.Value.ToString();
    }

    public string Print(IExpr expr)
    {
        return expr.Accept(this);
    }

    private string Parenthesize(string name, List<IExpr> exprs)
    {
        var expressionString = "";
        foreach (var expr in exprs) expressionString += $" {expr.Accept(this)}";

        return $"({name}{expressionString})";
    }
}