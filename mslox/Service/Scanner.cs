namespace mslox.Service;

public class Scanner
{
    public required String Source { get; init; }

    private char[] SourceArray => Source.ToCharArray();
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
                if (Match('/')) {
                    // A comment goes until the end of the line.
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                } else {
                    AddToken(TokenType.Slash);
                }
                break;
            
            // Ignore whitespace
            case ' ':
            case '\r':
            case '\t':
                break;
            case '\n':
                _line++;
                break;
            default: Lox.Error(_line, $"Unexpected character '{c}'."); break;
            
            // Literals
            case '"': StringParse(); break;
        }
    }

    private void StringParse()
    {
        while (Peek() != '"' && !IsAtEnd()) {
            if (Peek() == '\n') _line++;
            Advance();
        }

        if (IsAtEnd()) {
            Lox.Error(_line, "Unterminated string.");
            return;
        }

        // Advance past the closing ".
        Advance();

        // Trim the surrounding quotes.
        String value = Source.Substring(_start + 1, _current - 1);
        AddToken(TokenType.String, value);
    }

    private char Peek() {
        if (IsAtEnd()) return '\0';
        return SourceArray[_current];
    }
    
    private Boolean Match(char expected) {
        if (IsAtEnd()) return false;
        if (SourceArray[_current] != expected) return false;

        _current++;
        return true;
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
    

    private Boolean IsAtEnd()
    {
        return _current >= Source.Length;
    }

    private Char Advance()
    {
        return Source.ToCharArray()[_current++];
    }


}