using DanilvarScript.Visitor;

namespace DanilvarScript.Stmt;

public class Forawhile : Statement
{
    public Statement Body { get; private set; }

    public Forawhile(Statement body)
    {
        Body = body;
    }

    public override T? Accept<T>(IStmtVisitor<T?> stmtVisitor) where T : default
    {
        return stmtVisitor.VisitForawhileStmt(this);
    }
}