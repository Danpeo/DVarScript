using DVarScript.Interpreter.Tokens;

namespace DVarScript.Interpreter.Errors;

public class RuntimeError : Exception
{
    public Token Token { get; private set; }
    
    public RuntimeError(Token token, string message) : base(message)
    {
        Token = token;
    }
}