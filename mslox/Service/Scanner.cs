namespace mslox.Service;

public class Scanner
{
    private String Source { get; init; }
    private List<Token> Tokens { get; } = new();

    private Int64 _start = 0;
    private Int64 _current = 0;
    private Int64 _line = 0;
    
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
        throw new NotImplementedException();
    }

    private Boolean IsAtEnd()
    {
        return _current >= Source.Length;
    }
}