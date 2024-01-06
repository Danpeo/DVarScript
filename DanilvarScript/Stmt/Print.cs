using DanilvarScript.Expr;
using DanilvarScript.Visitor;

namespace DanilvarScript.Stmt;

public class Print : Statement
{
    public Expression Expression { get; private set; }

    public Print(Expression expression)
    {
        Expression = expression;
    }
    
    public override T? Accept<T>(IStmtVisitor<T?> stmtVisitor) where T : default
    {
        return stmtVisitor.VisitPrintStmt(this);
    }
}