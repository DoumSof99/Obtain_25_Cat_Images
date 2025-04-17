namespace Obtain_25_Cat_Images.Helpers {
    public static class Parser {
        public static List<string> ParseTemperament(string temperament) { 
            if(string.IsNullOrWhiteSpace(temperament)) return [];

            return temperament
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }
    }
}
