using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyDataParser
{
    internal class QuestionNumeric : Question
    {
        protected int min, max;

        public QuestionNumeric(string label, string text, int min, int max) : base(label, text)
        {
            this.min = min;
            this.max = max;
        }

        public override string GetVerboseString()
        {
            return String.Format("{0} (numeric): {1}", label, text);
        }
    }
}
