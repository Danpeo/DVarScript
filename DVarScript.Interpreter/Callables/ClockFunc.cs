namespace DVarScript.Interpreter.Callables;

public class ClockFunc : ICallable
{
    public object? Call(Interpreter interpreter, List<object> args) => 
        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;

    public override string ToString() => 
        "<native func>";

    public int Arity() => 0;
}