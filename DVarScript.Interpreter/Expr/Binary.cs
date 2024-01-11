using DVarScript.Interpreter.Tokens;
using DVarScript.Interpreter.Visitor;

namespace DVarScript.Interpreter.Expr;

public class Binary : Expression
{
    public Expression Left { get; private set; }
    public Token Operator { get; private set; }
    public Expression Right { get; private set; }

    public Binary(Expression left, Token operatorToken, Expression right)
    {
        Left = left;
        Operator = operatorToken;
        Right = right;
    }

    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitBinaryExpr(this);
    }
}