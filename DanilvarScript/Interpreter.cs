using System.Globalization;
using DanilvarScript.Env;
using DanilvarScript.Errors;
using DanilvarScript.Expr;
using DanilvarScript.Stmt;
using DanilvarScript.Tokens;
using DanilvarScript.Visitor;

namespace DanilvarScript;

public class Interpreter : IExprVisitor<object>, IStmtVisitor<object>
{
    private readonly VariableEnvironment _environment = new();

    public void Interpret(IEnumerable<Statement?> statements)
    {
        try
        {
            foreach (Statement? statement in statements)
            {
                Execute(statement);
            }
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
                /*CheckNumberOperands(expr.Operator, left, right);
                return (double)left * (double)right;*/
                return MultiplyOperands(expr, left, right);
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

    public object? VisitVariableExpr(Variable expr)
    {
        return _environment.Get(expr.Name);
    }

    public object VisitAssignExpr(Assign expr)
    {
        object value = Evaluate(expr.Value);
        _environment.Assign(expr.Name, value);

        return value;
    }

    public object? VisitPrintStmt(Print stmt)
    {
        object value = Evaluate(stmt.Expression);
        Console.WriteLine(Stringify(value));

        return null;
    }

    public object? VisitExpressionStmt(ExpressionStmt stmt)
    {
        Evaluate(stmt.Expression);

        return null;
    }

    public object? VisitLetStmt(Let stmt)
    {
        object? value = null;

        if (stmt.Initializer != null)
            value = Evaluate(stmt.Initializer);

        _environment.Define(stmt.Name, value);

        return null;
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

    private static object? MultiplyOperands(Binary expr, object left, object right)
    {
        if (left is double && right is double)
            return (double)left * (double)right;
        if (left is string && right is double)
            return MultiplyString((string)left, (int)(double)right);
        if (left is double && right is string)
            return MultiplyString((string)right, (int)(double)left);

        throw new RuntimeError(expr.Operator,
            "Operands must either numbers or strings.");
    }

    private static string MultiplyString(string str, int by)
    {
        string result = "";

        for (int i = 0; i < by; i++)
        {
            result += str;
        }

        return result;
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

    private void Execute(Statement? statement)
    {
        statement.Accept(this);
    }

    private object Evaluate(Expression expression) =>
        expression.Accept(this);
}