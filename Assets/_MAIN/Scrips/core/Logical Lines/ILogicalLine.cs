using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILogicalLine
{
    string keyword();
    bool Maches(DIALOGUE_LINE lINE);
    IEnumerator Execute(DIALOGUE_LINE data);
}
