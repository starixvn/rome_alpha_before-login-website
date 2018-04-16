// -----------------------------------------------------------------------
// <copyright file="EncryptExtension.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Alpha.Before.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using Microsoft.VisualBasic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class EncryptExtension
    {
        public const string HEXDIGIT = "0123456789abcdef";
        private const string CfsKey = "&％#@?,:*!~^.";

        /// <summary>
        /// Pearson Encode.
        /// </summary>
        /// <param name="key">The key value.</param>
        /// <returns>Return encode value.</returns>
        public static string PearsonEncode(string key)
        {
            if (string.IsNullOrEmpty(key) || key.Length == 1)
            {
                return key;
            }

            var hashCode = 0L;
            var totalChars = key.Length;
            var keys = key.ToCharArray();

            for (var i = 0; i < totalChars; i++)
            {
                hashCode += keys[i].GetHashCode();
            }

            return hashCode.ToString();
        }

        /// <summary>
        /// One-way encryption.
        /// </summary>
        /// <param name="codeValue">The code value.</param>
        /// <returns>Return encode value.</returns>
        public static string CfsEnCode(string codeValue)
        {
            int num = 30;
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

            codeValue = num3.ToString();
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

        /// <summary>
        /// One-way encryption.
        /// </summary>
        /// <param name="codeStr">The code value.</param>
        /// <param name="number">The length of value.</param>
        /// <returns>Return encode value.</returns>
        public static string CfsEnCode(string codeStr, int number)
        {
            int num = number < 6 ? 6 : number;
            int num2 = num - codeStr.Length;
            if (num2 > 1)
            {
                for (int m = 1; m <= num2; m++)
                {
                    codeStr = string.Format("{0}{1}", codeStr, Strings.Chr(0x15));
                }
            }

            double num3 = 1.0;
            for (int i = 1; i <= num; i++)
            {
                int num5 = num + (Strings.Asc(Strings.Mid(codeStr, i, 1)) * i);
                num3 *= num5;
            }

            codeStr = num3.ToString();
            string str = string.Empty;
            for (int j = 1; j <= codeStr.Length; j++)
            {
                str = string.Format("{0}{1}", str, CfsCode(Strings.Mid(codeStr, j, 3)));
            }

            string str2 = string.Empty;
            for (int k = 20; k <= (str.Length - 0x12); k += 2)
            {
                str2 = string.Format("{0}{1}", str2, Strings.Mid(str, k, 1));
            }

            return str2;
        }

        /// <summary>
        /// DeCrypt function.
        /// </summary>
        /// <param name="strText">The code value.</param>
        /// <returns>Return DeCrypt result.</returns>
        public static string CfsDeCrypt(string strText)
        {
            try
            {
                var rgbKey = new byte[] { 0x21, 0x63, 0x54, 0x5a, 0x91, 0xac, 110, 0xcc };
                var rgbIV = Encoding.UTF8.GetBytes(CfsKey);
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                var buffer = Convert.FromBase64String(strText);
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                stream2.Write(buffer, 0, buffer.Length);
                stream2.FlushFinalBlock();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        /// <summary>
        /// EnCrypt function.
        /// </summary>
        /// <param name="strText">The code value.</param>
        /// <returns>Return EnCrypt result.</returns>
        public static string CfsEnCrypt(string strText)
        {
            try
            {
                var rgbKey = new byte[] { 0x21, 0x63, 0x54, 0x5a, 0x91, 0xac, 110, 0xcc };
                var rgbIV = Encoding.UTF8.GetBytes(CfsKey);
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                byte[] bytes = Encoding.UTF8.GetBytes(strText);
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                stream2.Write(bytes, 0, bytes.Length);
                stream2.FlushFinalBlock();
                return Convert.ToBase64String(stream.ToArray());
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public static string RemoveSpecialChars(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return str.Replace("\n", "\\n").Replace("\"", "\\\"").Replace("'", "\\'");
        }

        /// <summary>
        /// Generate a random string.
        /// </summary>
        /// <param name="length">The length of value.</param>
        /// <returns>Return result.</returns>
        public static string RandomString(int length)
        {
            return RandomString(length, false);
        }

        /// <summary>
        /// Generate a random string.
        /// </summary>
        /// <param name="length">The length of value.</param>
        /// <param name="isUseZero">Use 0 yes/no.</param>
        /// <returns>Return result.</returns>
        public static string RandomString(int length, bool isUseZero)
        {
            var chars = isUseZero
                ? "qwertyuipasdfghjklzxcvbnm123456789"
                : "qwertyuiopasdfghjklzxcvbnm0123456789";

            string rndStr = string.Empty;
            Random rnd = new Random(DateTime.Now.Second + DateTime.Now.Millisecond);

            for (int i = 0; i < length; ++i)
            {
                rndStr += chars[rnd.Next() % chars.Length];
            }

            return rndStr;
        }

        /// <summary>
        /// Generate a random number string.
        /// </summary>
        /// <param name="length">The length of value.</param>
        /// <returns>Return result.</returns>
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

        /// <summary>
        /// Hex Encode.
        /// </summary>
        /// <param name="data">The input value.</param>
        /// <returns>Return result.</returns>        
        public static string HexEncode(string data)
        {
            var b16_digits = HEXDIGIT;
            string[] b16_map = new string[256];
            for (var i = 0; i < 256; i++)
            {
                b16_map[i] = string.Concat(b16_digits[i >> 4], b16_digits[i & 15]);
            }

            int c = data.Length;
            List<string> result = new List<string>();

            for (var i = 0; i < c; i++)
            {
                result.Add(b16_map[Convert.ToInt32(data[i])]);
            }

            return string.Join(null, result);
        }

        /// <summary>
        /// Hex Decode.
        /// </summary>
        /// <param name="data">The input value.</param>
        /// <returns>Return result.</returns>
        public static string HexDecode(string data)
        {
            string b16_digits = HEXDIGIT;
            char[] b16_map = new char[256];
            for (int i = 0; i < 256; i++)
            {
                b16_map[Convert.ToInt32(string.Concat(b16_digits[i >> 4], b16_digits[i & 15]), 16)] = Convert.ToChar(i);
            }

            int c = data.Length;
            if (c % 2 != 0)
            {
                data = '0' + data;
            }

            List<char> result = new List<char>();

            for (int i = 0; i < c; i += 2)
            {
                result.Add(b16_map[Convert.ToInt32(data.Substring(i, 2), 16)]);
            }

            return new string(result.ToArray());
        }

        /// <summary>
        /// RC4 encrypt.
        /// </summary>
        /// <param name="key">The encrypt code.</param>
        /// <param name="value">The input value.</param>
        /// <returns>Return result.</returns>
        public static string RC4(string key, string value)
        {
            char[] ks = key.ToCharArray();
            char[] vs = value.ToCharArray();
            int c = key.Length;
            int d = value.Length;
            int[] s = new int[256];
            int i = 0;
            for (i = 0; i < 256; i++)
            {
                s[i] = i;
            }

            int j = 0;
            int x;
            for (i = 0; i < 256; i++)
            {
                j = (j + s[i] + (int)ks[i % c]) % 256;
                x = s[i];
                s[i] = s[j];
                s[j] = x;
            }

            i = 0;
            j = 0;
            string ct = string.Empty;
            for (int y = 0; y < d; y++)
            {
                i = (i + 1) % 256;
                j = (j + s[i]) % 256;
                x = s[i];
                s[i] = s[j];
                s[j] = x;
                ct += Convert.ToChar((int)vs[y] ^ s[(s[i] + s[j]) % 256]);
            }

            return ct;
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