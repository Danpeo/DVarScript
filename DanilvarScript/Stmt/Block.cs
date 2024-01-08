using DanilvarScript.Visitor;

namespace DanilvarScript.Stmt;

public class Block : Statement
{
    public List<Statement> Statements { get; private set; }

    public Block(List<Statement> statements)
    {
        Statements = statements;
    }

    public override T? Accept<T>(IStmtVisitor<T?> stmtVisitor) where T : default
    {
        return stmtVisitor.VisitBlockStmt(this);
    }
}