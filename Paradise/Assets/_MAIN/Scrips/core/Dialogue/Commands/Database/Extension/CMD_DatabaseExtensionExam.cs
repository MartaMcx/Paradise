using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMD_DatabaseExtensionExam : CMD_DatabaseExtension
{
    new public static void Extend(CommandDatabase database)
    {
        //Add Action with no parametrers
        database.AddCommand("print", new Action(PrintDefaultMessage));
        database.AddCommand("print_1p", new Action<string>(PrintDefaultMessage));
        database.AddCommand("print_mp", new Action<string[]>(PrintDefaultLines));
        //Add Landa csin padres
        database.AddCommand("lambda", new Action(() => { Debug.Log("Printing lambda a default menssage to console."); }));
        database.AddCommand("lambda_1p", new Action<string>((arg) => { Debug.Log($"user lambda Message: '{arg}'."); }));
        database.AddCommand("lambda_mp", new Action<string[]>((args) => { Debug.Log(string.Join(", ", args)); }));
        //Add corrutine no paramters
        database.AddCommand("process", new Func<IEnumerator>(SimpleProcess));
        database.AddCommand("process_1p", new Func<string, IEnumerator>(LineProcess));
        database.AddCommand("process_mp", new Func<string[], IEnumerator>(MultilineProcess));

        //Special Example

        database.AddCommand("saveName", new Action(SavePlayer));
        database.AddCommand("moveCharDemo", new Func<string, IEnumerator>(MoveCharacter));

        //Savegame related commands
        database.AddCommand("setMoney", new Action<string>(SetMoney));
        database.AddCommand("addMoney", new Action<string>(AddMoney));
        database.AddCommand("modifyVariable", new Action<string[]>(ModifyVariable));

    }
    //palbra = decrip 
    private static void PrintDefaultMessage()
    {
        Debug.Log("Printing a default menssage to console.");
    }
    private static void PrintDefaultMessage(string message)
    {
        Debug.Log($"user Message: '{message}'.");
    }
    private static void PrintDefaultLines(string[] messages)
    {
        int i = 1;
        foreach (string message in messages)
        {
            Debug.Log($"{i++}'. '{message}'");
        }

    }
    private static IEnumerator SimpleProcess()
    {
        for (int i = 1; i <= 5; ++i)
        {
            Debug.Log($"Process Runing . . .[{i}]");
            yield return new WaitForSeconds(1);
        }
    }
    private static IEnumerator LineProcess(string line)
    {
        if (int.TryParse(line, out int num))
        {
            for (int i = 1; i <= num; ++i)
            {
                Debug.Log($"Process Runing . . .[{i}]");
                yield return new WaitForSeconds(1);
            }
        }
    }
    private static IEnumerator MultilineProcess(string[] data)
    {
        foreach (string line in data)
        {
            Debug.Log($"Process Mesage:'{line}'");
            yield return new WaitForSeconds(0.5f);
        }
    }
    private static IEnumerator MoveCharacter(string direction)
    {
        bool left = direction.ToLower() == "left";

        //Get the variables I need. This would be defined
        Transform character = GameObject.Find("Image").transform;
        float moveSpeed = 50;

        float targetx = left ? -50 : 50;
        float currentx = character.position.x;
        while (Mathf.Abs(targetx - currentx) > 0.1f)
        {
            //Debug.Log($"Moving Character to {(left ? "left" : "right")} [{currentx}/{targetx}]");
            currentx = Mathf.MoveTowards(currentx, targetx, moveSpeed * Time.deltaTime);
            character.position = new Vector3(currentx, character.position.y, character.position.z);
            yield return null;
        }
    }
    private static void SavePlayer()
    {
        /*string name = InputPanel.instance.lastInput;
        Debug.Log("Nombre jugador " + name);*/
    }


    //savegame related functions

    //[SerializeField] PlayerStatusSaveSchema PlayerStatusSaveSchema; //reference through unity editor

    private static void SetMoney(string newMoney)
    {
        int.TryParse(newMoney, out int Money);
        //PlayerStatusSaveSchema.GetVariablesDictionary()["money"] = Money;
    }
    private static void AddMoney(string moneyToAdd)
    {
        int.TryParse(moneyToAdd, out int Money);
        //PlayerStatusSaveSchema.GetVariablesDictionary()["money"] += Money;
    }

    private static void ModifyVariable(string[] values)
    {
        string variableToModify =values[0];
        int.TryParse(values[1], out int value);
        //PlayerStatusSaveSchema.GetVariablesDictionary()[variableToModify] += (Int32.Parse(value));
    }

    ///possible variables for now:
    ///money
    ///cassAffection


}


