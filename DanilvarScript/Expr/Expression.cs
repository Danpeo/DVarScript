using DanilvarScript.Visitor;

namespace DanilvarScript.Expr;

public abstract class Expression
{
    public abstract T Accept<T>(IVisitor<T> visitor);

}