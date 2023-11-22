using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueParser
{
    //defining pattern of command fields inside a line:  \w*[^\s]\(
    //   \word(NoSpace)(   --> command(

    private const string commandRegexPattern = "\\w*[^\\s]\\(";

    //get a line from the .txt and parse it using the method below, then construct the line with the correct fields
    public static DIALOGUE_LINE ParseMethod(string rawLine)
    {


        ///Debug.Log($"Parsing line - '{rawLine}'");

        //from rawLine, construct the line with each field properly delimited via GetDialogueLineFields() function
        (string speaker, string dialogue, string commands) = GetDialogueLineFields(rawLine);

        //show results of the parsing through console
        ///Debug.Log($"Speaker = '{speaker}'\nDialogue ='{dialogue}'\nCommands = '{commands}'");

        //return data type of DIALOGUE_LINE, which is the rawLine properly segmented
        return new DIALOGUE_LINE(speaker, dialogue, commands);
    }

    //from the line.txt given, arrange its correct fields
    private static (string, string, string) GetDialogueLineFields(string rawLine)
    {
        string speaker = "", dialogue = "", commands = "";



        //for dialogue field
        //check first if character is escaped, if not, then check if its beginning of "" (first "), mark it as dialogue start, then again checks " after
        //going through the whole string for that second " that is not escaped, and then mark it as dialogue end.

        int dialogueStart = -1;
        int dialogueEnd = -1;
        bool characterIsEscaped = false;

        for (int i = 0; i < rawLine.Length; ++i)
        {

            char currentCharacter = rawLine[i];
            if (currentCharacter == '\\') { characterIsEscaped = true; }
            else if (currentCharacter == '"' && characterIsEscaped == false)
            {
                if (dialogueStart == -1) { dialogueStart = i; }
                else if (dialogueEnd == -1) { dialogueEnd = i; break; }

            }
            else { characterIsEscaped = false; }

        }
        //show result of parsing for dialogue field
        //Debug.Log(rawLine.Substring(dialogueStart + 1, dialogueEnd - dialogueStart - 1));



        //identifying command field pattern (after dialogue)
        Regex commandRegex = new Regex(commandRegexPattern);
        Match checkMatch = commandRegex.Match(rawLine);

        int commandStart = -1;
        if (checkMatch.Success)
        {
            commandStart = checkMatch.Index;
            if (dialogueStart == -1 && dialogueEnd == -1) { return ("", "", rawLine.Trim()); } //if theres no dialogue, only commands in a line
        }


        //at this point, its confirmed that either dialogue or multi word argument command exist. discern between the two
        //if dialogueStart and dialogueEnd checks are hit, and commandStart has either not yet started or if so, its value is higher, posterior to dialogueEnd, then
        if (dialogueStart != -1 && dialogueEnd != -1 && (commandStart == -1 || commandStart > dialogueEnd))
        {
            //valid dialogue
            speaker = rawLine.Substring(0, dialogueStart).Trim();
            dialogue = rawLine.Substring((dialogueStart + 1), ((dialogueEnd - dialogueStart) - 1)).Replace("\\\"", "\""); // (\" to " if quotes inside dialogue)
            //valid commands inside a line that has both dialogue and commands
            if (commandStart != -1) { commands = rawLine.Substring(commandStart).Trim(); }
        }
        //only commands in line; commandStart starts on 0, it does check the "" regardless, so if the check on the first " (dialogueStart) is higher, posterior to commandStart,
        //then that means the " is from an argument inside the command
        else if (commandStart != -1 && dialogueStart > commandStart) { commands = rawLine; }

        //only speaker in line;
        else { speaker = rawLine; }


        return (speaker, dialogue, commands);
    }

}
