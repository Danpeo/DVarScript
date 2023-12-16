using DanilvarScript.Tokens;
using DanilvarScript.Visitor;

namespace DanilvarScript.Expr;

public class Unary : Expression
{
    public Token Operator { get; private set; }

    public Expression Right { get; private set; }

    public Unary(Token @operator, Expression right)
    {
        Operator = @operator;
        Right = right;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitUnaryExpr(this);
    }
}