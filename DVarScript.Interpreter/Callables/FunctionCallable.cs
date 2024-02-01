using DVarScript.Interpreter.Env;
using DVarScript.Interpreter.Errors;
using DVarScript.Interpreter.Stmt;

namespace DVarScript.Interpreter.Callables;

public class FunctionCallable : ICallable
{
    private readonly Function _declaration;
    private readonly ProgramEnvironment _closure;
    
    public FunctionCallable(Function declaration, ProgramEnvironment closure)
    {
        _declaration = declaration;
        _closure = closure;
    }
    
    public object? Call(Interpreter interpreter, List<object> args)
    {
        var env = new ProgramEnvironment(_closure);

        for (int i = 0; i < _declaration.Params.Count; i++)
        {
            env.Define(_declaration.Params[i].Lexeme, args[i]);
        }

        try
        {
            interpreter.ExecuteBlock(_declaration.Body, env);
        }
        catch (ReturnE ret)
        {
            return ret.Value;
        }
        
        return null;
    }

    public int Arity() => 
        _declaration.Params.Count;

    public override string ToString() => 
        $"<func {_declaration.Name.Lexeme}>";
}