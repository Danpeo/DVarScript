using DVarScript.Interpreter.Errors;
using DVarScript.Interpreter.Tokens;

namespace DVarScript.Interpreter.Env;

public class VariableEnvironment
{
    public VariableEnvironment? Enclosing { get; private set; }
    private readonly Dictionary<string, object?> _values = new();

    public VariableEnvironment()
    {
        Enclosing = null;
    }

    public VariableEnvironment(VariableEnvironment enclosing)
    {
        Enclosing = enclosing;
    }
    
    public void Define(Token name, object? value)
    {
        if (!_values.ContainsKey(name.Lexeme))
        {
            _values[name.Lexeme] = value;
            return;            
        }
        
        throw new RuntimeError(name, $"Variable '{name.Lexeme}' is already defined.");
    }

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