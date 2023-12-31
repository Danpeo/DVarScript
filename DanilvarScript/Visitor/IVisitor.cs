using DanilvarScript.Expr;

namespace DanilvarScript.Visitor;

public interface IVisitor<out T>
{
    T? VisitBinaryExpr(Binary expr);
    T? VistGroupingExpr(Grouping expr);
    T VisitLiteralExpr(Literal expr);
    T? VisitUnaryExpr(Unary expr);
    T VisitTernaryExpr(Ternary expr);
}