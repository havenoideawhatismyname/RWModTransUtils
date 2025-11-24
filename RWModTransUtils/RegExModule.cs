using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using RWModTransUtils;
using UnityEngine;

internal class RegExModule
{
    // 应用钩子
    public static void Apply()
    {
        //Debug.Log("RegExModule: Applying hooks");

        On.InGameTranslator.ctor += InGameTranslator_ctor;
        On.InGameTranslator.Translate += InGameTranslator_Translate;
        On.InGameTranslator.LoadShortStrings += InGameTranslator_LoadShortStrings;
        On.InGameTranslator.HasShortstringTranslation += InGameTranslator_HasShortstringTranslation;

        //Debug.Log("RegExModule: Hooks applied successfully");
    }

    private static bool InGameTranslator_HasShortstringTranslation(On.InGameTranslator.orig_HasShortstringTranslation orig, InGameTranslator self, string s)
    {
        //Debug.Log($"RegExModule: HasShortstringTranslation called for: '{s}'");

        // 先执行原版检查
        bool originalResult = orig(self, s);
        if (originalResult)
        {
            //Debug.Log($"RegExModule: Original translation found for: '{s}'");
            return true;
        }

        // 提前跳过正则表达式
        if (!EnableRegEX())
        {
            //Debug.Log($"RegExModule: RegEX disabled, returning false");
            // return false;
        }

        // 然后检查正则表达式匹配
        try
        {
            //Debug.Log($"RegExModule: Checking {regexTranslations.Count} regex patterns");

            if (regexTranslations.Count > 0)
            {
                foreach (var regex in regexTranslations.Keys)
                {
                    bool isMatch = regex.IsMatch(s);
                    //Debug.Log($"RegExModule: Pattern '{regex}' matches '{s}': {isMatch}");

                    if (isMatch)
                    {
                        //Debug.Log($"RegExModule: Regex match found for: '{s}' with pattern: '{regex}'");
                        return true;
                    }
                }
            }
        }
        catch (Exception e)
        {
            //Debug.LogError($"RegExModule: Error in HasShortstringTranslation: {e}");
            Debug.LogException(e);
        }

        //Debug.Log($"RegExModule: No regex match found for: '{s}'");
        return false;
    }

    private static void InGameTranslator_LoadShortStrings(On.InGameTranslator.orig_LoadShortStrings orig, InGameTranslator self)
    {
        //Debug.Log("RegExModule: LoadShortStrings called");

        // 先执行原版加载
        orig(self);

        //Debug.Log($"RegExModule: Original shortStrings count: {self.shortStrings.Count}");

        // 提前跳过正则表达式
        if (!EnableRegEX())
        {
            //Debug.Log("RegExModule: RegEX disabled, skipping regex processing");
            return;
        }

        // 然后处理正则表达式行
        try
        {
            ProcessRegexLines(self);
        }
        catch (Exception e)
        {
            //Debug.LogError($"RegExModule: Error in LoadShortStrings: {e}");
            Debug.LogException(e);
        }
    }

    private static string InGameTranslator_Translate(On.InGameTranslator.orig_Translate orig, InGameTranslator self, string s)
    {
        //Debug.Log($"RegExModule: Translate called for: '{s}'");

        // 先检查正则表达式匹配
        try
        {
            if (EnableRegEX())
            {
                UnityEngine.Debug.Log($"RegExModule: Checking {regexTranslations.Count} regex patterns for translation");

                if (regexTranslations.Count > 0)
                {
                    foreach (var regexPair in regexTranslations)
                    {
                        bool isMatch = regexPair.Key.IsMatch(s);
                        //Debug.Log($"RegExModule: Pattern '{regexPair.Key}' matches '{s}': {isMatch}");

                        if (isMatch)
                        {
                            string result = regexPair.Value;
                            //Debug.Log($"RegExModule: Regex translation found: '{s}' -> '{result}'");
                            return result;
                        }
                    }
                }
                //else
                //{
                //    Debug.Log("RegExModule: No regex patterns loaded");
                //}
            }
            //else
            //{
            //    Debug.Log("RegExModule: RegEX disabled");
            //}
        }
        catch (Exception e)
        {
            //Debug.LogError($"RegExModule: Error in Translate: {e}");
            Debug.LogException(e);
        }

        // 如果没有正则表达式匹配，回退到原版翻译
        string originalResult = orig(self, s);
        //Debug.Log($"RegExModule: Using original translation: '{s}' -> '{originalResult}'");
        return originalResult;
    }

    private static void InGameTranslator_ctor(On.InGameTranslator.orig_ctor orig, InGameTranslator self, RainWorld rainWorld)
    {
        //Debug.Log("RegExModule: InGameTranslator constructor called");

        orig(self, rainWorld);

        if (!EnableRegEX())
        {
            //Debug.Log("RegExModule: RegEX disabled, skipping initialization");
            return;
        }

        // 初始化正则表达式字典
        regexTranslations = new Dictionary<Regex, string>();
        //Debug.Log("RegExModule: Regex dictionary initialized");
    }

