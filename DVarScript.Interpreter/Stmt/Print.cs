using DVarScript.Interpreter.Expr;
using DVarScript.Interpreter.Visitor;

namespace DVarScript.Interpreter.Stmt;

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