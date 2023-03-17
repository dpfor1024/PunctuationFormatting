using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 字符串标点符号格式化
/// </summary>
public static class StringPunctuationFormatting
{
    static List<string> punctuations = new List<string> {
        "，",
        "！",
        "。",
        "？",
        "~"
    };

    /// <summary>
    /// 标点符号格式化
    /// </summary>
    /// <param name="TextComponent">文本组件</param>
    public static void PunctuationFormat(this Text TextComponent)
    {
        string text = TextComponent.text;
        if (text == "" || text.Length == 0 || text == string.Empty)
        {
            Debug.LogError("字符串为空");
            return;
        }

        TextGenerator generator = new TextGenerator();
        TextGenerationSettings settings = CopyFrom(TextComponent.GetGenerationSettings(TextComponent.rectTransform.rect.size));//获取文本框数据

        //获取文本框宽度
        float boundWidth = Alignment(TextComponent.transform);

        List<string> charList = StringFormat(text);
        List<string> stringList = new List<string>();
        string str = string.Empty;
        float width;
        foreach (var item in charList)
        {
            str += item;
            TextComponent.text = str;
            width = generator.GetPreferredWidth(TextComponent.text, settings) / settings.scaleFactor;//当前文本宽度
            if (width > boundWidth)//说明这次添加的字导致了换行
            {
                string line;//一行字
                if (isPunctuation(item))//判断是否为标点
                {
                    line = str.Substring(0, str.Length - 2);//将最后一个字拿给下一行
                    str = "\n" + str.Substring(str.Length - 2, 2);
                }
                else
                {
                    line = str.Substring(0, str.Length - 1);
                    str = "\n" + str.Substring(str.Length - 1, 1);
                }
                stringList.Add(line);
            }

            //如果是最后一次循环且没超行,
            //为了避免有和结尾一样的字时误以为结束，要判断内存地址是否相同
            if (object.ReferenceEquals(item, charList.LastOrDefault()))
            {
                stringList.Add(str);
            }
        }
        string endstring = string.Empty;
        foreach (var item in stringList)
        {
            //Debug.LogError(item);
            endstring += item;
        }
        TextComponent.text = endstring;
    }

    static bool isPunctuation(string item)
    {
        foreach (var pun in punctuations)
        {
            if (item == pun)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 判断对齐方式
    /// </summary>
    /// <param name="transform"></param>
    /// <returns>文本框宽度</returns>
    static float Alignment(Transform transform)
    {
        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        if (rectTransform.anchorMax == Vector2.one && rectTransform.anchorMin == Vector2.zero)
        {
            return rectTransform.sizeDelta.x + transform.parent.transform.GetComponent<RectTransform>().sizeDelta.x;
        }
        if (rectTransform.anchorMax == new Vector2(0.5f, 0.5f) && rectTransform.anchorMin == new Vector2(0.5f, 0.5f))
        {
            return rectTransform.sizeDelta.x;
        }
        throw new Exception("对齐方式未知");        
    }

    /// <summary>
    /// 格式化字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns>每个字符的列表</returns>
    static List<string> StringFormat(string str)
    {
        List<string> strlist = new List<string>();
        for (int i = 0; i < str.Length; i++)
        {
            strlist.Add(str.Substring(0, 1));
            str = str.Substring(1);
            i--;
        }
        return strlist;
    }

    /// <summary>
    /// 获取Text组件配置属性
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    static TextGenerationSettings CopyFrom(TextGenerationSettings o)
    {
        return new TextGenerationSettings
        {
            font = o.font,
            color = o.color,
            fontSize = o.fontSize,
            lineSpacing = o.lineSpacing,
            richText = o.richText,
            scaleFactor = o.scaleFactor,
            fontStyle = o.fontStyle,
            textAnchor = o.textAnchor,
            alignByGeometry = o.alignByGeometry,
            resizeTextForBestFit = o.resizeTextForBestFit,
            resizeTextMinSize = o.resizeTextMinSize,
            resizeTextMaxSize = o.resizeTextMaxSize,
            updateBounds = o.updateBounds,
            verticalOverflow = o.verticalOverflow,
            horizontalOverflow = o.horizontalOverflow,
            generationExtents = o.generationExtents,
            pivot = o.pivot,
            generateOutOfBounds = o.generateOutOfBounds
        };
    }

}
