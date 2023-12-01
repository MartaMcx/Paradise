using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TagManager
{
    private  Dictionary<string, Func<string>> tags = new Dictionary<string, Func<string>>();
    private Regex tagRegex = new Regex("<\\w>");

    public TagManager() 
    {
        InitiateTag();
    }
    private void InitiateTag()
    {
        tags["<mainchar>"] = () => "Luis";
        tags["<time>"] = () => DateTime.Now.ToString("hh:mm:tt");
        tags["<playerlevel>"] =() => "15";
        tags["<temvall>"] =() => "42";
        tags["<input>"] =() => InputPanel.Instance().getLastInput();
    }
    public string Inject(string text)
    {
        if(tagRegex.IsMatch(text))
        {
            foreach(Match match in tagRegex.Matches(text))
            {
                if(tags.TryGetValue(match.Value, out var tagValueRequest))
                {
                    text = text.Replace(match.Value, tagValueRequest());
                }
            }
        }
        return text;
    }
}
