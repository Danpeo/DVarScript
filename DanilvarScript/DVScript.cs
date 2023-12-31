using DanilvarScript.Errors;
using DanilvarScript.Expr;
using DanilvarScript.Tokens;
using DanilvarScript.Visitor;

namespace DanilvarScript;

public static class DVScript
{
    private static readonly Interpreter Interpreter = new();
    private static bool HadError;
    private static bool HadRuntimeError;

    public static void RunFile(string path)
    {
        try
        {
            byte[] bytes = File.ReadAllBytes(path);
            Run(System.Text.Encoding.Default.GetString(bytes));
            
            if (HadError)
                Environment.Exit(65);
            
            if (HadRuntimeError)
                Environment.Exit(70);
        }
        catch (IOException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public static void RunPrompt()
    {
        Console.WriteLine("> ");
    
        while (true)
        {
            string? line = Console.ReadLine();
    
            if (line == null)
                break;
            
            Run(line);
            
            HadError = false;
            
            Console.WriteLine("> ");
        }
    }
    
    public static void Run(string source)
    {
        var scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();
    
        /*foreach (Token token in tokens)
        {
            Console.WriteLine(token);
        }*/

        var parser = new Parser(tokens);
        Expression? expression = parser.Parse();
        
        if (HadError)
            return;

        /*if (expression != null) 
            Console.WriteLine(new AbstartTreePrinter().Print(expression));*/
        if (expression != null) 
            Interpreter.Interpret(expression);
    }
    
    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void RuntimeError(RuntimeError error)
    {
        Console.Error.WriteLine($"{error.Message}\n[line {error.Token.Line}]");
        HadRuntimeError = true;
    }
    
    public static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        HadError = true;
    }

    public static void Error(Token token, string message)
    {
        Report(token.Line, token.Type == TokenType.Eof ? " at end" : $" at '{token.Lexeme}'", message);
    }
}