using System.Data.Common;
using DanilvarScript.Tokens;

namespace DanilvarScript;

public class Scanner
{
    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        { "and", TokenType.And },
        { "class", TokenType.Class },
        { "else", TokenType.Else },
        { "false", TokenType.False },
        { "for", TokenType.For },
        { "fun", TokenType.Fun },
        { "if", TokenType.If },
        { "nil", TokenType.Nil },
        { "or", TokenType.Or },
        { "print", TokenType.Print },
        { "return", TokenType.Return },
        { "super", TokenType.Super },
        { "this", TokenType.This },
        { "true", TokenType.True },
        { "let", TokenType.Let },
        { "while", TokenType.While }
    };
    
    private readonly string _source;
    private readonly List<Token> _tokens = new();
    private int _start = 0;
    private int _current = 0;
    private int _line = 1;

    public Scanner(string source)
    {
        _source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _start = _current;
            ScanToken();
        }

        _tokens.Add(new Token(TokenType.Eof, "", null, _line));

        return _tokens;
    }

    private bool IsAtEnd() =>
        _current >= _source.Length;

    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            case '(':
                AddToken(TokenType.LeftParen);
                break;
            case ')':
                AddToken(TokenType.RightParen);
                break;
            case '{':
                AddToken(TokenType.LeftBrace);
                break;
            case '}':
                AddToken(TokenType.RightBrace);
                break;
            case ',':
                AddToken(TokenType.Comma);
                break;
            case '.':
                AddToken(TokenType.Dot);
                break;
            case '-':
                AddToken(TokenType.Minus);
                break;
            case '+':
                AddToken(TokenType.Plus);
                break;
            case ';':
                AddToken(TokenType.Semicolon);
                break;
            case '*':
                AddToken(TokenType.Star);
                break;
            case '!':
                AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
                break;
            case '/':
                if (Match('/'))
                {
                    // A comment goes until the end of the line.
                    while (Peek() != '\n' && !IsAtEnd())
                        Advance();
                }
                else if (Match('*'))
                {
                    MultilineComment();
                }
                else
                {
                    AddToken(TokenType.Slash);
                }
                break;
            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace.
                break;

            case '\n':
                _line++;
                break;
            case '"':
                String();
                break;
            default:
                if (char.IsDigit(c))
                {
                    Number();
                }
                else if (char.IsLetter(c))
                {
                    Identifier();
                }
                else
                {
                    DVScript.Error(_line, "Unexpected character.");
                }

                break;
        }
    }

    private void MultilineComment()
    {
        while (!(Peek() == '*' && Peek(1) == '/') && !IsAtEnd())
        {
            if (Peek() == '\n')
                _line++;
            Advance();
        }

        // Consume the closing */
        if (!IsAtEnd())
        {
            Advance(); // '*'
            Advance(); // '/'
        }
    }

    private void Identifier()
    {
        while (char.IsLetterOrDigit(Peek()))
            Advance();

        string text = _source.Substring(_start, _current - _start);

        if (Keywords.TryGetValue(text, out TokenType type))
            AddToken(type);
        else
            AddToken(TokenType.Identifier);
    }

    private void Number()
    {
        while (char.IsDigit(Peek()))
            Advance();

        // Look for a fractional part.
        if (Peek() == '.' && char.IsDigit(Peek(1)))
        {
            // Consume the "."
            Advance();

            while (char.IsDigit(Peek()))
                Advance();
        }

        AddToken(TokenType.Number, double.Parse(_source.Substring(_start, _current - _start)));
    }

    private void String()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
                _line++;
            Advance();
        }

        if (IsAtEnd())
        {
            DVScript.Error(_line, "Unterminated string.");
            return;
        }

        // The closing ".
        Advance();

        string value = _source.Substring(_start + 1, _current - _start - 1);
        AddToken(TokenType.String, value);
    }

    private bool Match(char expected)
    {
        if (IsAtEnd())
            return false;

        if (_source[_current] != expected)
            return false;

        _current++;
        return true;
    }

    private char Peek(int offset = 0) 
        => _current + offset >= _source.Length ? '\0' : _source[_current + offset];

    private char Advance() =>
        _source[_current++];

    private void AddToken(TokenType type, object? literal = null)
    {
        string text = _source.Substring(_start, _current - _start);
        _tokens.Add(new Token(type, text, literal, _line));
    }
}