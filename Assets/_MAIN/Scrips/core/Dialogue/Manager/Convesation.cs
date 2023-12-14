using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Convesation
{
    private List<string> lines = new List<string>();
    private int progress = 0;

    public Convesation(List<string> list, int progress = 0)
    {
        this.lines = list;
        this.progress = progress;
    }
    public int getProgress() {  return progress; }
    public void setProgress(int progress) {  this.progress = progress; }
    public void IncremaentProgres() { ++progress; }
    public int Count () { return lines.Count; }
    public List<string> GetLines() { return lines; }
    public string CurrentLine() {  return lines[progress]; }
    public bool HasReacheEnd() { return progress >= lines.Count; }
}
