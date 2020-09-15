using SrtVideoPlayer.Shared.Models.Enums;

namespace SrtVideoPlayer.Shared.Models.Results
{
    class LexicalAnalysisResult
    {
        public string[] Lexemes { get; }
        public TerminalSymbol[] TerminalSymbols { get; }

        public LexicalAnalysisResult(string[] lexemes, TerminalSymbol[] terminalSymbols)
        {
            Lexemes = lexemes;
            TerminalSymbols = terminalSymbols;
        }
    }
}