    // 处理正则表达式行
    private static void ProcessRegexLines(InGameTranslator self)
    {
        //Debug.Log("RegExModule: Processing regex lines");
        regexTranslations.Clear();

        // 遍历所有现有的翻译条目，找出正则表达式行
        var keysToRemove = new List<string>();
        var newRegexEntries = new Dictionary<Regex, string>();

        int regexCount = 0;

        foreach (var pair in self.shortStrings)
        {
            string key = pair.Key;
            string value = pair.Value;

            //Debug.Log($"RegExModule: Checking entry - Key: '{key}', Value: '{value}'");

            // 检查是否为正则表达式行
            if (key.StartsWith("[RegEX]"))
            {
                regexCount++;
                //Debug.Log($"RegExModule: Found regex entry #{regexCount}: '{key}' -> '{value}'");

                // 从原字典中移除
                keysToRemove.Add(key);

                // 处理正则表达式行
                string regexPattern = key.Substring(7); // 移除 [RegEX] 标记
                string translation = value;

                //Debug.Log($"RegExModule: Processing regex - Pattern: '{regexPattern}', Translation: '{translation}'");

                // 对模式和翻译都进行处理转义符
                string originalPattern = regexPattern;
                string originalTranslation = translation;

                regexPattern = ProcessEscapeSequences(regexPattern);
                translation = ProcessEscapeSequences(translation);

                //Debug.Log($"RegExModule: After escape processing - Pattern: '{regexPattern}' (was: '{originalPattern}'), Translation: '{translation}' (was: '{originalTranslation}')");

                try
                {
                    Regex regex = new Regex(regexPattern, RegexOptions.Compiled);
                    newRegexEntries[regex] = translation;

                    //Debug.Log($"RegExModule: Successfully compiled regex: {regexPattern} -> {translation}");
                }
                catch (ArgumentException ex)
                {
                    //Debug.LogError($"RegExModule: Invalid regex pattern '{regexPattern}': {ex.Message}");
                    // 如果正则表达式无效，保留为普通字符串匹配
                    string fallbackKey = regexPattern;
                    self.shortStrings[fallbackKey] = translation;
                    //Debug.Log($"RegExModule: Fallback to string match: '{fallbackKey}' -> '{translation}'");
                }
            }
        }

        //Debug.Log($"RegExModule: Found {regexCount} regex entries, removing {keysToRemove.Count} keys from shortStrings");

        // 移除所有已处理的正则表达式行
        foreach (string key in keysToRemove)
        {
            self.shortStrings.Remove(key);
            //Debug.Log($"RegExModule: Removed regex key: '{key}'");
        }

        // 添加新的正则表达式条目
        foreach (var pair in newRegexEntries)
        {
            regexTranslations[pair.Key] = pair.Value;
            //Debug.Log($"RegExModule: Added regex translation: '{pair.Key}' -> '{pair.Value}'");
        }

        //Debug.Log($"RegExModule: Final regexTranslations count: {regexTranslations.Count}");
        //Debug.Log($"RegExModule: Final shortStrings count: {self.shortStrings.Count}");
    }

    // 存储正则表达式翻译的字典
    private static Dictionary<Regex, string> regexTranslations = new Dictionary<Regex, string>();

    // 转义符映射表
    private static readonly Dictionary<string, string> escapeSequences = new Dictionary<string, string>
    {
        { "#SEPARATRIX#", "|" },
        { "#Environment.NewLine#", Environment.NewLine },
        { "#RainWorld.LINE#", "\r\n" },
        { "#REGEX.SEPARATRIX#", "|" },
        { "#TAB#", "\t" },
        { "#QUOTE#", "\"" },
        { "#BACKSLASH#", "\\" },
        { "#NL#", "\n" },
        { "#CR#", "\r" }
    };

    // 处理所有转义符
    private static string ProcessEscapeSequences(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        string result = input;

        //Debug.Log($"RegExModule: Processing escape sequences in: '{input}'");

        foreach (var escape in escapeSequences)
        {
            int count = CountOccurrences(result, escape.Key);
            if (count > 0)
            {
                string original = result;
                result = result.Replace(escape.Key, escape.Value);
                //Debug.Log($"RegExModule: Replaced {count} occurrence(s) of '{escape.Key}' with '{escape.Value}': '{original}' -> '{result}'");
            }
        }

        //Debug.Log($"RegExModule: Final result after escape processing: '{result}'");
        return result;
    }

    // 计算字符串中出现次数
    private static int CountOccurrences(string source, string value)
    {
        int count = 0;
        int index = 0;
        while ((index = source.IndexOf(value, index)) != -1)
        {
            index += value.Length;
            count++;
        }
        return count;
    }

    // 控制是否启用正则表达式
    public static bool EnableRegEX()
    {
        if (RemixMenu.disableRegEx.Value)
        {
            return false;
        }
        return true;
    } 
}