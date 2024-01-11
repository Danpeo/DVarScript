using DVarScript.Interpreter.Visitor;

namespace DVarScript.Interpreter.Expr;

public abstract class Expression
{
    public abstract T Accept<T>(IExprVisitor<T> exprVisitor);

}