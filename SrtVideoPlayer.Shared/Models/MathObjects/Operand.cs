namespace SrtVideoPlayer.Shared.Models.MathObjects
{
    class Operand : MathObject
    {
        public decimal Value { get; }

        public Operand(decimal value)
        {
            Value = value;
        }
    }
}
