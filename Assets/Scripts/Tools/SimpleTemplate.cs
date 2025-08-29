using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// 用法：SimpleTemplate.Format(template, vars)
// 支援：{name}、{num:000}、用 {{ 與 }} 輸出大括號字元
public static class SimpleTemplate
{
    // {token[:format]}，避免吃到 {{ 或 }}
    static readonly Regex Token = new(@"(?<!\{)\{([a-zA-Z0-9_]+)(?::([^}]+))?\}(?!\})",
        RegexOptions.Compiled);

    public static string Format(string template, IReadOnlyDictionary<string, object> vars)
    {
        if (string.IsNullOrEmpty(template)) return string.Empty;

        // 暫時把 {{ }} 變成不可見代位符，避免被 regex 誤判
        const string L = "\u0001", R = "\u0002";
        template = template.Replace("{{", L).Replace("}}", R);

        string result = Token.Replace(template, m =>
        {
            string key = m.Groups[1].Value;
            string fmt = m.Groups[2].Success ? m.Groups[2].Value : null;

            if (!vars.TryGetValue(key, out var val) || val == null)
                return m.Value; // 找不到就保留原樣，方便 Debug

            // 若是數字且有 format，就套 ToString(format)
            if (fmt != null && val is IFormattable formattable)
                return formattable.ToString(fmt, System.Globalization.CultureInfo.InvariantCulture);

            // 安全起見：避免值裡面意外帶入 '<' '>'，破壞 RichText
            return EscapeRichText(val.ToString());
        });

        // 還原 {{ 與 }}
        return result.Replace(L, "{").Replace(R, "}");
    }

    // 避免變數值中含有 < 或 > 造成 TMP 標籤被破壞
    static string EscapeRichText(string s) =>
        s?.Replace("<", "&lt;").Replace(">", "&gt;");
}
