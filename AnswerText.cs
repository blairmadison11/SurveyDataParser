using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyDataParser
{
    internal class AnswerText : Answer
    {
        private string value;

        public string TextValue => value;

        public AnswerText(Question question, string value) : base(question)
        {
            this.value = value;
        }

        public override string? ToString()
        {
            return value;
        }
    }
}
