using SrtVideoPlayer.Shared.Models.Enums;

namespace SrtVideoPlayer.Shared.Models.Results
{
    class LexemeResult
    {
        public bool Successful { get; }
        public string ErrorMessage { get; }
        public TerminalSymbol TerminalSymbol { get; }

        public LexemeResult(TerminalSymbol terminalSymbol)
        {
            Successful = true;
            TerminalSymbol = terminalSymbol;
        }

        public LexemeResult(string errorMessage)
        {
            Successful = false;
            ErrorMessage = errorMessage;
        }
    }
}
