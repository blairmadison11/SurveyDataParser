using Microsoft.VisualBasic.FileIO;

namespace SurveyDataParser
{
    public enum QuestionType { Unknown, OpenEnd, Numeric, Select };

    internal class Survey
    {
        private string mapPath;
        private Dictionary<string, Question> questions;
        private List<Respondent> respondents;

        public Question[] Questions => questions.Values.ToArray();

        public Survey(string mapPath)
        {
            this.mapPath = mapPath;
            questions = new Dictionary<string, Question>();
            respondents = new List<Respondent>();
        }

        public void ReadMap()
        {
            List<string[]> mapLines = new List<string[]>();
            using (TextFieldParser parser = new TextFieldParser(new StreamReader(this.mapPath)))
            {
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    mapLines.Add(parser.ReadFields());
                }
            }

            for (int i = 0; i < mapLines.Count; ++i)
            {
                string firstPart = CleanText(mapLines[i][0]);
                if (firstPart.StartsWith("Q") || firstPart.StartsWith("[Q") || firstPart.StartsWith("[hQ6"))
                {
                    AddQuestion(mapLines, i);
                }
            }
        }

        public string CleanText(string text)
        {
            string cleanText = text.Trim();
            if ((cleanText.StartsWith('\"') && cleanText.EndsWith('\"')) || (cleanText.StartsWith('[') && cleanText.EndsWith(']')))
            {
                cleanText = cleanText.Substring(1, text.Length - 2);
            }
            return cleanText;
        }

        public void AddQuestion(List<string[]> mapLines, int index)
        {
            // search for blank line to signal end of current question
            List<string[]> questionLines = new List<string[]>();
            int endLine = index;
            bool flag = true, isMultiPart = false;
            while (flag)
            {
                flag = false;
                string[] fields = mapLines[++endLine];
                foreach (string field in fields)
                {
                    if (field.StartsWith("[Q"))
                    {
                        isMultiPart = true;
                        flag = true;
                    }
                    else if (field != "")
                    {
                        flag = true;
                    }
                }
            }

            // Get the question label and text
            string[] questionLine = mapLines[index];
            string[] questionFields = CleanText(questionLine[0]).Split(':');
            string qLabel = CleanText(questionFields[0]);
            string qText = CleanText(questionFields[1]);

            //check question type
            QuestionType qType = QuestionType.Unknown;
            string[] typeLine = mapLines[++index];
            string typeField = typeLine[0];
            Dictionary<int, string> optionLabels = new Dictionary<int, string>();
            int min = 0, max = 0;
            if (typeField.StartsWith("Open"))
            {
                qType = QuestionType.OpenEnd;
            }
            else if (typeField.StartsWith("Values"))
            {
                string[] valueFields = typeLine[0].Split(new char[]{ ':','-'},StringSplitOptions.TrimEntries);
                min = int.Parse(valueFields[1]);
                max = int.Parse(valueFields[2]);
                if (index + 2 == endLine)
                {
                    qType = QuestionType.Numeric;
                }
                else
                {
                    //determine whether numeric or selection
                    string[] labelFields = mapLines[++index];
                    int labelNum;
                    if (int.TryParse(labelFields[1], out labelNum))
                    {
                        qType = QuestionType.Select;
                        // get selection labels
                        flag = true;
                        while (flag)
                        {
                            labelFields = mapLines[index];
                            if (int.TryParse(labelFields[1], out labelNum))
                            {
                                optionLabels.Add(labelNum, labelFields[2]);
                                ++index;
                            }
                            else
                            {
                                flag = false;
                            }
                            
                        }
                    }
                    else
                    {
                        qType = QuestionType.Numeric;
                    }
                }
            }


            // Check to see if the question is single-part or multi-part
            // and add it to the list of questions
            if (isMultiPart)
            {
                QuestionMultipart qmp = new QuestionMultipart(qLabel, qText);
                for (int i = index; i < endLine; ++i)
                {
                    string[] partFields = mapLines[i];
                    if (partFields[1].StartsWith("[Q"))
                    {
                        string rowLabel = CleanText(partFields[1]);
                        string rowText = CleanText(partFields[2]);
                        switch (qType)
                        {
                            case QuestionType.Select:
                                qmp.AddRow(new QuestionSelect(rowLabel, rowText, optionLabels));
                                break;
                            case QuestionType.OpenEnd:
                                qmp.AddRow(new QuestionOpenEnd(rowLabel, rowText));
                                break;
                        }
                    }
                }
                questions.Add(qLabel, qmp);
            }
            else
            {
                switch (qType)
                {
                    case QuestionType.Select:
                        questions.Add(qLabel, new QuestionSelect(qLabel, qText, optionLabels));
                        break;
                    case QuestionType.Numeric:
                        questions.Add(qLabel, new QuestionNumeric(qLabel, qText, min, max));
                        break;
                    case QuestionType.OpenEnd:
                        questions.Add(qLabel, new QuestionOpenEnd(qLabel, qText));
                        break;
                }
            }
        }

        public Question GetQuestion(string label)
        {
            Question q = null;
            if (questions.ContainsKey(label))
            {
                q = questions[label];
            }
            else if (label.Contains('r'))
            {
                string baseLabel = label.Substring(0, label.IndexOf('r'));
                Question baseQuestion = questions[baseLabel];
                if (baseQuestion is QuestionMultipart)
                {
                    q = ((QuestionMultipart)baseQuestion).GetRow(label);
                }
            }
            return q;
        }

        public void PrintQuestions()
        {
            foreach (Question question in questions.Values)
            {
                Console.WriteLine(question.GetVerboseString());
            }
        }
    }
}
