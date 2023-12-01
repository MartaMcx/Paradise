using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class DL_SPEAKER_DATA
{
    private string rawData = string.Empty;
    public string getRawText() { return rawData; }
    private string name, castName;
    public string displayname() { return castName != string.Empty ? castName : name; }
    private Vector2 castPosition;
    private List<(int layer, string expresion)> CastExpresion;

    public string getName() {  return name; } 
    public string getCastName() {  return castName; } 
    public Vector2 getCastPosition() {  return castPosition; }
    public List<(int layer, string expresion)> getCastExpresion() { return CastExpresion; }

    private string NAMECAST_ID = " as ";
    private string POSITIONCAST_ID = " at ";
    private string EXPRESSIONCAST_ID = @" [";
    private char AXISDELIMITAER = ':' ;
    private char EXPRESIONLAYER_DELIMITER = ',' ;
    public DL_SPEAKER_DATA(string rawSpeaker)
    {
        rawData= rawSpeaker;
        string pattern = @$"{NAMECAST_ID}|{POSITIONCAST_ID}|{EXPRESSIONCAST_ID.Insert(EXPRESSIONCAST_ID.Length-1, @"\")}";
        MatchCollection matches = Regex.Matches(rawSpeaker, pattern);

        castName = "";
        castPosition = Vector2.zero;
        CastExpresion = new List<(int layer, string expresion)>();

        if (matches.Count == 0)
        {
            name = rawSpeaker;
            return;
        }

        int index = matches[0].Index;
        name = rawSpeaker.Substring(0,index);
        for (int i = 0; i < matches.Count;++ i) 
        {
            Match match = matches[i];
            int startIndex =0,endIndex = 0;
            if(match.Value == NAMECAST_ID)
            {
                startIndex=match.Index+ NAMECAST_ID.Length;
                endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
            }
            else if(match.Value == POSITIONCAST_ID)
            {
                startIndex = match.Index + NAMECAST_ID.Length;
                endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                string castPos =rawSpeaker.Substring(startIndex,endIndex- startIndex);
                string[] axis = castPos.Split(AXISDELIMITAER, System.StringSplitOptions.RemoveEmptyEntries);

                float.TryParse(axis[0], out castPosition.x);
                if(axis.Length > 1) 
                {
                    float.TryParse(axis[1], out castPosition.y);
                }
            }
            else if(match.Value == EXPRESSIONCAST_ID)
            {
                startIndex = match.Index + EXPRESSIONCAST_ID.Length;
                endIndex =(i< matches.Count - 1) ? matches[i + 1].Index :rawSpeaker.Length;
                string castExp = rawSpeaker.Substring(startIndex,endIndex-startIndex);
                CastExpresion = castExp .Split(EXPRESIONLAYER_DELIMITER)
                    .Select( x=>
                    {
                        var part = x.Trim().Split(AXISDELIMITAER);
                        return (int.Parse(part[0]), part[1]);
                    }).ToList();
            }
        }
    }
}
