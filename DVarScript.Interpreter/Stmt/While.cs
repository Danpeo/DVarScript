using DVarScript.Interpreter.Expr;
using DVarScript.Interpreter.Visitor;

namespace DVarScript.Interpreter.Stmt;

public class While : Statement
{
    public Expression Condition { get; private set; }

    public Statement Body { get; private set; }

    public While(Expression condition, Statement body)
    {
        Condition = condition;
        Body = body;
    }

    public override T? Accept<T>(IStmtVisitor<T?> stmtVisitor) where T : default
    {
        return stmtVisitor.VisitWhileStmt(this);
    }
}