namespace mslox;

using System.Data;
using System.Reflection.Metadata;
using mslox.Expression;
using mslox.Statement;

public class Interpreter : Expression.IVisitor<Object>, Statement.IVisitor<Boolean>
{
    public Environment Globals { get; } = new Environment();
    private Environment environment;
    private Dictionary<IExpr, int> locals = new();

    public Interpreter()
    {
        Globals.Define("clock", new Clock());
        this.environment = Globals;
    }

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

    public bool Visit(While stmt)
    {
        while (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.Body);
        }

        return true;
    }

    public bool Visit(If stmt)
    {
        if (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.IfBranch);
        }
        else if (stmt.ElseBranch != null)
        {
            Execute(stmt.ElseBranch);
        }

        return true;
    }

    public bool Visit(ExpressionStmt stmt)
    {
        Evaluate(stmt.expression);
        return true;
    }

    public bool Visit(Function stmt)
    {
        var function = new LoxFunction(stmt, environment);
        environment.Define(stmt.Name.Lexeme, function);

        return true;
    }

    public bool Visit(Print stmt)
    {
        var value = Evaluate(stmt.expression);
        Console.WriteLine(Stringify(value));
        return true;
    }

    public bool Visit(Return stmt)
    {
        object value = null;
        if (stmt.Value != null) value = Evaluate(stmt.Value);

        throw new ReturnUnwind(value);
    }


    public bool Visit(Block stmt)
    {
        ExecuteBlock(stmt.Statements, new Environment(environment));

        return true;
    }

    public bool Visit(ClassStmt stmt)
    {
        environment.Define(stmt.name.Lexeme, null);

        Dictionary<String, LoxFunction> methods = [];
        foreach(var method in stmt.methods)
        {
            var function = new LoxFunction(method, environment);
            methods.Add(method.Name.Lexeme, function);
        }

        LoxClass klass = new LoxClass(stmt.name.Lexeme, methods);
        environment.Assign(stmt.name, klass);

        return true;
    }

    public object Visit(Assign expr)
    {
        var value = Evaluate(expr.Value);
        environment.Assign(expr.Name, value);

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

    public object Visit(Call expr)
    {
        var callee = Evaluate(expr.Callee);

        List<Object> arguments = new();
        foreach (var argument in expr.Arguments)
        {
            arguments.Add(Evaluate(argument));
        }

        if (!(callee is LoxCallable))
        {
            throw new RuntimeError(expr.Paren, "Can only call functions and classes");
        }

        var function = (LoxCallable)callee;

        if (arguments.Count != function.Arity())
        {
            throw new RuntimeError(expr.Paren, $"Expected {function.Arity()} arguments but got {arguments.Count}.");
        }

        return function.Call(this, arguments);
    }

    public object Visit(Get expr)
    {
        var obj = Evaluate(expr.Object);
        if (obj is LoxInstance)
        {
            return ((LoxInstance)obj).Get(expr.Name);
        }

        throw new RuntimeError(expr.Name, "Only instances have properties.");
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
        return LookupVariable(expr.Name, expr);
    }

    public object Visit(Logical expr)
    {
        var left = Evaluate(expr.Left);

        if (expr.Operator.Type == TokenType.Or)
        {
            if (IsTruthy(left)) return left;
        }
        else
        {
            if (!IsTruthy(left)) return left;
        }

        return Evaluate(expr.Right);
    }

    public object Visit(Set expr)
    {
        var obj = Evaluate(expr.Object);

        if (!(obj is LoxInstance))
        {
            throw new RuntimeError(expr.Name, "Only instances have fields.");
        }

        var value = Evaluate(expr.Value);
        ((LoxInstance)obj).Set(expr.Name, value);

        return value;
    }

    public object Visit(This expr)
    {
        return LookupVariable(expr.keyword, expr);
    }

    public void Execute(IStmt stmt)
    {
        stmt.Accept(this);
    }

    public void ExecuteBlock(List<IStmt> stmts, Environment environment)
    {
        var previous = this.environment;

        try
        {
            this.environment = environment;

            foreach (var statement in stmts)
            {
                Execute(statement);
            }
        }
        finally
        {
            this.environment = previous;
        }
    }

    public void Resolve(IExpr expr, int depth)
    {
        locals.Add(expr, depth);
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

    private Object LookupVariable(Token name, IExpr expr)
    {
        if (locals.ContainsKey(expr))
        {
            return environment.GetAt(locals[expr], name);
        } else
        {
            return Globals.Get(name);
        }
    }
}