using DanilvarScript.Expr;

namespace DanilvarScript.Visitor;

public interface IExprVisitor<out T>
{
    T? VisitBinaryExpr(Binary expr);
    T? VistGroupingExpr(Grouping expr);
    T VisitLiteralExpr(Literal expr);
    T? VisitUnaryExpr(Unary expr);
    T VisitTernaryExpr(Ternary expr);
    T? VisitVariableExpr(Variable expr);
    T VisitAssignExpr(Assign expr);
    T VisitLogicalExpr(Logical expr);
}