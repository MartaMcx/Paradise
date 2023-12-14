using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LogicalLineUtils
{
    public static class Encapsulation
    {
        private static char ENCAPSULATION_START = '{';
        private static char ENCAPSULATION_END = '}';
        public class EncapsulatedData
        {
            private List<string> lines;
            private int startingIndex;
            private int endingIndex;
            public void Iniciate(int startingIndex)
            {
                this.startingIndex = startingIndex;
                lines = new List<string>();
                endingIndex = 0;
            }
            public List<string> getLines() { return lines; }
            public void setEndingIndex(int endingIndex) { this.endingIndex = endingIndex; }
            public int getEndingIndex() { return endingIndex; }
            public void setStartingIndex(int startingIndex) { this.startingIndex = startingIndex; }
            public int getStartingIndex() { return startingIndex; }
        }
        
        public static EncapsulatedData RipEncapsulatedData(Convesation convesation, int startIndex, bool ripHeadEncap = false)
        {
            int encpsulationDepth = 0;
            EncapsulatedData data = new EncapsulatedData();
            data.Iniciate(startIndex);
            for (int i = startIndex; i < convesation.Count(); ++i)
            {
                string line = convesation.GetLines()[i];
                if (ripHeadEncap || (encpsulationDepth > 0 && !IsEncapsulatingEnd(line)))
                {
                    data.getLines().Add(line);
                }
                if (IsEncapsulatingStart(line))
                {
                    ++encpsulationDepth;
                    continue;
                }
                if (IsEncapsulatingEnd(line))
                {
                    --encpsulationDepth;
                    if (encpsulationDepth == 0)
                    {
                        data.setEndingIndex(i);
                        break;
                    }
                }
            }
            return data;
        }

        public static bool IsEncapsulatingStart(string line) { return line.Trim().StartsWith(ENCAPSULATION_START); }
        public static bool IsEncapsulatingEnd(string line) { return line.Trim().StartsWith(ENCAPSULATION_END); }
    }

    public static class Expressions
    {
        public static HashSet<string> OPERATORS = new HashSet<string>(){"+","+=","-","-=","*","*=","/","/=","="};
        public static string REGEX_ARITMATIC = @"([-+*/=]=?)";
        public static string REGEX_OPERATOR_LINE = @"^\$\w+\s*(=|\+=|-=|\*=\/=|)\s*";

        public static object CalculateValue(string[] expresPart)
        {
            List<string> operadString= new List<string>();
            List<string> operatorString= new List<string>();
            List<object> operands = new List<object>();

            for (int i = 0; i < expresPart.Length; ++i)
            {
                string part = expresPart[i].Trim();
                if (part == string.Empty)
                {
                    continue;
                }
                if (OPERATORS.Contains(part))
                { operadString.Add(part); }
                else
                {  operatorString.Add(part); }
            }
            foreach (string part in operadString) 
            {
                operands.Add(ExtractValue(part));
            }
            CalculaDivMul(operatorString, operands);
            CalculaSumRes(operatorString, operands);
            return operands[0];
        }
        private static void CalculaDivMul(List<string> operatorStrings, List<object> operands)
        {
            for (int i = 0;i < operatorStrings.Count; ++i) 
            {
                string operatorString = operatorStrings[i];

                if (operatorString =="*" || operatorString == "/")
                {
                    double leftOp= Convert.ToDouble(operands[i]);
                    double rightOp= Convert.ToDouble(operands[i+1]);

                    if (operatorString == "*")
                    {
                        operands[i] = leftOp * rightOp;
                    }
                    else
                    {
                        if (rightOp == 0)
                        {
                            if (rightOp == 0)
                            {
                                Debug.LogError("cannot divide by zero");
                                return;
                            }
                            operands[i] = leftOp / rightOp;
                        }
                    }
                    operands.RemoveAt(i + 1);
                    operatorStrings.RemoveAt(i);
                    --i;
                }
            }
        }
        private static void CalculaSumRes(List<string> operatorStrings, List<object> operands)
        {
            for (int i = 0; i < operatorStrings.Count; ++i)
            {
                string operatorString = operatorStrings[i];

                if (operatorString == "+" || operatorString == "-")
                {
                    double leftOp = Convert.ToDouble(operands[i]);
                    double rightOp = Convert.ToDouble(operands[i + 1]);

                    if (operatorString == "+")
                    {
                        operands[i] = leftOp + rightOp;
                    }
                    else
                    {
                        operands[i] = leftOp - rightOp;
                    }
                    operands.RemoveAt(i + 1);
                    operatorStrings.RemoveAt(i);
                    --i;
                }
            }

        }

        private static object ExtractValue(string value)
        {
            bool negate = false;
            if (value.StartsWith('!'))
            {
                negate = true;
                value = value.Substring(1);
            }
            if (value.StartsWith(VariableStore.VARIABLE_ID))
            {
                string variableName = value.TrimStart(VariableStore.VARIABLE_ID);
                if(!VariableStore.HasVarable(variableName))
                {
                    Debug.LogError($"Varieble {variableName} does not exist !");
                    return null;
                }
                VariableStore.TryGetValue(variableName, out object val);

                if(val is bool boolVal && negate)
                {
                    return !boolVal;
                }
                return val;
            }
            else if(value.StartsWith('\"') && value.EndsWith('\"'))
            {
                value = TagManager.InjectVaiables(value);
                return value.Trim('"');
            }
            else
            {
                if(int.TryParse(value,out int intValue))
                { return intValue; }
                
                else if(float.TryParse(value, out float floatValue))
                { return floatValue; }
                
                else if (bool.TryParse(value, out bool boolValue))
                { return negate ? !boolValue : boolValue; }
                
                else { return value; }
            }
        }
    }
}
