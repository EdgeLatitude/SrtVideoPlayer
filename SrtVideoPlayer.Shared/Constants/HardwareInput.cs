namespace SrtVideoPlayer.Shared.Constants
{
    public static class HardwareInput
    {
        public const string CopyCharacter = "c";
        public const string RootCharacter = "r";
        public const string ResultOperator = LexicalSymbolsAsString.ResultOperator;

        public static readonly string[] ParenthesesDecimalSeparatorsAndOperators = new string[]
        {
            LexicalSymbolsAsString.OpeningParenthesis,
            LexicalSymbolsAsString.ClosingParenthesis,
            LexicalSymbolsAsString.Comma,
            LexicalSymbolsAsString.Dot,
            LexicalSymbolsAsString.AdditionOperator,
            LexicalSymbolsAsString.SubstractionOperator,
            LexicalSymbolsAsString.MultiplicationOperator,
            LexicalSymbolsAsString.DivisionOperator,
            LexicalSymbolsAsString.PotentiationOperator,
            LexicalSymbolsAsString.SquareRootOperator,
            LexicalSymbolsAsString.SimpleMultiplicationOperator,
            LexicalSymbolsAsString.SimpleDivisionOperator
        };
    }
}
