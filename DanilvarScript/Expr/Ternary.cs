using DanilvarScript.Visitor;

namespace DanilvarScript.Expr;

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
    
    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitTernaryExpr(this);
    }
}