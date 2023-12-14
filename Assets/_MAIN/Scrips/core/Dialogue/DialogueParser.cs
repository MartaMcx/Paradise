using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueParser
{
    private const string commandRegexPattern = @"[\w[\]]*[^\s]\(";

    public static DIALOGUE_LINE Parse(string rawLine)
    {

        (string speaker, string dialogue, string commands) = GetDialogueLineFields(rawLine);

       return new DIALOGUE_LINE(rawLine, speaker, dialogue, commands);
    }
    private static (string, string, string) GetDialogueLineFields(string rawLine)
    {
        string speaker = "", dialogue = "", commands = "";
        
        int dialogueStart = -1;
        int dialogueEnd = -1;
        bool isEscaped = false;

        for (int i = 0; i < rawLine.Length; ++i)
        {
            char current = rawLine[i];
            if (current == '\\')
            { 
                isEscaped = true; 
            }
            else if (current == '"' && isEscaped == false)
            {
                if (dialogueStart == -1) 
                { 
                    dialogueStart = i; 
                }
                else if (dialogueEnd == -1)
                { 
                    dialogueEnd = i; break; 
                }

            }
            else { isEscaped = false; }

        }
       
        Regex commandRegex = new Regex(commandRegexPattern);
        MatchCollection checkMatch = commandRegex.Matches(rawLine);

        int commandStart = -1;
        foreach (Match match in checkMatch)
        {
            if (match.Index < dialogueStart || match.Index > dialogueEnd)
            {
                commandStart = match.Index;
                break;
            }
        }
        if(commandStart !=-1 &&(dialogueStart==-1 && dialogueEnd==-1))
        {
            return ("", "", rawLine.Trim());
        }
        if (dialogueStart != -1 && dialogueEnd != -1 && (commandStart == -1 || commandStart > dialogueEnd))
        {
            speaker = rawLine.Substring(0, dialogueStart).Trim();
            dialogue = rawLine.Substring((dialogueStart + 1), ((dialogueEnd - dialogueStart) - 1)).Replace("\\\"", "\"");
            if (commandStart != -1)
            { 
                commands = rawLine.Substring(commandStart).Trim();
            }
        }
        else if (commandStart != -1 && dialogueStart > commandStart)
        { 
            commands = rawLine; 
        }
        else
        {
            dialogue = rawLine; 
        }

        return (speaker, dialogue, commands);
    }

}
