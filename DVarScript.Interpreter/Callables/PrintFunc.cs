namespace DVarScript.Interpreter.Callables;

public class PrintFunc : ICallable
{
    private readonly bool _printLine;

    public PrintFunc(bool printLine)
    {
        _printLine = printLine;
    }

    public object? Call(Interpreter interpreter, List<object> args)
    {
        string result = "";

        foreach (object arg in args)
        {
            result += interpreter.Stringify(arg) + " ";
        }

        if (_printLine)
            Console.WriteLine(result);
        else
            Console.Write(result);
        
        return null;
    }

    public int Arity() => -1;
}