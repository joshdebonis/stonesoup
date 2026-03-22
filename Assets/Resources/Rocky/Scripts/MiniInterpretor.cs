using System;
using System.Collections.Generic;
using System.Text;

public static class MiniInterpreter
{
    static readonly HashSet<string> ValidCommands = new HashSet<string>
    {
        "left","right","move","shoot","pickup","drop","use","loop","endloop"
    };

    public static List<(string cmd, int value)> Parse(string input)
    {
        var result = new List<(string, int)>();
        if (string.IsNullOrEmpty(input)) return result;

        int i = 0;
        int n = input.Length;
        int cnt=10000;

        while (i < n)
        {
            --cnt;
            if(cnt<=0)
                throw new Exception("crash");
            SkipWhitespace(input, ref i);

            string word = ReadWord(input, ref i);
            if (string.IsNullOrEmpty(word)) {
                ++i;
                continue;
            }
            if (!ValidCommands.Contains(word))
                continue;

            SkipWhitespace(input, ref i);

            int value = 1;
            int start = i;

            if (i < n && char.IsDigit(input[i]))
            {
                long number = 0;
                while (i < n && char.IsDigit(input[i]))
                {
                    number = number * 10 + (input[i] - '0');
                    if (number > int.MaxValue) break;
                    i++;
                }
                if (number > 0 && number <= int.MaxValue)
                    value = (int)number;
                else
                    i = start;
            }

            result.Add((word, value));
        }

        return result;
    }

    static void SkipWhitespace(string s, ref int i)
    {
        while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
    }

    static string ReadWord(string s, ref int i)
    {
        var sb = new StringBuilder();
        while (i < s.Length && char.IsLetter(s[i]))
        {
            sb.Append(char.ToLowerInvariant(s[i]));
            i++;
        }
        return sb.ToString();
    }
}