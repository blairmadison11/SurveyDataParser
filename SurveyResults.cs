using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyDataParser
{
    internal class SurveyResults
    {
        private string dataPath;
        private List<Respondent> respondents = new List<Respondent>();

        public List<Respondent> Respondents => respondents;

        public SurveyResults(string dataPath)
        {
            this.dataPath = dataPath;
        }

        public void ParseResults(Survey survey)
        {
            List<string[]> resultLines = new List<string[]>();
            using (TextFieldParser parser = new TextFieldParser(new StreamReader(dataPath)))
            {
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    resultLines.Add(parser.ReadFields());
                }
            }

            string[] columns = resultLines[0];
            for (int i = 1; i < resultLines.Count; ++i)
            {
                Respondent r = new Respondent(int.Parse(resultLines[i][0]));
                for (int c = 0; c < columns.Length; ++c)
                {
                    string column = columns[c];
                    if ((column.StartsWith('Q') || column == "hQ6") && resultLines[i][c] != "")
                    {
                        Question q = survey.GetQuestion(column);
                        Answer a = new Answer(q);
                        if (q is QuestionNumeric)
                        {
                            a = new AnswerNumeric(q, int.Parse(resultLines[i][c]));
                        }
                        else if (q is QuestionSelect)
                        {
                            a = new AnswerSelect(q, int.Parse(resultLines[i][c]));
                        }
                        else if (q is QuestionOpenEnd)
                        {
                            a = new AnswerText(q, resultLines[i][c]);
                        }
                        r.AddAnswer(a);
                    }
                }

                // CHECK FOR ATTENTION QUESTION
                if (r.GetAnswerValue("Q25") == 4)
                {
                    respondents.Add(r);
                }
            }
        }

        public string GetResultsString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Respondent r in respondents)
            {
                sb.AppendLine(String.Format("Record #{0}\n-------------------------------------------------------------------", r.RecordNum));
                sb.Append(r.ToString());
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public string GetResultsStringFromCustomList(string name, List<Respondent> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("List of Respondents, " + name);
            sb.AppendLine("Count = " + list.Count);
            sb.AppendLine("-------------------------------------------------------------------\n");
            foreach (Respondent r in list)
            {
                sb.AppendLine(String.Format("Record #{0}\n-------------------------------------------------------------------", r.RecordNum));
                sb.Append(r.ToString());
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public string GetRespondentString (int index)
        {
            return respondents[index].ToString();
        }
    }
}
