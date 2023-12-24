using DanilvarScript.Expr;
using DanilvarScript.Tokens;

namespace DanilvarScript;

public class Parser
{
    private class ParseError : Exception
    {
    }

    private readonly List<Token> _tokens;
    private int _current = 0;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public Expression? Parse()
    {
        try
        {
            return Expression();
        }
        catch (ParseError e)
        {
            return null;
        }
    }
    
    private Expression Expression()
        => Equality();

    private Expression Equality()
        => BaseBinaryExpr(ComparisonExpr, TokenType.EqualEqual, TokenType.BangEqual);

    private Expression ComparisonExpr()
        => BaseBinaryExpr(Term, TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual);

    private Expression Term()
        => BaseBinaryExpr(Factor, TokenType.Minus, TokenType.Plus);

    private Expression Factor()
        => BaseBinaryExpr(Unary, TokenType.Slash, TokenType.Star);

    private Expression Unary()
    {
        if (Match(TokenType.Bang, TokenType.Minus))
        {
            Token operatorToken = Prev();
            Expression right = Unary();
            return new Unary(operatorToken, right);
        }

        return Primary();
    }

    private Expression Primary()
    {
        if (Match(TokenType.False))
            return new Literal(false);
        if (Match(TokenType.True))
            return new Literal(true);
        if (Match(TokenType.Nil))
            return new Literal(null);

        if (Match(TokenType.Number, TokenType.String))
            return new Literal(Prev().Literal);

        if (Match(TokenType.LeftParen))
        {
            Expression expression = Expression();
            Consume(TokenType.RightParen, "Expect ')' after expression.");

            return new Grouping(expression);
        }

        throw Error(Peek(), "Expect expression.");
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type))
            return Advance();

        throw Error(Peek(), message);
    }

    private ParseError Error(Token token, string message)
    {
        DVScript.Error(token, message);

        return new ParseError();
    }

    private void Sync()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Prev().Type == TokenType.Semicolon)
                return;

            switch (Peek().Type)
            {
                case TokenType.Class:
                case TokenType.Fun:
                case TokenType.Let:
                case TokenType.For:
                case TokenType.If:
                case TokenType.While:
                case TokenType.Print:
                case TokenType.Return:
                    return;
            }

            Advance();
        }
    }

    private Expression BaseBinaryExpr(Func<Expression> nextExpr, params TokenType[] operators)
    {
        Expression expression = nextExpr();

        while (Match(operators))
        {
            Token operatorToken = Prev();
            Expression right = nextExpr();

            expression = new Binary(expression, operatorToken, right);
        }

        return expression;
    }

    private bool Match(params TokenType[] tokenTypes)
    {
        foreach (TokenType tokenType in tokenTypes)
        {
            if (Check(tokenType))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    private bool Check(TokenType tokenType)
    {
        if (IsAtEnd())
            return false;

        return Peek().Type == tokenType;
    }

    private Token Advance()
    {
        if (!IsAtEnd())
            _current++;

        return Prev();
    }

    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.Eof;
    }

    private Token Peek()
    {
        return _tokens[_current];
    }

    private Token Prev()
    {
        return _tokens[_current - 1];
    }
}