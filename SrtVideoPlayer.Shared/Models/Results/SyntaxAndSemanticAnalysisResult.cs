using SrtVideoPlayer.Shared.Models.MathObjects;

namespace SrtVideoPlayer.Shared.Models.Results
{
    class SyntaxAndSemanticAnalysisResult
    {
        public MathObject[] PostfixOperation { get; }

        public SyntaxAndSemanticAnalysisResult(MathObject[] postfixOperation)
        {
            PostfixOperation = postfixOperation;
        }
    }
}
