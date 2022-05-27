/*
 * Description:             StringExtension.cs
 * Author:                  TONYTANG
 * Create Date:             2018/08/08
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// StringExtension.cs
/// String��չ����
/// </summary>
public static class StringExtension {

    /// <summary>
    /// �ж��ַ����Ƿ�Ϊnull����""
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this string s)
    {
        return string.IsNullOrEmpty(s);
    }

    /// <summary>
    /// �Ƴ��׸��ַ�
    /// </summary>
    public static string RemoveFirstChar(this System.String str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        return str.Substring(1);
    }

    /// <summary>
    /// �Ƴ�ĩβ�ַ�
    /// </summary>
    public static string RemoveLastChar(this System.String str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        return str.Substring(0, str.Length - 1);
    }
}