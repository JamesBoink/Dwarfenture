namespace StormPig.UI {
    public static class IFieldFilter {
        private static readonly string[] _numbers = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", };


        /// <summary>
        /// Takes input in, removes an non-number characters and returns modified input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Filter(string input) {
            string exclude = input;
            
            for(int i = 0; i < input.Length; i++) {
                for(int j =0; j < _numbers.Length; j++) {
                    if (exclude.Contains(_numbers[j])) {
                        exclude = exclude.Replace(_numbers[j], ""); // throw out all numbers to leave just unallowed chars
                    }
                }                
            }

            for (int i = 0; i < input.Length; i++) {
                for (int j = 0; j < exclude.Length; j++) {
                    if (input.Contains(exclude[j])) {
                        input = input.Replace(exclude[j].ToString(), ""); // using the exclude, remove all unallowed chars 
                    }
                }
            }

            // return clean, number-only input
            return input;
        }
    }
}