using DanilvarScript.Expr;
using DanilvarScript.Tokens;

namespace DanilvarScript.Visitor;

public class Interpreter : IVisitor<object>
{
    public object VisitBinaryExpr(Binary expr)
    {
        throw new NotImplementedException();
    }

    public object VistGroupingExpr(Grouping expr)
    {
        return Evaluate(expr.Expression);
    }

    public object VisitLiteralExpr(Literal expr)
    {
        return expr.Value!;
    }

    public object? VisitUnaryExpr(Unary expr)
    {
        object right = Evaluate(expr.Right);

        return expr.TokenOperator.Type switch
        {
            TokenType.Bang => !IsTruthy(right),
            TokenType.Minus => -(double)right,
            _ => null
        };
    }

    public object VisitTernaryExpr(Ternary expr)
    {
        throw new NotImplementedException();
    }

    private bool IsTruthy(object right)
    {
        throw new NotImplementedException();
    }

    private object Evaluate(Expression expression) => 
        expression.Accept(this);
}