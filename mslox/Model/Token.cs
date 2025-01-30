namespace mslox;

public class Token
{
    private TokenType type { get; init; }
    private String lexeme { get; init; }
    private Object literal { get; init; }
    private Int64 line { get; init; }
    
    public new String ToString() => $"{type} {lexeme} {literal}";
}