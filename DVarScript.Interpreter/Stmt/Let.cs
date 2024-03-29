using DVarScript.Interpreter.Expr;
using DVarScript.Interpreter.Tokens;
using DVarScript.Interpreter.Visitor;

namespace DVarScript.Interpreter.Stmt;

public class Let : Statement
{
    public Token Name { get; private set; }

    public Expression? Initializer { get; private set; }

    public Let(Token name, Expression? initializer)
    {
        Name = name;
        Initializer = initializer;
    }

    public override T? Accept<T>(IStmtVisitor<T?> stmtVisitor) where T : default
    {
        return stmtVisitor.VisitLetStmt(this);
    }
}