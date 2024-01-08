using DanilvarScript.Expr;
using DanilvarScript.Visitor;

namespace DanilvarScript.Stmt;

public class If : Statement
{
    public Expression Condition { get; private set; }
    public Statement TrueBranch { get; private set; }
    public Statement? FalseBranch { get; set; }

    public If(Expression condition, Statement trueBranch, Statement? falseBranch)
    {
        Condition = condition;
        TrueBranch = trueBranch;
        FalseBranch = falseBranch;
    }

    public override T? Accept<T>(IStmtVisitor<T?> stmtVisitor) where T : default
    {
        return stmtVisitor.VisitIfStmt(this);
    }
}