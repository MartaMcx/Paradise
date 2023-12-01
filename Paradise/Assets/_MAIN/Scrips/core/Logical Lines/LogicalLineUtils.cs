using System.Collections;
using System.Collections.Generic;
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
        public static string REGEX_Variable_IDS = @"[!]?\$[a-zA-Z0-9_.]+";

    }
}
