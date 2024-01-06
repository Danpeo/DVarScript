using DanilvarScript.Visitor;

namespace DanilvarScript.Stmt;

public abstract class Statement
{
    public abstract T? Accept<T>(IStmtVisitor<T?> stmtVisitor);
}