using DanilvarScript.Expr;
using DanilvarScript.Visitor;

namespace DanilvarScript.Stmt;

public class ExpressionStmt : Statement
{
    public Expression Expression { get; private set; }

    public ExpressionStmt(Expression expression)
    {
        Expression = expression;
    }

    public override T? Accept<T>(IStmtVisitor<T?> stmtVisitor) where T : default
    {
        return stmtVisitor.VisitExpressionStmt(this);
    }
}