using DanilvarScript.Tokens;
using DanilvarScript.Visitor;

namespace DanilvarScript.Expr;

public class Unary : Expression
{
    public Token TokenOperator { get; private set; }

    public Expression Right { get; private set; }

    public Unary(Token tokenOperator, Expression right)
    {
        TokenOperator = tokenOperator;
        Right = right;
    }

    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitUnaryExpr(this);
    }
}