﻿using System.Text;
using System.Threading.Channels;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace Stroki
{
    public class Parsing 
    {
        const int minus = 45;
        const int dot = 46; // ascii point
        const int zero = 48; // ascii number start
        const int nine = 57; // ascii number end

        public static string CheckTypeSup(byte[] asciiBytes, string s, int length, string type)
        {
            int count = 0;
            for (int i = 0; i < length; i++)
                if (asciiBytes[i] >= zero && asciiBytes[i] <= nine || asciiBytes[i] == dot || asciiBytes[i] == minus)
                    if (s.Count(e => e == minus) == 1 || s.Count(e => e == dot) == 1)
                    count++;
            if (count == length && asciiBytes[0] != zero && asciiBytes[length - 1] != minus) // length - 1 тк один знак уходит на "-"
                type = "int";
            else if (count == length && asciiBytes[0] != dot && asciiBytes[length - 1] != dot && !s.Contains(' ') && s.Count(e => e == dot) == 1)
                if (s.Contains('.') || s.Contains(','))
                    type = "double";
                else
                    type = "string";
            else
                type = "string";
            return type.ToString();
        }
        public static string CheckType(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "File is Empty";
            SubStringReplace(s, ",", ".");
            string type = "";
            s = s.Trim();
            byte[] asciiBytes = Encoding.ASCII.GetBytes(s);

            if (s.ToLower() == "false" || s.ToLower() == "true")
                type = "bool";
            else if (s == "0")
                type = "int";
            else
            {
                type = CheckTypeSup(asciiBytes, s, s.Length, type);
            }
            return type.ToString();
        }
        public static int[] CharToInt(char[] letters, int[] c, int n)
        {
            for (int i = 0; i < n; i++)
                c[i] = (int)Char.GetNumericValue(letters[i]);
            return c;
        }
        public static void CharToDouble(char[] letters, string s) //доделать тк он конвертируеь запятую в -1
        {
            s = s.Replace(',', '.');
            int n = s.Length;
            int[] c = new int[n];
            string[] strings = s.Split('.');
            Console.WriteLine(string.Join(" ", strings));
            for (int i = 0; i < strings[0].Length; i++)
                Console.Write((int)Char.GetNumericValue(strings[0][i]));

            Console.Write(".");
            
            for (int i = 0; i < strings[1].Length; i++)
                Console.Write((int)Char.GetNumericValue(strings[1][i]));

            //return c;
        }
        public static dynamic Converter(string s, string type)
        {
            //создание массива char элементов и заполнение его символами из строки
            char[] letters = s.ToCharArray();
            int n = letters.GetLength(0);
            int[] c = new int[n];
        
            Console.WriteLine(string.Join(" ", letters));
            if (type == "bool")
            {
                if (s.ToLower() == "true")
                    return true;
                else if (s.ToLower() == "false") return false;
            }
            if (type == "string")
                return s;
            if (type == "int")
                CharToInt(letters, c, n);
            if (type == "double")
                CharToDouble(letters, s);

            string result = string.Join("", c);

            return result;
        }
        static int[] GetPrefix(string s)
        {
            int[] result = new int[s.Length];
            result[0] = 0;

            for (int i = 1; i < s.Length; i++)
            {
                int k = result[i - 1];
                while (s[k] != s[i] && k > 0)
                    k = result[k - 1];
                if (s[k] == s[i])
                    result[i] = k + 1;
                else
                    result[i] = 0;

            }
            return result;
        }

        static int FindSubstring(string pattern, string text)
        {
            int[] pf = GetPrefix(pattern);
            int index = 0;

            for (int i = 0; i < text.Length; i++)
            {
                while (index > 0 && pattern[index] != text[i])
                    index = pf[index - 1];
                if (pattern[index] == text[i])
                    index++;
                if (index == pattern.Length)

                    return i - index + 1;

            }

            return -1;
        }
        //основная kmp функция
        public static List<int> KMPSearch(string s, string subString)
        {
            //лист с индексами начала и конца подстрок(нужен для методов использующиъ поиск подстрок)
            List<int> subStringIndexList = new List<int>(); 

            return subStringIndexList;
        }
        //ещё один велосипед, заменяет подстрочку в строке на новую, учитывая количество вхождений подстроки(указывается пользователем)
        public static string SubStringReplace(string s,string subString,string newSubString,int occurrences = 1)
        {
            List<int> subStringIndexList = SlowSubStringSearch(s, subString);//KMPSearch(s, subString);
            if (subStringIndexList.Count/2<occurrences)
            {
                occurrences = subStringIndexList.Count/2;
            }
            
            for (int j = 0; j < occurrences * 2 - 1; j += 2)
            {
                s = SubStringDelete(s, subString, 1);
                s = SubStringInsert(s, newSubString, subStringIndexList[j]+j/2);
            }
            return s;
        }
        //Удаляет подстроку с учётом введённого количества вхождений подстроки
        public static string SubStringDelete(string s, string subString,int occurrences)
        {
            string newString;
            List<int> subStringIndexList = SlowSubStringSearch(s, subString);//KMPSearch(s, subString); некич сделай кмп сёрч
            if (subStringIndexList.Count / 2 < occurrences)
            {
                occurrences = subStringIndexList.Count / 2;
            }
            for (int j = occurrences*2 - 1; j > 0; j -= 2) 
            {
                newString = "";
                for (int i = 0; i < subStringIndexList[j - 1]; i++)
                {
                    newString += s[i];
                }
                for (int i = subStringIndexList[j]+1;i<s.Length;i++)
                {
                    newString += s[i];
                }
                s = newString;
            }
            Console.WriteLine(s);
            return s;
        }
        //велосипед(String.Insert())
        public static string SubStringInsert(string s, string subString, int InsertionIndex)
        {
            string newString = "";
            for (int i = 0;i< InsertionIndex; i++)
            {
                newString += s[i];
            }
            for (int i = 0; i < subString.Length; i++)
            {
                newString += subString[i];
            }
            for (int i = InsertionIndex; i < s.Length; i++)
            {
                newString += s[i];
            }
            return newString;
        }

        //медленный поиск начальных и конечных индексов подстрок в строке (abccab, ab - 0,1;4,5) и запись их в лист
        public static List<int> SlowSubStringSearch(string s, string subString) 
        {
            List<int> subStringIndexList = new List<int>();
            int startIndex;
            int endIndex = 0;
            bool breakFlag;
            for (int i = 0;i<s.Length;i++)
            {
                startIndex = i;
                breakFlag = false;
                for (int j = 0;j<subString.Length;j++)
                {
                    if (subString[j] != s[j+i])
                    {
                        breakFlag = true;
                        break;
                    }
                    endIndex = j+i;
                }
                if (breakFlag == false)
                {
                    subStringIndexList.Add(startIndex);
                    subStringIndexList.Add(endIndex);
                }
            }
            return subStringIndexList;
        }

    }
}