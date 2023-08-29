using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyDataParser
{
    internal class QuestionOpenEnd : Question
    {
        public QuestionOpenEnd(string label, string text) : base(label, text)
        {

        }

        public override string GetVerboseString()
        {
            return String.Format("{0} (open end): {1}", label, text);
        }
    }
}
