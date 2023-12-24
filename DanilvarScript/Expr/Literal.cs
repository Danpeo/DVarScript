using DanilvarScript.Visitor;

namespace DanilvarScript.Expr;

public class Literal : Expression
{
    public object? Value { get; private set; }

    public Literal(object? value)
    {
        Value = value;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitLiteralExpr(this);
    }
}