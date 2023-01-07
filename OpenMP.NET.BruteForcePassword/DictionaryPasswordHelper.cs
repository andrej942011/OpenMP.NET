namespace OpenMP.NET.BruteForcePassword
{
    public static class DictionaryPasswordHelper
    {
        static char[] _symbols = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public static List<string> DictionaryPassword { get; set; }

        public static List<string> CreateDictionaryPassword(int lengtchPass)
        {
            DictionaryPassword = new List<string>();

            var attempt = "";

            var counters = new int[lengtchPass + 1];
            int cracks = 0;

            var array = new List<string>();
            for (int i = 0; i < _symbols.Length; i++)
            {
                array.Add(_symbols[i].ToString());
            }

            while (true)
            {
                var word = CreatePassPhrase(counters, array);

                var checkWord = DictionaryPassword.SingleOrDefault(x => x.Equals(word));

                if (word != null && checkWord == null && word.Length == lengtchPass)
                {
                    DictionaryPassword.Add(word);

                    //Console.WriteLine($"{cracks} {word}");
                }
                else
                {
                    return DictionaryPassword;
                }

                cracks++;
                counters[0]++;
            }
        }

        private static string? CreatePassPhrase(int[] counters, List<string> symbolsInDictionary)
        {
            for (int i = 0; i < counters.Length - 2; i++)
            {
                if (counters[i] == _symbols.Length)
                {
                    counters[i + 1]++;
                    counters[i] = 0;
                }
            }

            if (counters[counters.Length - 1] == _symbols.Length)
            {
                return null;
            }

            var splitString = "";

            //attempt = array[counters[0]] + array[counters[1]] + array[counters[2]] + array[counters[3]];
            for (int i = 0; i < counters.Length - 1; i++)
            {
                var t = counters[i];
                if (symbolsInDictionary.Count > t)
                {
                    splitString += symbolsInDictionary[counters[i]];
                }
                else
                {
                    break;
                }
            }



            return splitString;
        }
    }
}