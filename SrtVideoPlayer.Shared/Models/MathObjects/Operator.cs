using SrtVideoPlayer.Shared.Models.Enums;

namespace SrtVideoPlayer.Shared.Models.MathObjects
{
    class Operator : MathObject
    {
        public TerminalSymbol TerminalSymbol { get; }
        public int Precedence { get; }
        public string Symbol { get; }
        public bool? RightAssociative { get; }
        public bool? Unary { get; }

        public Operator(TerminalSymbol terminalSymbol, int precedence, char symbol)
        {
            TerminalSymbol = terminalSymbol;
            Precedence = precedence;
            Symbol = char.ToString(symbol);
        }

        public Operator(TerminalSymbol terminalSymbol, int precedence, char symbol, bool rightAssociative, bool unary)
        {
            TerminalSymbol = terminalSymbol;
            Precedence = precedence;
            Symbol = char.ToString(symbol);
            RightAssociative = rightAssociative;
            Unary = unary;
        }
    }
}
