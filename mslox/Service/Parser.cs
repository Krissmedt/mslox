using System.Runtime.InteropServices.JavaScript;

namespace mslox.Service;

public class Parser
{
    private class ParseError : Exception { }
    public List<Token> Tokens { get; init; } = new();
    private int _current = 0;

    public IExpr Parse() {
        try {
            return Expression();
        }
        catch (ParseError) 
        {
            return null;
        }
    }

    private IExpr Expression()
    {
        return Equality();
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
            if(Previous().Type == TokenType.Semicolon) return;

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