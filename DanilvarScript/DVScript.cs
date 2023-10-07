using DanilvarScript;
using DanilvarScript.Tokens;

namespace DanilvarScript;

public static class DVScript
{
    private static bool HadError;

    public static void RunFile(string path)
    {
        try
        {
            byte[] bytes = File.ReadAllBytes(path);
            Run(System.Text.Encoding.Default.GetString(bytes));
            
            if (HadError)
                Environment.Exit(65);
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
        IEnumerable<Token> tokens = scanner.ScanTokens();
    
        foreach (Token token in tokens)
        {
            Console.WriteLine(token);
        }
    }
    
    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }
    
    public static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        HadError = true;
    }
}