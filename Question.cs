using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyDataParser
{
    internal class Question
    {
        protected string label, text;
        protected Question parent = null;

        public string Label => label;
        public string Text => text;
        public Question Parent => parent;
        public bool HasParent => parent != null;

        public Question(string label, string text)
        {
            this.label = label;
            this.text = text;
        }

        public void SetParent(Question q)
        {
            parent = q;
        }

        public virtual string GetVerboseString()
        {
            return String.Format("{0} :: {1}", label, text);
        }

        public virtual string GetTextWithAnswer(Answer answer)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(text);
            sb.AppendLine("\t" + answer.ToString());
            return sb.ToString();
        }

        public override string? ToString()
        {
            return text;
        }
    }
}
