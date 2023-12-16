using DanilvarScript.Visitor;

namespace DanilvarScript.Expr;

public class Grouping : Expression
{
    public Expression Expression { get; private set; }

    public Grouping(Expression expression)
    {
        Expression = expression;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VistGroupingExpr(this);
    }
}