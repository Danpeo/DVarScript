using DVarScript.Interpreter.Tokens;
using DVarScript.Interpreter.Visitor;

namespace DVarScript.Interpreter.Stmt;

public class Function : Statement
{
    public Token Name { get; private set; }

    public List<Token> Params { get; private set; }

    public List<Statement> Body { get; private set; }

    public Function(Token name, List<Token> @params, List<Statement> body)
    {
        Name = name;
        Params = @params;
        Body = body;
    }


    public override T? Accept<T>(IStmtVisitor<T?> stmtVisitor) where T : default
    {
        return stmtVisitor.VisitFunctionStmt(this);
    }
}