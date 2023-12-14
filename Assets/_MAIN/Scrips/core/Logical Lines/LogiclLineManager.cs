using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

public class LogiclLineManager 
{
    private List<ILogicalLine> logicalLines = new List<ILogicalLine>();
    public LogiclLineManager() 
    {
        logicalLines.Add(new LL_input());
        logicalLines.Add(new LL_Choice());
    }
    public bool TryGetLogic(DIALOGUE_LINE line, out Coroutine logic)
    {
        foreach(var logicalLine in logicalLines)
        {
            if(logicalLine.Maches(line))
            {
                logic = DialogueSystem.Instance().StartCoroutine(logicalLine.Execute(line));
                return true;
            }
        }
        logic = null;
        return false;
    }
}
