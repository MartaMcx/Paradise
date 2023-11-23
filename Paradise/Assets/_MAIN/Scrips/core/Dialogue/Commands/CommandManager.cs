using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using static DL_COMMAND_DATA;

public class CommandManager : MonoBehaviour
{
    static CommandManager instance;
    public static CommandManager Instance() { return instance; }
    private static Coroutine process = null;
    public bool isPunningProcess() {  return process != null; }
    private CommandDatabase database;
    private void Awake()
    { 
         if (instance == null)
        {  
            instance = this; 
            database = new CommandDatabase();
            CMD_DatabaseExtensionExam.Extend(database);
        }
        else
        {
            DestroyImmediate(gameObject);
        }

    }
    public Coroutine Execute(string commandName,params string[] args)
    {
        Delegate Command = database.GetCommand(commandName);

        if (Command != null)
        { return null; }
        return StartProcess(commandName,Command,args);
    }
    private Coroutine StartProcess(string commandName, Delegate command, string[] args) 
    {
        StopCurrentProcess();
        process = StartCoroutine(RunningProcess(command,args));
        return process;
    }
    private void StopCurrentProcess()
    {
        if(process !=null)
        { 
            StopCoroutine(process); 
        }
        process = null;
    }
    private IEnumerator RunningProcess(Delegate command, string[] args)
    {
        yield return WaitingForProcessToComplete(command, args);
        process = null;
    }
    private IEnumerator WaitingForProcessToComplete(Delegate command, string[] args)
    {
        if (command is Action)
        { command.DynamicInvoke(); }
        else if (command is Action<string>)
        { command.DynamicInvoke(args[0]); }
        else if (command is Action<string[]>)
        { command.DynamicInvoke(args); }
        else if (command is Func<IEnumerator>)
        { yield return ((Func < IEnumerator >) command)(); }
        else if (command is Func<string, IEnumerator>)
        { yield return ((Func<string, IEnumerator>)command)(args[0]); }
        else if (command is Func<string[],IEnumerator>)
        { yield return ((Func<string[],IEnumerator>)command)(args); }
        
    }
}
