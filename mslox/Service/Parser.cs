﻿using System.Data.Common;
using System.Runtime.InteropServices.JavaScript;
using mslox.Expression;
using mslox.Statement;

namespace mslox.Service;

public class Parser
{
    private class ParseError : Exception { }
    public List<Token> Tokens { get; init; } = new();
    private int _current = 0;

    public List<IStmt> Parse()
    {
        List<IStmt> statements = new();
        while (!IsAtEnd())
        {
            statements.Add(Declaration());
        }
        return statements;
    }

    private IStmt Declaration()
    {
        try
        {
            if (Match(TokenType.Var)) return VarDeclaration();

            return Statement();
        }
        catch (ParseError error)
        {
            Synchronize();
            return null;
        }
    }

    private IStmt VarDeclaration()
    {
        var name = Consume(TokenType.Identifier, "Expect variable name.");

        IExpr initializer = null;
        if (Match(TokenType.Equal))
        {
            initializer = Expression();
        }

        Consume(TokenType.Semicolon, "Expect ';' after variable declaration.");
        return new Var(name, initializer);
    }

    private IStmt Statement()
    {
        if (Match(TokenType.If)) return IfStatement();
        if (Match(TokenType.Print)) return PrintStatement();
        if (Match(TokenType.While)) return WhileStatement();
        if (Match(TokenType.LeftBrace)) return new Block(BlockStatement());

        return ExpressionStatement();
    }

    private IStmt IfStatement()
    {
        Consume(TokenType.LeftParen, "Expect '(' after 'if'.");
        var condition = Expression();
        Consume(TokenType.RightParen, "Expect ')' after if condition.");

        IStmt thenBranch = Statement();
        IStmt? elseBranch = null;
        if (Match(TokenType.Else))
        {
            elseBranch = Statement();
        }

        return new If(condition, thenBranch, elseBranch);
    }

    private IStmt PrintStatement()
    {
        var value = Expression();
        Consume(TokenType.Semicolon, "Expect ';' after value.");
        return new Print(value);
    }

    private IStmt WhileStatement()
    {
        Consume(TokenType.LeftParen, "Expect '(' after 'while'.");
        var condition = Expression();
        Consume(TokenType.RightParen, "Expect ')' after condition.");
        var body = Statement();

        return new While(condition, body);
    }

    private List<IStmt> BlockStatement()
    {
        List<IStmt> statements = new();

        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            statements.Add(Declaration());
        }

        Consume(TokenType.RightBrace, "Expect '}' after block.");
        return statements;
    }

    private IStmt ExpressionStatement()
    {
        var expr = Expression();
        Consume(TokenType.Semicolon, "Expect ';' after expression.");
        return new ExpressionStmt(expr);
    }

    private IExpr Expression()
    {
        return Assignment();
    }

    private IExpr Assignment()
    {
        var expr = Or();

        if (Match(TokenType.Equal))
        {
            var equals = Previous();
            var value = Assignment();

            if (expr is Variable)
            {
                var name = ((Variable)expr).Name;
                return new Assign(name, value);
            }

            Error(equals, "Invalid assignment target.");
        }

        return expr;
    }

    private IExpr Or()
    {
        var expr = And();

        while (Match(TokenType.Or))
        {
            var oper = Previous();
            var right = And();
            expr = new Logical(expr, oper, right);
        }

        return expr;
    }

    private IExpr And()
    {
        var expr = Equality();

        while (Match(TokenType.And))
        {
            var oper = Previous();
            var right = Equality();
            expr = new Logical(expr, oper, right);
        }

        return expr;
    }

    private IExpr Equality()
    {
        var expr = Comparison();

        while (Match(TokenType.BangEqual, TokenType.EqualEqual))
        {
            var operat = Previous();
            var right = Comparison();
            expr = new Binary
            {
                Left = expr,
                Right = right,
                Operator = operat
            };
        }

        return expr;
    }

    private IExpr Comparison()
    {
        var expr = Term();

        while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
        {
            var operat = Previous();
            var right = Term();
            expr = new Binary
            {
                Left = expr,
                Right = right,
                Operator = operat
            };
        }

        return expr;
    }

    private IExpr Term()
    {
        var expr = Factor();

        while (Match(TokenType.Minus, TokenType.Plus))
        {
            var operat = Previous();
            var right = Factor();
            expr = new Binary
            {
                Left = expr,
                Right = right,
                Operator = operat
            };
        }

        return expr;
    }

    private IExpr Factor()
    {
        var expr = Unary();

        while (Match(TokenType.Slash, TokenType.Star))
        {
            var operat = Previous();
            var right = Unary();
            expr = new Binary
            {
                Left = expr,
                Right = right,
                Operator = operat
            };
        }

        return expr;
    }

    private IExpr Unary()
    {
        if (Match(TokenType.Bang, TokenType.Minus))
        {
            var operat = Previous();
            var right = Unary();
            return new Unary
            {
                Right = right,
                Operator = operat
            };
        }

        return Primary();
    }

    private IExpr Primary()
    {
        if (Match(TokenType.False)) return new Literal { Value = false };
        if (Match(TokenType.True)) return new Literal { Value = true };
        if (Match(TokenType.Nil)) return new Literal { Value = null };

        if (Match(TokenType.Number, TokenType.String))
        {
            return new Literal { Value = Previous().Literal };
        }

        if (Match(TokenType.Identifier))
        {
            return new Variable(Previous());
        }

        if (Match(TokenType.LeftParen))
        {
            var expr = Expression();
            Consume(TokenType.RightParen, "Expect ')' after expression.");
            return new Grouping { Expression = expr };
        }

        throw Error(Peek(), "Expect expression.");
    }

    private Token Consume(TokenType type, String message)
    {
        if (Check(type)) return Advance();

        throw Error(Peek(), message);
    }

    private ParseError Error(Token token, String message)
    {
        Lox.Error(token, message);
        return new ParseError();
    }

    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == TokenType.Semicolon) return;

            switch (Peek().Type)
            {
                case TokenType.Class:
                case TokenType.Fun:
                case TokenType.Var:
                case TokenType.For:
                case TokenType.If:
                case TokenType.While:
                case TokenType.Print:
                case TokenType.Return:
                    return;
            }

            Advance();
        }
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    private bool Check(TokenType type)
    {
        return !IsAtEnd() && Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) _current++;
        return Previous();
    }

    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.Eof;
    }

    private Token Peek()
    {
        return Tokens[_current];
    }

    private Token Previous()
    {
        return Tokens[_current - 1];
    }
}