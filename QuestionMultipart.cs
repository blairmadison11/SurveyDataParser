using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyDataParser
{
    internal class QuestionMultipart : Question
    {
        private Dictionary<string, Question> rows = new Dictionary<string, Question>();
        private bool isMultiSelect = false;

        public QuestionMultipart(string label, string text) : base(label, text)
        {
        
        }

        public void AddRow(Question q)
        {
            q.SetParent(this);
            rows.Add(q.Label, q);
        }

        public Question GetRow(string label)
        {
            return rows[label];
        }

        public string[] GetRowKeys()
        {
            return rows.Keys.ToArray();
        }

        public override string GetVerboseString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("{0} (multi-part): {1}", label, text));
            foreach (Question row in rows.Values)
            {
                sb.AppendLine(String.Format("\t{0}", row.ToString()));
            }
            return sb.ToString();
        }
    }
}
