using DanilvarScript.Visitor;

namespace DanilvarScript.Expr;

public class Literal : Expression
{
    public object? Value { get; private set; }

    public Literal(object? value)
    {
        Value = value;
    }

    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitLiteralExpr(this);
    }
}