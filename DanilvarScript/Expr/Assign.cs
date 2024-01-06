using DanilvarScript.Tokens;
using DanilvarScript.Visitor;

namespace DanilvarScript.Expr;

public class Assign : Expression
{
    public Token Name { get; private set; }

    public Expression Value { get; private set; }

    public Assign(Token name, Expression value)
    {
        Name = name;
        Value = value;
    }

    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitAssignExpr(this);
    }
}