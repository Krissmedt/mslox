using System.ComponentModel;
using System.Data.Common;
using mslox;

class Interpreter : IVisitor<Object>
{
    public void Interpret(IExpr expression)
    {
        try
        {
            var value = evaluate(expression);
            Console.WriteLine(stringify(value));
        }
        catch (RuntimeError error)
        {
            Lox.RuntimeError(error);
        }
    }

    public object Visit(Unary expr)
    {
        var right = evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.Minus:
                checkNumberOperand(expr.Operator, right);
                return -(double)right;
            case TokenType.Bang:
                return !isTruthy(right);
        }

        return null;
    }

    public object Visit(Binary expr)
    {
        var left = evaluate(expr.Left);
        var right = evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.Minus:
                checkNumberOperands(expr.Operator, left, right);
                return (double)left - (double)right;
            case TokenType.Plus:
                if (left is Double && right is Double)
                {
                    return (double)left + (double)right;
                }

                if (left is String && right is String)
                {
                    return (String)left + (String)right;
                }

                throw new RuntimeError(expr.Operator, "Operands must be two numbers or two strings");
            case TokenType.Slash:
                checkNumberOperands(expr.Operator, left, right);
                return (double)left / (double)right;
            case TokenType.Star:
                checkNumberOperands(expr.Operator, left, right);
                return (double)left * (double)right;
            case TokenType.Greater:
                checkNumberOperands(expr.Operator, left, right);
                return (double)left > (double)right;
            case TokenType.GreaterEqual:
                checkNumberOperands(expr.Operator, left, right);
                return (double)left >= (double)right;
            case TokenType.Less:
                checkNumberOperands(expr.Operator, left, right);
                return (double)left < (double)right;
            case TokenType.LessEqual:
                checkNumberOperands(expr.Operator, left, right);
                return (double)left <= (double)right;
            case TokenType.EqualEqual:
                return isEqual(left, right);
            case TokenType.BangEqual:
                return !isEqual(left, right);
        }

        return null;
    }

    public object Visit(Grouping expr)
    {
        return evaluate(expr.Expression);
    }

    public object Visit(Literal expr)
    {
        return expr.Value;
    }

    private Object evaluate(IExpr expr)
    {
        return expr.Accept(this);
    }

    private Boolean isTruthy(Object value)
    {
        if (value == null) return false;
        if (value is Boolean) return (Boolean)value;
        return true;
    }

    private Boolean isEqual(Object a, Object b)
    {
        if (a == null && b == null) return true;
        if (a == null) return false;

        return a.Equals(b);
    }

    private void checkNumberOperand(Token op, Object operand)
    {
        if (operand is Double) return;
        throw new RuntimeError(op, "Operand must be a number");
    }

    private void checkNumberOperands(Token op, Object left, Object right)
    {
        if (left is Double && right is Double) return;
        throw new RuntimeError(op, "Operands must be numbers");
    }

    private String stringify(Object value)
    {
        if (value == null) return null;

        if (value is Double)
        {
            if (Double.IsInfinity((Double)value))
            {
                return "NaN";
            }
            
            String text = value.ToString();
            if (text.EndsWith(".0"))
            {
                text = text.Substring(0, text.Length - 2);
            }
            return text;
        }

        

        return value.ToString();
    }
}