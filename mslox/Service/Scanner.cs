namespace mslox.Service;

public class Scanner
{
    public required String Source { get; init; }
    private List<Token> Tokens { get; } = new();

    private Int32 _start = 0;
    private Int32 _current = 0;
    private Int32 _line = 0;
    
    public List<Token> Scan()
    {
        while ((!IsAtEnd()))
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
        char c = Advance();
        switch (c) {
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
        }
    }

    private Boolean IsAtEnd()
    {
        return _current >= Source.Length;
    }

    private Char Advance()
    {
        return Source.ToCharArray()[_current++];
    }

    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, Object value)
    {
        var text = Source.Substring(_start, _current);
        Tokens.Add(new Token
        {
            Type = type,
            Lexeme = text,
            Line = _line,
            Literal = value
        });
    }
}