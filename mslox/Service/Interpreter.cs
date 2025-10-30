using System.ComponentModel;
using System.Data.Common;
namespace mslox;

using mslox.Expression;
using mslox.Statement;

class Interpreter : Expression.IVisitor<Object>, Statement.IVisitor<Boolean>
{
    private Environment environment = new Environment();

    public void Interpret(List<IStmt> program)
    {
        try
        {
            foreach (var stmt in program)
            {
                Execute(stmt);
            }
        }
        catch (RuntimeError error)
        {
            Lox.RuntimeError(error);
        }
    }

    public bool Visit(Var stmt)
    {
        Object value = null;

        if (stmt.Initializer != null)
        {
            value = Evaluate(stmt.Initializer);
        }

        environment.Define(stmt.Name.Lexeme, value);

        return true;
    }

    public bool Visit(ExpressionStmt stmt)
    {
        Evaluate(stmt.expression);
        return true;
    }

    public bool Visit(Print stmt)
    {
        var value = Evaluate(stmt.expression);
        Console.WriteLine(Stringify(value));
        return true;
    }

    
    public bool Visit(Block stmt)
    {
        ExecuteBlock(stmt.Statements, new Environment(environment));
        return true;
    }


    public object Visit(Assign expr)
    {
        var value = Evaluate(expr.Value);
        environment.Assign(expr.Name, expr.Value);

        return value;
    }

    public object Visit(Unary expr)
    {
        var right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.Minus:
                CheckNumberOperand(expr.Operator, right);
                return -(double)right;
            case TokenType.Bang:
                return !IsTruthy(right);
        }

        return null;
    }

    public object Visit(Binary expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.Minus:
                CheckNumberOperands(expr.Operator, left, right);
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
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left / (double)right;
            case TokenType.Star:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left * (double)right;
            case TokenType.Greater:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left > (double)right;
            case TokenType.GreaterEqual:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left >= (double)right;
            case TokenType.Less:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left < (double)right;
            case TokenType.LessEqual:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left <= (double)right;
            case TokenType.EqualEqual:
                return IsEqual(left, right);
            case TokenType.BangEqual:
                return !IsEqual(left, right);
        }

        return null;
    }

    public object Visit(Grouping expr)
    {
        return Evaluate(expr.Expression);
    }

    public object Visit(Literal expr)
    {
        return expr.Value;
    }

    public object Visit(Variable expr)
    {
        return environment.Get(expr.Name);
    }

    private void Execute(IStmt stmt)
    {
        stmt.Accept(this);
    }

    private void ExecuteBlock(List<IStmt> stmts, Environment environment)
    {
        var previous = this.environment;

        try
        {
            this.environment = environment;

            foreach (var statement in stmts)
            {
                Execute(statement);
            }
        } finally
        {
            this.environment = previous;
        }
    }

    private Object Evaluate(IExpr expr)
    {
        return expr.Accept(this);
    }

    private Boolean IsTruthy(Object value)
    {
        if (value == null) return false;
        if (value is Boolean) return (Boolean)value;
        return true;
    }

    private Boolean IsEqual(Object a, Object b)
    {
        if (a == null && b == null) return true;
        if (a == null) return false;

        return a.Equals(b);
    }

    private void CheckNumberOperand(Token op, Object operand)
    {
        if (operand is Double) return;
        throw new RuntimeError(op, "Operand must be a number");
    }

    private void CheckNumberOperands(Token op, Object left, Object right)
    {
        if (left is Double && right is Double) return;
        throw new RuntimeError(op, "Operands must be numbers");
    }

    private String Stringify(Object value)
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