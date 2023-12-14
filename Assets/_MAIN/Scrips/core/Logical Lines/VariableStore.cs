using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VariableStore
{
    private static string DEFAULT_DATAB_NAME = "Default";
    private static char DATAB_VARIABLE_ID = '.';
    public static string REGEX_Variable_IDS = @"[!]?\$[a-zA-Z0-9_.]+";
    public static char VARIABLE_ID = '$';


    private static Dictionary<string,Database> databases = new Dictionary<string, Database>();
    public class Database
    {
        private Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
        public Dictionary<string, Variable> GetVariables() { return variables; }
        public Database(string val)
        {
            this.name=val;
            this.variables= new Dictionary<string, Variable>();
        }
        public string name;
    }
    public abstract class Variable
    {
        public abstract object Get();
        public abstract void Set(object value);
    }
    public class Variable<T> : Variable
    {
        private T value;
        private Func<T> getter;
        private Action<T> setter;
        public Variable(T defaultValue = default, Func<T> getter =null, Action<T> setter=null)
        {
            value = defaultValue;
            if(getter == null)
            {
                this.getter = () => value;
            }
            else
            {
                this.getter = getter;
            }
            if (setter == null)
            {
                this.setter =newValue => value =newValue;
            }
            else
            {
                this.setter = setter;
            }
        }
        public override object Get()
        {
            return getter();
        }

        public override void Set(object newValue)
        {
            setter((T)newValue);
        }
    }
    private static Database defaultDatabase = databases[DEFAULT_DATAB_NAME];
    public static bool CreateDatabase(string name)
    {
        if(!databases.ContainsKey(name))
        {
            databases[name] = new Database(name);
            return true;
        }
        return false;
    }
    public static Database GetDatabase(string name) 
    {
        if(name == string.Empty)
        {
            return defaultDatabase;
        }
        if(!databases.ContainsKey(name))
        {
            CreateDatabase(name);
        }
        return databases[name];
    }

    public static bool CreateVariable<T>(string name,T defaultValue, Func<T> getter,Action<T> setter)
    {

        (string[] parts,Database db,string variableName) =ExtractInfo(name);
        if (db.GetVariables().ContainsKey(variableName)) { return false; }
        
        db.GetVariables()[variableName]=new Variable<T>(defaultValue, getter, setter);
        return true;
    }

    public static bool TryGetValue(string name, out object variable)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);
        if(!db.GetVariables().ContainsKey(variableName))
        {
            variable = null; return false;
        }
        variable = db.GetVariables()[variableName].Get();
        return true;
    }
    public static bool TrySetValue<T>(string name, T value)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);
        if (!db.GetVariables().ContainsKey(variableName))
        {
            return false;
        }
        db.GetVariables()[variableName].Set(value);
        return true;
    }
    private static (string[], Database,string) ExtractInfo(string info)
    {
        string[] parts = info.Split(DATAB_VARIABLE_ID);
        Database db= parts.Length > 1 ? GetDatabase(parts[0]):defaultDatabase;
        string variableName = parts.Length > 1 ? parts[1] : parts[0];
        return (parts,db, variableName);

    }

    public static bool HasVarable(string name) 
    {
        string[] parts = name.Split(DATAB_VARIABLE_ID);
        Database db = parts.Length > 1 ? GetDatabase(parts[0]) : defaultDatabase;
        string variableName = parts.Length > 1 ? parts[1] : parts[0];
        return db.GetVariables().ContainsKey(variableName);
    }

    public static void RemoveVariable(string name)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);
        if (db.GetVariables().ContainsKey(name))
        {
            db.GetVariables().Remove(name);
        }
    }
    public static void RemAllVar()
    {
        databases.Clear();
        databases[DEFAULT_DATAB_NAME] = new Database(DEFAULT_DATAB_NAME);
    }

    public static void PriAllDatabases()
    {
        foreach(KeyValuePair<string,Database> dbEntry in databases)
        {
            Debug.Log($"database: '{dbEntry.Key}'");
        }
    }
}
