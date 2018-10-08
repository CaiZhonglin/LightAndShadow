using System.Text.RegularExpressions;
using System;

public class UnicodeUtil {
    public static string Convert(string unicodeString) {
        if (string.IsNullOrEmpty(unicodeString))
            return string.Empty;

        string outStr = unicodeString;

        Regex re = new Regex("\\\\u[0123456789abcdef]{4}", RegexOptions.IgnoreCase);
        MatchCollection mc = re.Matches(unicodeString);
        foreach (Match ma in mc) {
            outStr = outStr.Replace(ma.Value, ConverUnicodeStringToChar(ma.Value).ToString());
        }
        return outStr;
    }

    private static char ConverUnicodeStringToChar(string str) {
        char outStr = Char.MinValue;
        outStr = (char)int.Parse(str.Remove(0, 2), System.Globalization.NumberStyles.HexNumber);
        return outStr;
    }  
}