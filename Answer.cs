using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyDataParser
{
    internal class Answer
    {
        protected Question question;

        public virtual int NumericValue => 0;

        public Question AssocQuestion => question;

        public Answer(Question question)
        {
            this.question = question;
        }
    }
}
