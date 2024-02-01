using DVarScript.Interpreter.Stmt;

namespace DVarScript.Interpreter.Visitor;

public interface IStmtVisitor<out T>
{
    T? VisitPrintStmt(Print stmt);
    T? VisitExpressionStmt(ExpressionStmt stmt);
    T? VisitLetStmt(Let stmt);
    T? VisitBlockStmt(Block stmt);
    T? VisitIfStmt(If stmt);
    T? VisitWhileStmt(While stmt);
    T? VisitForawhileStmt(Forawhile stmt);
    T? VisitFunctionStmt(Function stmt);
    T? VisitReturnStmt(Return stmt);
}