using DVarScript.Interpreter.Errors;
using DVarScript.Interpreter.Tokens;

namespace DVarScript.Interpreter.Env;

public class ProgramEnvironment
{
    public ProgramEnvironment? Enclosing { get; private set; }
    private readonly Dictionary<string, object?> _values = new();

    public ProgramEnvironment()
    {
        Enclosing = null;
    }

    public ProgramEnvironment(ProgramEnvironment enclosing)
    {
        Enclosing = enclosing;
    }

    public void DefineVariable(Token name, object? value)
    {
        if (!_values.ContainsKey(name.Lexeme))
        {
            Define(name.Lexeme, value);
            return;
        }

        throw new RuntimeError(name, $"Variable '{name.Lexeme}' is already defined.");
    }

    public void Define(string name, object? value) => 
        _values[name] = value;

    public void Assign(Token name, object value)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            _values[name.Lexeme] = value;
            return;
        }

        if (Enclosing != null)
        {
            Enclosing.Assign(name, value);
            return;
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public object? Get(Token name)
    {
        if (_values.TryGetValue(name.Lexeme, out var value))
            return value;

        if (Enclosing != null)
            return Enclosing.Get(name);

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }
}