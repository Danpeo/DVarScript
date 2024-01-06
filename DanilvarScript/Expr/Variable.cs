using DanilvarScript.Tokens;
using DanilvarScript.Visitor;

namespace DanilvarScript.Expr;

public class Variable : Expression
{
    public Token Name { get; private set; }

    public Variable(Token name)
    {
        Name = name;
    }

    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitVariableExpr(this);
    }
}