using DVarScript.Interpreter.Tokens;
using DVarScript.Interpreter.Visitor;

namespace DVarScript.Interpreter.Expr;

public class Logical : Expression
{
    public Expression Left { get; private set; }

    public Token OperatorToken { get; private set; }

    public Expression Right { get; private set; }

    public Logical(Expression left, Token operatorToken, Expression right)
    {
        Left = left;
        OperatorToken = operatorToken;
        Right = right;
    }

    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitLogicalExpr(this);
    }
}