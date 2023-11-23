using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandDatabase
{
    private Dictionary<string,Delegate> database =new Dictionary<string,Delegate>();
    public bool HasCommand(string command) {  return database.ContainsKey(command); }
    public void AddCommand(string commandName, Delegate command)
    {
        if (!database.ContainsKey(commandName))
        {
            database.Add(commandName, command);
        }
        else
        {
            Debug.LogError($"Comman alserdy exists in the database {commandName}");
        }
    }
    public Delegate GetCommand(string commandName) 
    {
        if (!database.ContainsKey(commandName))
        {
            Debug.LogError($"Command '{commandName}' does not exit in databade !");
            return null;
        }
        return database[commandName];
    }
}
