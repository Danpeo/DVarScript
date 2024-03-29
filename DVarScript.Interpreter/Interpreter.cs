using System.Globalization;
using DVarScript.Interpreter.Callables;
using DVarScript.Interpreter.Env;
using DVarScript.Interpreter.Errors;
using DVarScript.Interpreter.Expr;
using DVarScript.Interpreter.Stmt;
using DVarScript.Interpreter.Tokens;
using DVarScript.Interpreter.Visitor;

namespace DVarScript.Interpreter;

public class Interpreter : IExprVisitor<object>, IStmtVisitor<object>
{
    public ProgramEnvironment Globals { get; } = new();
    private ProgramEnvironment _environment;

    public Interpreter()
    {
        Globals.Define("clock", new ClockFunc());
        Globals.Define("println", new PrintFunc(printLine: true));
        Globals.Define("print", new PrintFunc(printLine: false));
        
        _environment = Globals;
    }
    
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
                return IsFalsey(right);
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

    public object VisitLogicalExpr(Logical expr)
    {
        object left = Evaluate(expr.Left);

        if (expr.OperatorToken.Type == TokenType.Or)
        {
            if (IsTruthy(left))
                return left;
        }
        else
        {
            if (IsFalsey(left))
                return left;
        }

        return Evaluate(expr.Right);
    }

    public object? VisitCallExpr(Call expr)
    {
        object callee = Evaluate(expr.Callee);

        var args = expr.Args.Select(Evaluate).ToList();

        if (callee is not ICallable function)
            throw new RuntimeError(expr.Paren, "Can only call functions and classes.");

        if (args.Count != function.Arity() && function.Arity() != -1)
            throw new RuntimeError(expr.Paren, $"Expected {function.Arity()} arguments but got {args.Count}.");

        return function.Call(this, args);
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

        _environment.DefineVariable(stmt.Name, value);

        return null;
    }

    public object? VisitBlockStmt(Block stmt)
    {
        ExecuteBlock(stmt.Statements, new ProgramEnvironment(_environment));
        return null;
    }

    public object? VisitIfStmt(If stmt)
    {
        if (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.TrueBranch);
        }
        else if (stmt.FalseBranch != null)
        {
            Execute(stmt.FalseBranch);
        }

        return null;
    }

    public object? VisitWhileStmt(While stmt)
    {
        while (IsTruthy(Evaluate(stmt.Condition)))
            Execute(stmt.Body);

        return null;
    }


    public object? VisitForawhileStmt(Forawhile stmt)
    {
        var random = new Random();
        for (int i = 0; i < random.Next(); i++)
            Execute(stmt.Body);

        return null;
    }

    public object? VisitFunctionStmt(Function stmt)
    {
        var function = new FunctionCallable(stmt, _environment);
        _environment.Define(stmt.Name.Lexeme, function);
        
        return null;
    }

    public object? VisitReturnStmt(Return stmt)
    {
        object? value = null;
        
        if (stmt.Value!= null)
            value = Evaluate(stmt.Value);

        throw new ReturnE(value);
    }

    public void ExecuteBlock(List<Statement> statements, ProgramEnvironment environment)
    {
        ProgramEnvironment prevEnv = _environment;

        try
        {
            _environment = environment;

            foreach (Statement statement in statements)
                Execute(statement);
        }
        finally
        {
            _environment = prevEnv;
        }
    }

    public string Stringify(object? obj)
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

    private bool IsFalsey(object? obj) =>
        !IsTruthy(obj);

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