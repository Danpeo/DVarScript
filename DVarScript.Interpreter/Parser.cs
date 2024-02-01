using DVarScript.Interpreter.Expr;
using DVarScript.Interpreter.Stmt;
using DVarScript.Interpreter.Tokens;
using Expression = DVarScript.Interpreter.Expr.Expression;

namespace DVarScript.Interpreter;

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

    public IEnumerable<Statement?> ParseStatements()
    {
        var statments = new List<Statement?>();

        while (!IsAtEnd())
        {
            statments.Add(Declaration());
        }

        return statments;
    }

    public Expression? ParseExpression()
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

    private Statement Statement()
    {
        if (Match(TokenType.If))
            return IfStmt();

        if (Match(TokenType.While))
            return WhileStmt();

        if (Match(TokenType.For))
            return ForStmt();

        if (Match(TokenType.Forever))
            return ForeverStmt();

        if (Match(TokenType.Forawhile))
            return ForawhileStmt();

        if (Match(TokenType.LeftBrace))
            return new Block(Block());

        if (Match(TokenType.Print))
            return PrintStmt();

        if (Match(TokenType.Return))
            return ReturnStmt();
        
        return ExpressionStmt();
    }

    private Statement ReturnStmt()
    {
        Token keyword = Prev();
        Expression? value = null;

        if (!Check(TokenType.Semicolon))
            value = Expression();

        Consume(TokenType.Semicolon, ';', "return value");
        return new Return(keyword, value);
    }

    private Statement ForStmt()
    {
        ConsumeSymbol("for", '(', TokenType.LeftParen);

        Statement? initializer;

        if (Match(TokenType.Semicolon))
            initializer = null;
        else if (Match(TokenType.Let))
            initializer = LetDeclaration();
        else
            initializer = ExpressionStmt();

        Expression? condition = null;

        if (!Check(TokenType.Semicolon))
            condition = Expression();

        ConsumeSymbol("loop condition");

        Expression? increment = null;

        if (!Check(TokenType.RightParen))
            increment = Expression();

        ConsumeSymbol("for clauses", ')', TokenType.RightParen);

        Statement body = Statement();

        if (increment != null)
        {
            var statements = new List<Statement>
            {
                body,
                new ExpressionStmt(increment)
            };
            body = new Block(statements);
        }

        condition ??= new Literal(true);

        body = new While(condition, body);

        if (initializer != null)
        {
            var statements = new List<Statement>
            {
                initializer,
                body
            };
            body = new Block(statements);
        }

        return body;
    }

    private Statement WhileStmt()
    {
        ConsumeSymbol("while", '(', TokenType.LeftParen);
        Expression condition = Expression();
        ConsumeSymbol("while", '(', TokenType.RightParen);
        Statement body = Statement();

        return new While(condition, body);
    }

    private Statement ForawhileStmt() =>
        new Forawhile(Statement());

    private Statement ForeverStmt()
    {
        Expression condition = new Literal(true);
        return new While(condition, Statement());
    }

    private Statement IfStmt()
    {
        ConsumeSymbol("if", '(', TokenType.LeftParen);
        Expression condition = Expression();
        ConsumeSymbol("if", ')', TokenType.RightParen);

        Statement trueBranch = Statement();
        Statement? falseBranch = null;

        if (Match(TokenType.Else))
            falseBranch = Statement();

        return new If(condition, trueBranch, falseBranch);
    }

    private Expression Expression()
    {
        //return Equality();
        //return Ternary();
        return Assignment();
    }

    private Expression Assignment()
    {
        Expression expression = Or();

        if (Match(TokenType.Equal))
        {
            Token equals = Prev();
            Expression value = Assignment();

            if (expression is Variable variable)
            {
                Token name = variable.Name;

                return new Assign(name, value);
            }

            Error(equals, "Invalid assignment target.");
        }

        return expression;
    }

    private Expression Or() =>
        BaseLogicalExpr(And, TokenType.Or);

    private Expression And() =>
        BaseLogicalExpr(Ternary, TokenType.And);

    private Statement? Declaration()
    {
        try
        {
            if (Match(TokenType.Func))
                return FunctionDeclaration("function");

            if (Match(TokenType.Let))
                return LetDeclaration();

            return Statement();
        }
        catch (ParseError e)
        {
            Sync();
            return null;
        }
    }

    private Function FunctionDeclaration(string kind)
    {
        Token name = Consume(TokenType.Identifier, $"Expect {kind} name.");

        Consume(TokenType.LeftParen, '(', kind);

        var parameters = new List<Token>();

        if (!Check(TokenType.RightParen))
        {
            do
            {
                if (parameters.Count >= 255)
                    Error(Peek(), "Can't have more than 255 parameters.");

                parameters.Add(Consume(TokenType.Identifier, "Expect parameter name."));
            } while (Match(TokenType.Comma));
        }

        Consume(TokenType.RightParen, ')', kind);
        Consume(TokenType.LeftBrace, "Expect '{' before " + kind + " body.");
        
        List<Statement> body = Block();
        
        return new Function(name, parameters, body);
    }

    private Statement LetDeclaration()
    {
        Token name = Consume(TokenType.Identifier, "Expect variable name.");

        Expression? initializer = null;
        if (Match(TokenType.Be))
        {
            initializer = Expression();
        }

        ConsumeSymbol("variable declaration");
        return new Let(name, initializer);
    }

    private List<Statement> Block()
    {
        var statements = new List<Statement>();

        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            statements.Add(Declaration());
        }

        ConsumeSymbol("block", '}', TokenType.RightBrace);
        return statements;
    }

    private Statement ExpressionStmt()
    {
        Expression expression = Expression();
        ConsumeSymbol("expression");
        return new ExpressionStmt(expression);
    }

    private Statement PrintStmt()
    {
        Expression value = Expression();
        ConsumeSymbol("value");

        return new Print(value);
    }

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

        return CallExpr();
    }

    private Expression CallExpr()
    {
        Expression expression = Primary();

        while (true)
        {
            if (Match(TokenType.LeftParen))
            {
                expression = FinishCall(expression);
            }
            else
            {
                break;
            }
        }

        return expression;
    }

    private Expression FinishCall(Expression callee)
    {
        var args = new List<Expression>();

        if (!Check(TokenType.RightParen))
        {
            do
            {
                if (args.Count >= 255)
                    Error(Peek(), "Can't have more than 255 arguments.");

                args.Add(Expression());
            } while (Match(TokenType.Comma));
        }

        Token paren = Consume(TokenType.RightParen, ')', "arguments");

        return new Call(callee, paren, args);
    }

    private Expression Ternary()
    {
        Expression condition = Equality();

        if (Match(TokenType.QuestionMark))
        {
            Expression trueBranch = Expression();
            Consume(TokenType.Colon, "Expect ':' After true branch of expression.");
            Expression falseBranch = Ternary();

            return new Ternary(condition, trueBranch, falseBranch);
        }

        return condition;
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

        if (Match(TokenType.Identifier))
            return new Variable(Prev());

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

    private Token Consume(TokenType type, char symbol, string after) =>
        Consume(type, $"Expect '{symbol}' after {after}.");

    private void ConsumeSymbol(string after = "statement", char symbol = ';',
        TokenType tokenType = TokenType.Semicolon)
    {
        Consume(tokenType, $"Expect '{symbol}' after {after}.");
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
                case TokenType.Func:
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

    private Expression BaseLogicalExpr(Func<Expression> nextExpr, params TokenType[] operators)
    {
        Expression expression = nextExpr();

        while (Match(operators))
        {
            Token operatorToken = Prev();
            Expression right = nextExpr();

            expression = new Logical(expression, operatorToken, right);
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

    private Token Peek(int offset = 0) =>
        _tokens[_current + offset];

    private Token Prev()
    {
        return _tokens[_current - 1];
    }
}