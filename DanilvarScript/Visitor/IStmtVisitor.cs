using DanilvarScript.Stmt;

namespace DanilvarScript.Visitor;

public interface IStmtVisitor<out T>
{
    T? VisitPrintStmt(Print stmt);
    T? VisitExpressionStmt(ExpressionStmt stmt);
    T? VisitLetStmt(Let stmt);
    T? VisitBlockStmt(Block stmt);
    T? VisitIfStmt(If stmt);
    T? VisitWhileStmt(While stmt);
    T? VisitForeverStmt(Forever stmt);
    T? VisitForawhileStmt(Forawhile stmt);
}