using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager 
{
    public static List<string> ReadTextFile(string filePath, bool includeBlankLines = true)
    {
        if (!filePath.StartsWith('/'))
        { 
            filePath = FilePaths.root + filePath;
        }

        List<string> lines = new List<string>();

        try
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (includeBlankLines || !string.IsNullOrWhiteSpace(line)) 
                    { lines.Add(line); }
                }
            }
        }
        catch (FileNotFoundException ex)
        {
            Debug.LogError($"File not found: '{ex.FileName}'");
        }
        return lines;
    }

    //loading lines from text Assets of unity (inside resources), be it by string named file or text asset, read it and get each line inside of it
    //better for testing purposes.

    public static List<string> ReadTextAsset(string filePath, bool includeBlankLines = true)
    {

        TextAsset textAsset = Resources.Load<TextAsset>(filePath);
        if (textAsset == null) 
        { Debug.LogError($"Asset not found: '{filePath}'"); return null; }
        return ReadTextAsset(textAsset, includeBlankLines);

    }
    public static List<string> ReadTextAsset(TextAsset textAsset, bool includeBlankLines = true)
    {
        List<string> lines = new List<string>();
        using (StringReader sr = new StringReader(textAsset.text))
        {

            while (sr.Peek() > -1)
            {

                string line = sr.ReadLine();
                if (includeBlankLines || !string.IsNullOrWhiteSpace(line)) 
                { lines.Add(line); }
            }
        }
        return lines;
    }
}
