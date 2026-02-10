using System;

namespace Implem.Libraries.Exceptions
{
    public class FormulaErrorException: Exception
    {
        public string ErrorDetails { get; set; }

        public FormulaErrorException(string message) : base(message)
        {
        }

        public FormulaErrorException(string message, string errorDetails) : base(message)
        {
            ErrorDetails = errorDetails;
        }

        public override string ToString()
        {
            string text = base.ToString();
            if (!string.IsNullOrEmpty(ErrorDetails) && ErrorDetails != Message)
            {
                string text2 = $"   {ErrorDetails.Replace("\n", "\n   ")}";
                text = $"{text}\n   --- Script error details follow ---\n{text2}";
            }
            return text;
        }
    }
}
