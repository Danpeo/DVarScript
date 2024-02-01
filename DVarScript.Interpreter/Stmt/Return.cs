using DVarScript.Interpreter.Expr;
using DVarScript.Interpreter.Tokens;
using DVarScript.Interpreter.Visitor;

namespace DVarScript.Interpreter.Stmt;

public class Return : Statement
{
    public Token Keyword { get; private set; }

    public Expression? Value { get; private set; }

    public Return(Token keyword, Expression? value)
    {
        Keyword = keyword;
        Value = value;
    }

    public override T? Accept<T>(IStmtVisitor<T?> stmtVisitor) where T : default
    {
        return stmtVisitor.VisitReturnStmt(this);
    }
}