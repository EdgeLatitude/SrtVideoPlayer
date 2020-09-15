namespace SrtVideoPlayer.Shared.Models.Results
{
    class CalculationResult
    {
        public bool Successful { get; }
        public string ErrorMessage { get; }
        public decimal Result { get; }

        public CalculationResult(decimal result)
        {
            Successful = true;
            Result = result;
        }

        public CalculationResult(string errorMessage)
        {
            Successful = false;
            ErrorMessage = errorMessage;
        }
    }
}
