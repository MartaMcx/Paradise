using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions; //necessary for RegEx (Regular Expressions)
using UnityEditor;
using UnityEngine;

public class DL_DIALOGUE_DATA
{
    //delimiting the pattern of the segments
    private const string segmentPattern = @"\{[ca]\}|\{w[ca]\s\d*\.?\d*\}";

    // @ means anything inside the "" is the literal text, ie: \n, will be shown as \n ,verbatim string literal
    // \s --> space | \d --> digit
    // \d*\.?\d* --> 4.8 (example), digit(s) followed by an optional . , if theres the . , its followed by another digit(s), which is the decimal
    // \{[ca]\} --> {ca} || {c} || {a}
    // \{w[ca]\s\d*\.?\d*\} --> {w 4.8} || {wc 3.1} || {wa 1}


    //initializing list dialogSegments of structures DIALOGUE_SEGMENT, sumOfAllSegments will go here
    List<DIALOGUE_SEGMENT> dialogueSegments;
    public List<DIALOGUE_SEGMENT> GetDialogueSegments() { return dialogueSegments; }

    bool hasDialogue => dialogueSegments.Count > 0;
    public bool GetHasDialogue() { return hasDialogue; }



    //will ONLY take the dialogue section of an already parsed line
    public DL_DIALOGUE_DATA(string rawDialogue)
    {

        dialogueSegments = RipSegments(rawDialogue);

    }

    //will get each segment that follows the structure DIALOGUE_SEGMENT inside a raw line of dialogue
    public List<DIALOGUE_SEGMENT> RipSegments(string rawDialogue)
    {

        List<DIALOGUE_SEGMENT> sumOfAllSegments = new List<DIALOGUE_SEGMENT>();

        //any match found in rawDialogue using the segmentPattern pattern, save it to an ARRAY/INDEX of strings called matches
        MatchCollection matches = Regex.Matches(rawDialogue, segmentPattern);

        int lastIndex = 0;


        //Find first or only segment in the dialogue line
        DIALOGUE_SEGMENT segment = new DIALOGUE_SEGMENT();

        /*
        string dialogue;
        if (matches.Count == 0) { dialogue = rawDialogue; } //no matches were found, therefore rawDialogue is only pure dialogue
        else { dialogue = rawDialogue.Substring(0, matches[0].Index); } //will take from beginning of rawDialogue to where the match (mark) is found (only dialogue
        */

        segment.SetDIALOGUE_SEGMENT_Dialogue(matches.Count == 0 ? rawDialogue : rawDialogue.Substring(0, matches[0].Index));
        // if no matches were found, return rawDialogue as segment.dialogue,
        // if there are matches, will ONLY take from BEGINNING of rawDialogue to where the match (mark) is found. (will take only dialogue)

        segment.SetDIALOGUE_SEGMENT_DialogueMarks("none"); //the dialogue line was dialogue only, no marks, due for it being the first segment
        segment.SetDIALOGUE_SEGMENT_signalWaitDelay(0);    //no wait time
        sumOfAllSegments.Add(segment);

        if (matches.Count == 0) { return sumOfAllSegments; }  //EXIT POINT Nº1 --> RawDialogue had no marks            
        else { lastIndex = matches[0].Index; }


        //going through all segments of the line, delimited by the matches found
        for (int i = 0; i < matches.Count; ++i)
        {

            Match match = matches[i];
            segment = new DIALOGUE_SEGMENT();

            //gets the start Mark of the segment
            string matchMark = match.Value; // == {a} | {c} | {w 2} | {wa 4.35} | {wc 1}
            matchMark = matchMark.Substring(1, match.Length - 2); // substring(Start Index, length of the substring) --> {a} => "a" || {w 2.14} = "w 2.14" 
            string[] splitMark = matchMark.Split(' '); // divide the strings if theres a space, ie: "w 2.14" --> splitMark[0] = "w", splitMark[1] = "2.14"

            //segment.dialogueMarks(Enum.Parse(typeof(DIALOGUE_SEGMENT.DialogueSignals), splitMark[0])); shown in the tutorial 23:42, current line 63

            segment.SetDIALOGUE_SEGMENT_DialogueMarks(splitMark[0].ToLower()); // does the same as line 63


            //will get the delay Mark if there is, and save it as the proper value
            if (splitMark.Length > 1)
            {
                ///float.TryParse(splitMark[1], out float signalWaitDelayDecimals); //FIXED
                float.TryParse(splitMark[1], System.Globalization.NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out float signalWaitDelayDecimals);
                segment.SetDIALOGUE_SEGMENT_signalWaitDelay(signalWaitDelayDecimals);
                //var decimals = float.Parse(splitMark[1], CultureInfo.InvariantCulture.NumberFormat); // another way, ignore
            }


            //will get the dialogue segment (for 2nd segments and forward)
            int nextIndex = i + 1 < matches.Count ? matches[i + 1].Index : rawDialogue.Length; //checks if theres another future match, if there is, save its index, if not, nextindex is the endpoint of rawDialogue
            segment.SetDIALOGUE_SEGMENT_Dialogue(rawDialogue.Substring(lastIndex + match.Length, nextIndex - (lastIndex + match.Length))); //previousSegmentDialogue{a}Dialogue Line2{wa 3.4} -> segment.dialogue = "Dialogue Line2" 

            lastIndex = nextIndex; //preparing lastIndex for the next loop

            sumOfAllSegments.Add(segment);
        }
        return sumOfAllSegments;  //EXIT POINT Nº2 --> Return first and more segments found in the Rawline
    }


    //defining the structure of a dialogue segment

}
public class DIALOGUE_SEGMENT
{

    string dialogue;
    DialogueSignals DialogueMarks;
    float signalWaitDelay;
    enum DialogueSignals { none, c, a, wa, wc }
    bool appendText => (DialogueMarks == DialogueSignals.a || DialogueMarks == DialogueSignals.wa);

    //setters getters
    public void SetDIALOGUE_SEGMENT_Dialogue(string dialogue) { this.dialogue = dialogue; }
    public void SetDIALOGUE_SEGMENT_DialogueMarks(string MarkType)
    {
        switch (MarkType)
        {
            case "none":
                DialogueMarks = DialogueSignals.none; break;
            case "c":
                DialogueMarks = DialogueSignals.c; break;
            case "a":
                DialogueMarks = DialogueSignals.a; break;
            case "wa":
                DialogueMarks = DialogueSignals.wa; break;
            case "wc":
                DialogueMarks = DialogueSignals.wc; break;
        }
    }
    public string GetDialogueMark()
    {
        var DialogueMark = DialogueMarks.ToString();
        return DialogueMark;
    }

    public void SetDIALOGUE_SEGMENT_signalWaitDelay(float signalWaitDelay) { this.signalWaitDelay = signalWaitDelay; }
    public float GetSignalWaitDelay() { return signalWaitDelay; }
    public string GetSegmentDialogue() { return dialogue; }
    public bool GetSegmentAppendText() { return appendText; }

    //segment.GetDialogueMark() == "a" || segment.GetDialogueMark() == "wa"
}
