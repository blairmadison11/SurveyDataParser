using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyDataParser
{
    public enum GenderLabel { Man, Woman, Nonbinary, Unsure, Other, DND };
    public enum OrientationLabel { Straight, Gay, Lesbian, Bisexual, Pansexual, Queer, Asexual, Unsure, Other, DND };
    public enum FrequencyLabel { Often, Sometimes, Rarely, Never };
    public enum LikelihoodLabel { MuchMore, SomewhatMore, NoDifference, SomewhatLess, MuchLess };
    public enum AgreementLabel { AgreeCompletely, AgreeSomewhat, Neither, DisagreeSomewhat, DisagreeCompletely };


    internal class Respondent
    {
        public Dictionary<string,Answer> answers = new Dictionary<string,Answer>();
        private int recordNum;

        public Respondent(int recordNum)
        {
            this.recordNum = recordNum;
        }

        public void AddAnswer(Answer answer)
        {
            answers.Add(answer.AssocQuestion.Label, answer);
        }

        public int RecordNum => recordNum;
        public int Age => this.GetAnswerValue("Q1");
        public GenderLabel Gender => ((GenderLabel)(this.GetAnswerValue("Q2") - 1));
        public bool isTrans => this.GetAnswerValue("Q3") == 1;
        public OrientationLabel Orientation => ((OrientationLabel)(this.GetAnswerValue("Q4") - 1));
        public bool isLGBTQ
        {
            get
            {
                bool val = false;
                int Q4 = this.GetAnswerValue("Q4");
                int Q5 = this.GetAnswerValue("Q5");
                return (Gender == GenderLabel.Nonbinary || this.isTrans || (Q4 >= 2 && Q4 <= 7) || Q5 == 1) && (Q5 != 2);
            }
        }

        public bool isInKeyState
        {
            get
            {
                int Q28 = this.GetAnswerValue("Q28");
                return (Q28 == 1 || Q28 == 3 || Q28 == 4 ||
                    Q28 == 9 || Q28 == 10 || Q28 == 12 ||
                    Q28 == 14 || Q28 == 15 || Q28 == 16 ||
                    Q28 == 17 || Q28 == 24 || Q28 == 25 ||
                    Q28 == 26 || Q28 == 27 || Q28 == 34 ||
                    Q28 == 36 || Q28 == 40 || Q28 == 41 ||
                    Q28 == 42 || Q28 == 43 || Q28 == 44 ||
                    Q28 == 49 || Q28 == 51);
            }
        }

        public bool isBetweenAge(int min, int max)
        {
            return Age >= min && Age <= max;
        }

        public Answer GetAnswer(string label)
        {
            if (answers.ContainsKey(label))
            {
                return answers[label];
            }
            else
            {
                return null;
            }
        }

        public int GetAnswerValue(string label)
        {
            int val = 0;
            Answer answer = GetAnswer(label);
            if (answer != null)
            {
                if (answer is AnswerNumeric)
                {
                    val = ((AnswerNumeric)answer).NumericValue;
                }
                else if (answer is AnswerSelect)
                {
                    val = ((AnswerSelect)answer).NumericValue;
                }
            }
            return val;
        }

        public string GetAnswerText(string label)
        {
            string val = "";
            Answer answer = GetAnswer(label);
            if (answer != null)
            {
                if (answer is AnswerText)
                {
                    val = ((AnswerText)answer).TextValue;
                }
            }
            return val;
        }

        public override string? ToString()
        {
            StringBuilder sb = new StringBuilder();
            Question currentMulti = null;
            foreach (Answer answer in answers.Values)
            {
                Question q = answer.AssocQuestion;
                if (q.HasParent)
                {
                    if (currentMulti != q.Parent)
                    {
                        currentMulti = q.Parent;
                        sb.AppendLine();
                        sb.AppendLine(q.Parent.ToString());
                        sb.AppendLine();
                    }
                    
                    if (answer is AnswerSelect)
                    {
                        AnswerSelect ansSel = (AnswerSelect) answer;
                        if (ansSel.IsIgnorable())
                        {
                            // do nothing
                        }
                        else if (ansSel.IsLabelOnly())
                        {
                            sb.AppendLine(String.Format("\t{0}", q.Text));
                        }
                        else
                        {
                            sb.AppendLine(String.Format("\t{0}", answer.AssocQuestion.ToString()));
                            sb.AppendLine(String.Format("\t\t{0}\n", answer.ToString()));
                        }    
                    }
                    else if (answer is AnswerText)
                    {
                        sb.AppendLine(String.Format("\t{0}", answer.ToString()));
                    }
                }
                else
                {
                    sb.AppendLine();
                    sb.AppendLine(answer.AssocQuestion.ToString());
                    sb.AppendLine(String.Format("\t{0}", answer.ToString()));
                }
                
            }
            return sb.ToString();
        }
    }
}
