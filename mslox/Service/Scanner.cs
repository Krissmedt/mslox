using System.Collections.Immutable;

namespace mslox.Service;

public class Scanner
{
    private int _current;
    private int _line;

    private int _start;
    public required string Source { get; init; }

    private char[] SourceArray => Source.ToCharArray();
    private List<Token> Tokens { get; } = new();

    private static ImmutableDictionary<string, TokenType> Keywords { get; } = ImmutableDictionary.CreateRange(
        [
            KeyValuePair.Create("and", TokenType.And),
            KeyValuePair.Create("class", TokenType.Class),
            KeyValuePair.Create("else", TokenType.Else),
            KeyValuePair.Create("false", TokenType.False),
            KeyValuePair.Create("for", TokenType.For),
            KeyValuePair.Create("fun", TokenType.Fun),
            KeyValuePair.Create("if", TokenType.If),
            KeyValuePair.Create("nil", TokenType.Nil),
            KeyValuePair.Create("or", TokenType.Or),
            KeyValuePair.Create("print", TokenType.Print),
            KeyValuePair.Create("return", TokenType.Return),
            KeyValuePair.Create("super", TokenType.Super),
            KeyValuePair.Create("this", TokenType.This),
            KeyValuePair.Create("true", TokenType.True),
            KeyValuePair.Create("var", TokenType.Var),
            KeyValuePair.Create("while", TokenType.While)
        ]
    );

    public List<Token> Scan()
    {
        while (!IsAtEnd())
        {
            _start = _current;
            ScanToken();
        }

        Tokens.Add(new Token
        {
            Type = TokenType.Eof,
            Lexeme = "",
            Line = _line
        });

        return Tokens;
    }

    private void ScanToken()
    {
        var c = Advance();
        switch (c)
        {
            // Single-character tokens
            case '(': AddToken(TokenType.LeftParen); break;
            case ')': AddToken(TokenType.RightParen); break;
            case '{': AddToken(TokenType.LeftBrace); break;
            case '}': AddToken(TokenType.RightBrace); break;
            case ',': AddToken(TokenType.Comma); break;
            case '.': AddToken(TokenType.Dot); break;
            case '-': AddToken(TokenType.Minus); break;
            case '+': AddToken(TokenType.Plus); break;
            case ';': AddToken(TokenType.Semicolon); break;
            case '*': AddToken(TokenType.Star); break;

            // Two-character tokens
            case '!':
                AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
                break;

            // Division/comment special case
            case '/':
                if (Match('/'))
                    // A comment goes until the end of the line.
                    while (Peek() != '\n' && !IsAtEnd())
                        Advance();
                else
                    AddToken(TokenType.Slash);
                break;

            // Ignore whitespace
            case ' ':
            case '\r':
            case '\t':
                break;
            case '\n':
                _line++;
                break;

            // Literals
            case '"': StringParse(); break;

            default:
                if (IsDigit(c))
                    Number();
                else if (IsAlpha(c))
                    Identifier();
                else
                    Lox.Error(_line, $"Unexpected character '{c}'.");
                break;
        }
    }

    private void Identifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();

        var text = Source.Substring(_start, _current - _start);
        var type = Keywords.GetValueOrDefault(text, TokenType.Identifier);

        AddToken(type);
    }

    private bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z') ||
               c == '_';
    }

    private bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }


    private void Number()
    {
        while (IsDigit(Peek())) Advance();

        // Look for fractional part
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();

            while (IsDigit(Peek())) Advance();
        }

        AddToken(TokenType.Number, double.Parse(Source.Substring(_start, _current - _start)));
    }

    private bool IsDigit(char c)
    {
        return c is >= '0' and <= '9';
    }

    private void StringParse()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') _line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(_line, "Unterminated string.");
            return;
        }

        // Advance past the closing ".
        Advance();

        // Trim the surrounding quotes.
        var value = Source.Substring(_start + 1, _current - _start - 2);
        AddToken(TokenType.String, value);
    }

    private char Peek()
    {
        if (IsAtEnd()) return '\0';
        return SourceArray[_current];
    }

    private char PeekNext()
    {
        if (_current + 1 > Source.Length) return '\0';
        return SourceArray[_current + 1];
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (SourceArray[_current] != expected) return false;

        _current++;
        return true;
    }

    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, object value)
    {
        var text = Source.Substring(_start, _current - _start);
        Tokens.Add(new Token
        {
            Type = type,
            Lexeme = text,
            Line = _line,
            Literal = value
        });
    }

    private bool IsAtEnd()
    {
        return _current >= Source.Length;
    }

    private char Advance()
    {
        return Source.ToCharArray()[_current++];
    }
}