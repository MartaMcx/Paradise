using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        VariableStore.CreateDatabase("DB1");   
        VariableStore.CreateDatabase("DB2");   
        VariableStore.CreateDatabase("DB3");
        
        VariableStore.PriAllDatabases();
    }

}
