namespace mslox;

public class Token
{
    public required TokenType Type { get; init; }
    public required string Lexeme { get; init; }
    public object Literal { get; init; }
    public required long Line { get; init; }

    public override string ToString()
    {
        return $"{Type} {Lexeme} {Literal}";
    }
}