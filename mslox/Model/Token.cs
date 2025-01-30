namespace mslox;

public class Token
{
    public required TokenType Type { get; init; }
    public required String Lexeme { get; init; }
    public Object Literal { get; init; }
    public required Int64 Line { get; init; }
    
    public override String ToString() => $"{Type} {Lexeme} {Literal}";
}