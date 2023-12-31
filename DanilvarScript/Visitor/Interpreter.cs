using System.Globalization;
using DanilvarScript.Errors;
using DanilvarScript.Expr;
using DanilvarScript.Tokens;

namespace DanilvarScript.Visitor;

public class Interpreter : IVisitor<object>
{
    public void Interpret(Expression expression)
    {
        try
        {
            object value = Evaluate(expression);
            Console.WriteLine(Stringify(value));
        }
        catch (RuntimeError error)
        {
            DVScript.RuntimeError(error);
        }
    }
    
    public object? VisitBinaryExpr(Binary expr)
    {
        object left = Evaluate(expr.Left);
        object right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.Greater:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left > (double)right;
            case TokenType.GreaterEqual:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left >= (double)right;
            case TokenType.Less:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left < (double)right;
            case TokenType.LessEqual:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left <= (double)right;
            case TokenType.BangEqual:
                return !IsEqual(left, right);
            case TokenType.EqualEqual:
                return IsEqual(left, right);
            case TokenType.Minus:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left - (double)right;
            case TokenType.Slash:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left / (double)right;
            case TokenType.Star:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left * (double)right;
            case TokenType.Plus:
                return AddOperands(expr, left, right);
        }

        return null;
    }

    public object VistGroupingExpr(Grouping expr)
    {
        return Evaluate(expr.Expression);
    }

    public object VisitLiteralExpr(Literal expr)
    {
        return expr.Value!;
    }

    public object? VisitUnaryExpr(Unary expr)
    {
        object right = Evaluate(expr.Right);

        switch (expr.TokenOperator.Type)
        {
            case TokenType.Bang:
                return !IsTruthy(right);
            case TokenType.Minus:
                CheckNumberOperand(expr.TokenOperator, right);
                return -(double)right;
            default:
                return null;
        }
    }

    public object VisitTernaryExpr(Ternary expr)
    {
        object condition = Evaluate(expr.Condition);
        object trueBranch = Evaluate(expr.TrueBranch);
        object falseBranch = Evaluate(expr.FalseBranch);
        
        return IsTruthy(condition) ? trueBranch : falseBranch;
    }

    private string Stringify(object? obj)
    {
        if (obj == null)
            return "nil";

        if (obj is double num)
        {
            string text = num.ToString(CultureInfo.InvariantCulture);

            if (text.EndsWith(".0"))
            {
                text = text.Substring(0, text.Length - 2);
            }

            return text;
        }

        return obj.ToString()!;
    }

    private static object? AddOperands(Binary expr, object left, object right)
    {
        if (left is double && right is double)
            return (double)left + (double)right;
        if (left is string && right is string)
            return (string)left + (string)right;
        if (left is string && right is double)
            return (string)left + (double)right;
        if (left is double && right is string)
            return (double)left + (string)right;

        throw new RuntimeError(expr.Operator,
            "Operands must either numbers or strings.");
    }

    private void CheckNumberOperand(Token operatorToken, object operand)
    {
        if (operand is double)
            return;

        throw new RuntimeError(operatorToken, "Operand must be a number.");
    }

    private void CheckNumberOperands(Token operatorToken, object left, object right)
    {
        if (left is double && right is double)
            return;

        throw new RuntimeError(operatorToken, "Operands must be a number.");
    }

    private bool IsTruthy(object? obj)
    {
        if (obj == null)
            return false;

        /*if (obj == (object)0)
            return false;

        if ((string)obj == "0")
            return false;*/

        if (obj is bool boolean)
            return boolean;

        return true;
    }

    private bool IsEqual(object? lhs, object? rhs)
    {
        if (lhs == null && rhs == null)
            return false;

        if (lhs == null)
            return false;

        return lhs.Equals(rhs);
    }

    private object Evaluate(Expression expression) =>
        expression.Accept(this);
}