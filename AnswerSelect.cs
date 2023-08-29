using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyDataParser
{
    internal class AnswerSelect : Answer
    {
        private int value;

        public override int NumericValue => value;

        public AnswerSelect(Question question, int value) : base(question)
        {
            this.value = value;
        }

        public bool IsIgnorable()
        {
            bool isIgnorable = false;
            string str = this.ToString();
            if (str == "Unchecked" || str == "Never")
            {
                isIgnorable = true;
            }
            return isIgnorable;
        }

        public bool IsLabelOnly()
        {
            bool isLabelOnly = false;
            string str = this.ToString();
            if (str == "Checked")
            {
                isLabelOnly = true;
            }
            return isLabelOnly;
        }

        public override string? ToString()
        {
            return ((QuestionSelect)question).GetLabel(value);
        }
    }
}
