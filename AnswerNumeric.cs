using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyDataParser
{
    internal class AnswerNumeric : Answer
    {
        private int value;

        public override int NumericValue => value;

        public AnswerNumeric(Question question, int value) : base(question)
        {
            this.value = value;
        }

        public override string? ToString()
        {
            return value.ToString();
        }
    }
}
