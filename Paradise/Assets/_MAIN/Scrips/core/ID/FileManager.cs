using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager 
{
    public static List<string> ReadTextFile(string filePath, bool includeBlankLines = false)
    {
        if (!filePath.StartsWith('/')) { filePath = FilePaths.root + filePath; } //if filePath isn't an absolute route, make the route start from game's root

        List<string> textLines = new List<string>();


        //StreamReader reads the entirety of a file line per line, of the specified file
        //if blanks counts as lines, they are included into the list of valid lines, textlines

        try
        {
            using (StreamReader streamreader = new StreamReader(filePath))
            {
                while (!streamreader.EndOfStream)
                {
                    string line = streamreader.ReadLine();
                    if (includeBlankLines || !string.IsNullOrWhiteSpace(line)) { textLines.Add(line); }
                }
            }
        }
        catch (FileNotFoundException exception)
        {
            Debug.LogError($"File not found: '{exception.FileName}'");
        }
        return textLines;
    }

    //loading lines from text Assets of unity (inside resources), be it by string named file or text asset, read it and get each line inside of it
    //better for testing purposes.

    public static List<string> ReadTextAssetFromString(string filePath, bool includeBlankLines = false)
    {

        TextAsset textAsset = Resources.Load<TextAsset>(filePath);
        if (textAsset == null) { Debug.LogError($"Asset not found: '{filePath}'"); return null; }
        return ReadTextAsset(textAsset, includeBlankLines);

    }
    public static List<string> ReadTextAsset(TextAsset textAsset, bool includeBlankLines = false)
    {
        List<string> textLines = new List<string>();
        using (StringReader stringreader = new StringReader(textAsset.text))
        {

            while (stringreader.Peek() > -1)
            {

                string line = stringreader.ReadLine();
                if (includeBlankLines || !string.IsNullOrWhiteSpace(line)) { textLines.Add(line); }
            }
        }
        return textLines;
    }
}
