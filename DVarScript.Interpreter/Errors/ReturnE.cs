namespace DVarScript.Interpreter.Errors;

public class ReturnE : Exception
{
    public object? Value { get; }

    public ReturnE(object? value)
    {
        Value = value;
    }
}