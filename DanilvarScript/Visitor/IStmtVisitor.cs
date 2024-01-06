using DanilvarScript.Stmt;

namespace DanilvarScript.Visitor;

public interface IStmtVisitor<out T>
{
    T? VisitPrintStmt(Print stmt);
    T? VisitExpressionStmt(ExpressionStmt stmt);
    T? VisitLetStmt(Let stmt);
}