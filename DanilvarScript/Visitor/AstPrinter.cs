using System.Text;
using DanilvarScript.Expr;

namespace DanilvarScript.Visitor;

public class AstPrinter : IVisitor<string>
{
    public string Print(Expression expr)
    {
        return expr.Accept(this);
    }

    public string VisitBinaryExpr(Binary expr) 
        => Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);

    public string VistGroupingExpr(Grouping expr)
        => Parenthesize("group", expr.Expression);

    public string VisitLiteralExpr(Literal? expr)
    {
        if (expr is null)
            return "nil";
        
        return expr.Value.ToString();
    }

    public string VisitUnaryExpr(Unary expr) 
        => Parenthesize(expr.TokenOperator.Lexeme, expr.Right);

    public string VisitTernaryExpr(Ternary expr) 
        => Parenthesize("ternary", expr.Condition, expr.TrueBranch, expr.FalseBranch);

    private string Parenthesize(string name, params Expression[] expressions)
    {
        var strBuilder = new StringBuilder();

        strBuilder
            .Append('(')
            .Append(name);

        foreach (Expression expr in expressions)
        {
            strBuilder.Append(' ');
            strBuilder.Append(expr.Accept(this));
        }

        strBuilder.Append(')');

        return strBuilder.ToString();
    }
}