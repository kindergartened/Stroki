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
        /// <summary>
        /// вспомогательная функция для проверки на тип данных
        /// </summary>
        /// <param name="asciiBytes">используется для проверки на тип данных с помощью ASCII</param>
        /// <param name="s">строка</param>
        /// <param name="length">длина строки</param>
        /// <param name="type">тип строки</param>
        /// <returns></returns>
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
        /// <summary>
        /// функция проверки строки на тип данных
        /// </summary>
        /// <param name="s">строка</param>
        /// <returns></returns>
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
        /// <summary>
        /// вспомогательная функция переводящая символы в целые числа
        /// </summary>
        /// <param name="letters">массив символов,состоящий из символов строки s</param>
        /// <param name="c">массив целых чисел</param>
        /// <param name="n">длина массива символов letters</param>
        /// <returns></returns>
        public static int[] CharToInt(char[] letters, int[] c, int n)
        {
            for (int i = 0; i < n; i++)
                c[i] = (int)Char.GetNumericValue(letters[i]);
            return c;
        }
        /// <summary>
        /// посимвольно переводим в int и переворачиваем строки
        /// </summary>
        /// <param name="c1r">массив</param>
        /// <param name="l">длина целой части строки</param>
        /// <param name="l1">длина дробной части строки</param>
        static void CharToIntforDouble(int[] c1r, char[] l, int l1)
        {
            int index = l1 + 1;

            for (int i = 1; i < l1 + 1; i++)
            {
                c1r[i] = (int)Char.GetNumericValue(l[i - 1]);
            }
            for (int i = l1 + 1; i < l.Length; i++)
            {
                c1r[index] = (int)Char.GetNumericValue(l[index]);
                index++;
            }
            Array.Reverse(c1r);
        }
        /// <summary>
        /// вспомогательная функция,в которой мы получаем ответ ,конвертированный в double
        /// </summary>
        /// <param name="c1r">массив</param>
        /// <param name="minus">флаг для отрицательного числа</param>
        /// <param name="l2">длина дробной части</param>
        /// <returns></returns>
        static double Print(int[] c1r, bool minus, int l2)
        {
            double result = 0;
            int n = c1r.Length;
            for (int i = 0; i < n; i++)
                result += c1r[i] * Math.Pow(10, i);
            double ans = result / Math.Pow(10, l2);
            if (minus)
                return (ans * (-1));
            else
                return (ans);
        }
        /// <summary>
        /// основаная функция double,необходимая для конверта строки в double
        /// </summary>
        /// <param name="s">исходная строка</param>
        static void Double(string s)
        {
            s = s.Replace(',', '.');
            bool minus = false;
            if (s.Contains('-'))
                minus = true;
            s = s.Trim('-');
            char[] l = s.ToCharArray();
            string[] parts = s.Split('.');
            int l1 = parts[0].Length;
            int l2 = parts[1].Length;
            int[] c1r = new int[l.Length];
            CharToIntforDouble(c1r, l, l1);
            Print(c1r, minus, l2);
        }
       
        
        /// <summary>
        /// метод,позволяющий конвертировать строку в другой тип
        /// </summary>
        /// <param name="s">строка</param>
        /// <param name="type">тип</param>
        /// <returns></returns>
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
                Double(s);

            string result = string.Join("", c);

            return result;
        }
        /// <summary>
        /// вспомогательная фукция КМП для вычисления префикса функции
        /// </summary>
        /// <param name="s">строка</param>
        /// <returns></returns>
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
        /// <summary>
        /// эффективный алгоритм нахождения точек входа подстроки в строку
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="s">строка</param>
        /// <returns></returns>
        static int FindSubstring(string pattern, string s)
        {
            int[] pf = GetPrefix(pattern);
            int index = 0;

            for (int i = 0; i < s.Length; i++)
            {
                while (index > 0 && pattern[index] != s[i])
                    index = pf[index - 1];
                if (pattern[index] == s[i])
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
            Console.WriteLine(subStringIndexList[0]);
            s = SubStringDelete(s, subString, 1);
            s = SubStringInsert(s, newSubString, subStringIndexList[0]);
            for (int j = 2; j < occurrences * 2 - 1; j += 2)
            {
                Console.WriteLine(subStringIndexList[j]);
                s = SubStringDelete(s, subString, 1);
                s = SubStringInsert(s, newSubString, subStringIndexList[j]+(newSubString.Length- subString.Length));
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
/*
static void CharToInt(int[] c1r, char[] l, int l1)
    {
        int index = l1+1;

        for (int i = 1; i < l1+1; i++)
        {
            c1r[i] = (int)Char.GetNumericValue(l[i-1]);
        }
        for (int i = l1+1; i < l.Length; i++)
        {
            c1r[index] = (int)Char.GetNumericValue(l[index]);
            index++;
        }
        Array.Reverse(c1r);
    }

    static double Print(int[] c1r, bool minus, int l2)
    {
        double result = 0;
        int n = c1r.Length;
        for (int i = 0;i < n;i++)
            result += c1r[i] * Math.Pow(10, i);
        double ans = result / Math.Pow(10,l2);
        if (minus)
            Console.WriteLine(ans * (-1));
        else
            Console.WriteLine(ans);
        return result;
    }

    static void Main()
    {
        string s = "35345,426";
        s = s.Replace(',','.');
        bool minus = false;
        if (s.Contains('-'))
            minus = true;
        s= s.Trim('-');
        char[] l = s.ToCharArray();
        string[] parts = s.Split('.');
        int l1 = parts[0].Length;
        int l2 = parts[1].Length;
        int[] c1r = new int[l.Length];
        CharToInt(c1r,l, l1);
        Print(c1r, minus,l2);
    }
} */