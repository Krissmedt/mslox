using mslox.Expression;
using mslox.Statement;

namespace mslox;

public class Resolver : Expression.IVisitor<Boolean>, Statement.IVisitor<Boolean>
{
    private Interpreter interpreter;
    private Stack<Dictionary<String, Boolean>> scopes = new();
    private FunctionType currentFunction = FunctionType.NONE;

    #region Expression Visitor
    public Resolver(Interpreter interpreter)
    {
        this.interpreter = interpreter;
    }

    public bool Visit(Unary expr)
    {
        Resolve(expr.Right);
        return true;
    }

    public bool Visit(Call expr)
    {
        Resolve(expr.Callee);

        foreach (var argument in expr.Arguments)
        {
            Resolve(argument);
        }

        return true;
    }

    public bool Visit(Get expr)
    {
        Resolve(expr.Object);
        return true;
    }

    public bool Visit(Set expr)
    {
        Resolve(expr.Value);
        Resolve(expr.Object);
        return true;
    }

    public bool Visit(This expr)
    {
        ResolveLocal(expr, expr.keyword);
        return true;
    }

    public bool Visit(Binary expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return true;
    }

    public bool Visit(Grouping expr)
    {
        Resolve(expr.Expression);
        return true;
    }

    public bool Visit(Literal expr)
    {
        return true;
    }

    public bool Visit(Variable expr)
    {
        if (scopes.Count != 0 && (scopes.Peek().TryGetValue(expr.Name.Lexeme, out var resolved) ? !resolved : false))
        {
            Lox.Error(expr.Name, "Can't read local variable in it's own initializer.");
        }

        ResolveLocal(expr, expr.Name);
        return true;
    }

    public bool Visit(Assign expr)
    {
        Resolve(expr.Value);
        ResolveLocal(expr, expr.Name);
        return true;
    }

    public bool Visit(Logical expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return true;
    }
    #endregion

    #region Statement Visitor
    public bool Visit(Block stmt)
    {
        BeginScope();
        Resolve(stmt.Statements);
        EndScope();

        return true;
    }

    public bool Visit(ClassStmt stmt)
    {
        Declare(stmt.name);
        Define(stmt.name);

        BeginScope();
        scopes.Peek().Add("this", true);

        foreach (var method in stmt.methods)
        {
            var declaration = FunctionType.METHOD;
            ResolveFunction(method, declaration);
        }

        EndScope();

        return true;
    }

    public bool Visit(ExpressionStmt stmt)
    {
        Resolve(stmt.expression);
        return true;
    }

    public bool Visit(Function stmt)
    {
        Declare(stmt.Name);
        Define(stmt.Name);

        ResolveFunction(stmt, FunctionType.FUNCTION);
        return true;
    }

    public bool Visit(If stmt)
    {
        Resolve(stmt.Condition);
        Resolve(stmt.IfBranch);
        if (stmt.ElseBranch != null) Resolve(stmt.ElseBranch);
        return true;
    }

    public bool Visit(Print stmt)
    {
        Resolve(stmt.expression);
        return true;
    }

    public bool Visit(Return stmt)
    {
        if (currentFunction == FunctionType.NONE)
        {
            Lox.Error(stmt.Keyword, "Can't return from top-level code.");
        }

        if (stmt.Value != null)
        {
            Resolve(stmt.Value);
        }
        return true;
    }

    public bool Visit(Var stmt)
    {
        Declare(stmt.Name);
        if (stmt.Initializer != null)
        {
            Resolve(stmt.Initializer);
        }
        Define(stmt.Name);

        return true;
    }

    public bool Visit(While stmt)
    {
        Resolve(stmt.Condition);
        Resolve(stmt.Body);
        return true;
    }
    #endregion

    private void BeginScope()
    {
        scopes.Push([]);
    }

    private void EndScope()
    {
        scopes.Pop();
    }

    public void Resolve(List<IStmt> statements)
    {
        foreach (var stmt in statements)
        {
            Resolve(stmt);
        }
    }

    private void Resolve(IStmt statement)
    {
        statement.Accept(this);
    }

    private void Resolve(IExpr expr)
    {
        expr.Accept(this);
    }

    private void ResolveLocal(IExpr expr, Token name)
    {
        for (var i = 0; i < scopes.Count; i++)
        {
            if (scopes.ElementAt(i).ContainsKey(name.Lexeme))
            {
                interpreter.Resolve(expr, i);
                return;
            }
        }
    }

    private void ResolveFunction(Function function, FunctionType functionType)
    {
        var enclosingFunction = currentFunction;
        currentFunction = functionType;

        BeginScope();
        foreach (var param in function.Params)
        {
            Declare(param);
            Define(param);
        }
        Resolve(function.Body);
        EndScope();
    }

    private void Declare(Token name)
    {
        if (scopes.Count == 0) return;

        var scope = scopes.Peek();
        if (scope.ContainsKey(name.Lexeme))
        {
            Lox.Error(name, "Already a variable with this name in this scope.");
        }
        scope[name.Lexeme] = false;
    }

    private void Define(Token name)
    {
        if (scopes.Count == 0) return;

        var scope = scopes.Peek();
        scope[name.Lexeme] = true;
    }
}