using DVarScript.Interpreter.Tokens;
using DVarScript.Interpreter.Visitor;

namespace DVarScript.Interpreter.Expr;

public class Call : Expression
{
    public Expression Callee { get; private set; }

    public Token Paren { get; private set; }

    public List<Expression> Args { get; set; }

    public Call(Expression callee, Token paren, List<Expression> args)
    {
        Callee = callee;
        Paren = paren;
        Args = args;
    }


    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitCallExpr(this);
    }
}