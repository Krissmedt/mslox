using mslox.Service;
using Xunit.Abstractions;

namespace mslox.test;

public class AstPrinterTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public AstPrinterTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void ManualTest()
    {
        var sut = new AstPrinter();

        var expression = new Binary
        {
            Left = new Unary
            {
                Right = new Literal { Value = 123 },
                Operator = new Token
                {
                    Type = TokenType.Minus,
                    Lexeme = "-",
                    Line = 1
                }
            },
            Right = new Grouping
            {
                Expression = new Literal { Value = 45.67 }
            },
            Operator = new Token
            {
                Type = TokenType.Star,
                Lexeme = "*",
                Line = 1
            }
        };

        _testOutputHelper.WriteLine(sut.Print(expression));
    }
}