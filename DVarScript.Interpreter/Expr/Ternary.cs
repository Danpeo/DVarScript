using DVarScript.Interpreter.Visitor;

namespace DVarScript.Interpreter.Expr;

public class Ternary : Expression
{
    public Expression Condition { get; private set; }
    public Expression TrueBranch { get; private set; }
    public Expression FalseBranch { get; private set; }

    public Ternary(Expression condition, Expression trueBranch, Expression falseBranch)
    {
        Condition = condition;
        TrueBranch = trueBranch;
        FalseBranch = falseBranch;
    }
    
    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitTernaryExpr(this);
    }
}