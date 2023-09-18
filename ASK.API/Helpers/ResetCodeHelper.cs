using System.Text;

namespace ASK.API.Helpers
{
    public class ResetCodeHelper
    {
        private const string AllowedCharacters = "QWERTYUPASDFGHJKLXCVBNM346789";

        private const int CodeLength = 6;

        private static readonly Random Random = new Random();

        private static readonly HashSet<string> GeneratedCodes = new HashSet<string>();

        public static string GenerateResetCode()
        {
            while (true)
            {
                var code = GenerateRandomCode();
                if (GeneratedCodes.Add(code))
                {
                    return code;
                }
            }
        }

        private static string GenerateRandomCode()

        {
            var codeBuilder = new StringBuilder();
            for (int i = 0; i < CodeLength; i++)
            {
                int randomIndex = Random.Next(0, AllowedCharacters.Length);
                codeBuilder.Append(AllowedCharacters[randomIndex]);
            }
            return codeBuilder.ToString();
        }
    }
}
