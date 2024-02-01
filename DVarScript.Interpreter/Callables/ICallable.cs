namespace DVarScript.Interpreter.Callables;

public interface ICallable
{
    object? Call(Interpreter interpreter, List<object> args);
    int Arity();
}