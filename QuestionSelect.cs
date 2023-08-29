using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyDataParser
{
    internal class QuestionSelect : Question
    {
        protected Dictionary<int, string> optionLabels;

        public Dictionary<int, string> Options => optionLabels;

        public QuestionSelect(string label, string text, Dictionary<int, string> labels) : base(label, text)
        {
            optionLabels = labels;
        }

        public string GetLabel(int index)
        {
            return optionLabels[index];
        }

        public int[] GetOptionKeys()
        {
            return optionLabels.Keys.ToArray();
        }

        public override string GetVerboseString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("{0} (select): {1}", label, text));
            foreach (int index in optionLabels.Keys)
            {
                sb.AppendLine(String.Format("\t{0}: {1}", index, optionLabels[index]));
            }
            return sb.ToString();
        }        
    }
}
