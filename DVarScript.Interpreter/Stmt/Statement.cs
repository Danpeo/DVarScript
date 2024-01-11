using DVarScript.Interpreter.Visitor;

namespace DVarScript.Interpreter.Stmt;

public abstract class Statement
{
    public abstract T? Accept<T>(IStmtVisitor<T?> stmtVisitor);
}