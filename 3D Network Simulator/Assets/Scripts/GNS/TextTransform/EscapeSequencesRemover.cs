using System.Text.RegularExpressions;
using Interfaces.TextTransformer;

namespace Text
{
    public class EscapeSequenceRemover : ITextTransformer
    {
        public string Process(string input)
        {
            return RemoveControlCharacters(input);
        }

        public string RemoveControlCharacters(string input)
        {
            string cleanedInput = Regex.Replace(input, "\\[\\dm", "");
            cleanedInput = Regex.Replace(cleanedInput, "\u001B", "");
            cleanedInput = Regex.Replace(cleanedInput, "\u0007", "");
            cleanedInput = Regex.Replace(cleanedInput, "\u0008", "");
            return cleanedInput;
        }
    }
}