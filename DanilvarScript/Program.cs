//TODO: use Cocona later!
if (args.Length > 1)
{
    Console.WriteLine("Usage: Danilvar [script]");
    Environment.Exit(64);
}
else if (args.Length == 1)
{
    DVScript.RunFile(args[0]);
}
else
{
    DVScript.RunPrompt();
}


/*using DanilvarScript.Expr;
using DanilvarScript.Tokens;
using DanilvarScript.Visitor;

Expression expression = new Binary(
    new Unary(
        new Token(TokenType.Minus, "-", null, 1),
        new Literal(123)),
    new Token(TokenType.Star, "*", null, 1),
    new Grouping(
        new Literal(45.67)));

Console.WriteLine(new AstPrinter().Print(expression));*/