using DanilvarScript.Visitor;

namespace DanilvarScript.Stmt;

public class Forever : Statement
{
    public Statement Body { get; private set; }

    public Forever(Statement body)
    {
        Body = body;
    }

    public override T? Accept<T>(IStmtVisitor<T?> stmtVisitor) where T : default
    {
        return stmtVisitor.VisitForeverStmt(this);
    }
}