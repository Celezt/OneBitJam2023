using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConvertExtensions
{
    public static int ToCharArray(this uint value, char[] buffer, int startIndex = 0)
        => ToCharArray((ulong)value, buffer, startIndex);
    public static int ToCharArray(this ulong value, char[] buffer, int startIndex = 0)
    {
        if (value == 0)
        {
            buffer[startIndex] = '0';
            return 1;
        }

        int len = 1;
        for (ulong rem = value / 10; rem > 0; rem /= 10)
            len++;

        for (int i = len - 1; i >= 0; i--)
        {
            buffer[startIndex + i] = (char)('0' + (value % 10));
            value /= 10;
        }

        return len;
    }
}
