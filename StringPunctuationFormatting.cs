using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 字符串标点符号格式化，以下代码为最新版本
/// </summary>
public static class StringPunctuationFormatting
{
    static List<string> punctuations = new List<string> {
        "，",    ",",
        "！",    "!",
        "。",    ".",
        "？",    "?",
        "~",
        "+",    "-",    "*",    "/",
        "《",    "<",
        "》",    ">",
        "、",    @"\",
        ":",    "：",
        ";",    "；",
        "“",    "\'",
        "”",    "‘",    "’",

    };

    static IEnumerator FrameEnumerator(Text TextComponent)
    {
        yield return new WaitUntil(() => Alignment(TextComponent.transform) != 0);
        PunctuationFormat(TextComponent);

        //while (true)
        //{
        //    float result = Alignment(TextComponent.transform);
        //    if (result != 0)
        //    {
        //        PunctuationFormat(TextComponent);
        //        yield break;
        //    }
        //    yield return null;
        //}

    }

    /// <summary>
    /// 延迟执行标点符号处理
    /// </summary>
    public static void LateFramePunctuationFormat(this Text TextComponent)
    {
        TextComponent.StartCoroutine(FrameEnumerator(TextComponent));       
    }


    /// <summary>
    /// 标点符号格式化
    /// </summary>
    /// <param name="TextComponent">文本组件</param>
    public static void PunctuationFormat(this Text TextComponent)
    {
        //Debug.LogError("执行处理");
        string text = TextComponent.text;
        if ( text == string.Empty || text == "" || text.Length == 0)
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
            str += item;//当前组合的字符
            TextComponent.text = str;
            width = generator.GetPreferredWidth(TextComponent.text, settings) / settings.scaleFactor;//当前文本宽度
            if (width > boundWidth)//说明这次添加的字导致了换行
            {
                string line;//处理好的一行字
                if (isPunctuation(item))//判断是否为标点
                {
                    line = str.Substring(0, str.Length - 2);//将最后一个字拿给下一行
                    str = str.Substring(str.Length - 2, 2);
                }
                else
                {
                    line = str.Substring(0, str.Length - 1);
                    str = str.Substring(str.Length - 1, 1);
                }
                //再次判断最后一个字是否为标点
                lastOneIsPun(ref line, ref str);
                stringList.Add(line);
            }

            //如果是最后一次循环且没超行,
            //为了避免有和结尾一样的字时误以为结束，要判断内存地址是否相同
            if (object.ReferenceEquals(item, charList.LastOrDefault()))
            {
                stringList.Add(str);
            }
        }
        
        //以下处理单字不成行
        if (stringList.Count > 1 && stringList[stringList.Count - 1].Length <= 3) //判断最后一行字数，如果是单字的话
        {
            Debug.LogError("单子不成行");
            string newLine= stringList[stringList.Count - 2].Substring(0, stringList[stringList.Count - 2].Length - 1);
            string x= stringList[stringList.Count - 2].Substring(stringList[stringList.Count - 2].Length - 1, 1);//上一行的最后一个字
            stringList[stringList.Count - 2] = newLine;//删去一个字的新行
            stringList[stringList.Count - 1]=stringList[stringList.Count - 1].Insert(1, x);//添加给新一行
        }
        
        string endstring = string.Empty;
        for (int i = 0; i < stringList.Count; i++)
        {
            endstring += stringList[i];            

        }
        //foreach (var item in stringList)
        //{
        //    Debug.LogError(item);
        //    endstring += item;
        //}
        TextComponent.text = endstring;
    }
    /// <summary>
    /// 判断最后一个字是否为标点
    /// </summary>
    /// <returns></returns>
    static void lastOneIsPun(ref string line, ref string str)
    {
        if (isPunctuation(str[0].ToString()))//如果第一个字是标点
        {
            str = line.Substring(line.Length - 1, 1) + str;
            line = line.Substring(0, line.Length - 1);
            lastOneIsPun(ref line, ref str);
            return;
        }
        str = "\n" + str;
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
    /// 获取文本框的宽
    /// </summary>
    /// <param name="transform"></param>
    /// <returns>文本框宽度</returns>
    static float Alignment(Transform transform)
    {
        RectTransform rectTransform = transform.GetComponent<RectTransform>();        
        Debug.LogError(rectTransform.sizeDelta.x);
            return rectTransform.sizeDelta.x;

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




