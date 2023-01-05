using Microsoft.VisualBasic;

namespace Fanex.Alpha.Before.Extensions
{
    public static class EncryptExtension
    {
        public static string CfsEnCode(string codeValue)
        {
            const int num = 30;
            int num2 = num - codeValue.Length;
            if (num2 > 1)
            {
                for (int m = 1; m <= num2; m++)
                {
                    codeValue = string.Format("{0}{1}", codeValue, Strings.Chr(0x15));
                }
            }

            double num3 = 1.0;
            for (int i = 1; i <= num; i++)
            {
                int num5 = num + (Strings.Asc(Strings.Mid(codeValue, i, 1)) * i);
                num3 *= num5;
            }

            codeValue = num3.ToString("G15");
            string str = string.Empty;
            for (int j = 1; j <= codeValue.Length; j++)
            {
                str = string.Format("{0}{1}", str, CfsCode(Strings.Mid(codeValue, j, 3)));
            }

            string str2 = string.Empty;
            for (int k = 20; k <= (str.Length - 0x12); k += 2)
            {
                str2 = string.Format("{0}{1}", str2, Strings.Mid(str, k, 1));
            }

            return str2;
        }

        public static string RandomNumberString(int length)
        {
            string chars = "0123456789";
            string rndStr = string.Empty;
            Random rnd = new Random();

            for (int i = 0; i < length; ++i)
            {
                rndStr += chars[rnd.Next() % chars.Length];
            }

            return rndStr;
        }

        private static string CfsCode(string words)
        {
            string number = string.Empty;
            for (int i = 1; i <= words.Length; i++)
            {
                number = number + Strings.Asc(Strings.Mid(words, i, 1));
            }

            return Conversion.Hex(number);
        }
    }
}