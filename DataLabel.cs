using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyDataParser
{
    internal class DataLabel
    {
        private string[] labels;

        public int Length => labels == null ? 0 : labels.Length;

        public DataLabel(int length)
        {
            labels = new string[length];
        }

        public void AddLabel(int num, string label)
        {
            labels[num] = label;
        }

        public string GetLabel(int num)
        {
            return labels[num];
        }
    }
}
