using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SurveyDataParser
{
    internal class Analyzer
    {
        private Survey survey;
        private SurveyResults surveyResults;

        private string[] poolNames = {
            "13-17 Male",
            "13-17 Female",
            "13-17 Other",
            "All Other"
        };

        private string[] listNames = {
            "Total Sample",
            "Age 13-17",
            "Age 18-24",
            "Age 25-34",
            "Age 35-44",
            "Age 45-55",
            "Age 13-35",
            "Age 36-55",
            "Male",
            "Female",
            "Nonbinary",
            "Transgender",
            "LGBTQ",
            "Non-LGBTQ",
            "Gay/Lesbian",
            "Bisexual/Pansexual",
            "Cishet white men",
            "Everyone but cishet white men",
            "Less than $25,000",
            "25,000-49,999",
            "50,000-74,999",
            "75,000-99,999",
            "$100,000+",
            "White and Non-Hispanic",
            "Non-White/Hispanic",
            "Hispanic/Latino/Spanish",
            "Black",
            "Asian/Pacific Islander",
            "Native American or Other Ethnicity",
            "PC/Console Gaming <4 hr/wk",
            "PC/Console Gaming 4-10 hr/wk",
            "PC/Console Gaming >10 hr/wk",
            "Mobile Gaming <4 hr/wk",
            "Mobile Gaming 4-10 hr/wk",
            "Mobile Gaming >10 hr/wk",
            "Play Single Player Often or Sometimes",
            "Play Cooperative Often or Sometimes",
            "Play Competitive Often or Sometimes",
            "Low Monthly Spend on Games ($10 or less)",
            "Medium Monthly Spend on Games ($11 - $40)",
            "High Monthly Spend on Games ($40+)",
            "Resident of a Key State",
            "Not Resident of a Key State",
            "Provided open-end response"
        };

        private Dictionary<string, List<Respondent>> demoPools = new Dictionary<string, List<Respondent>>();
        private Dictionary<string, double> demoWeight = new Dictionary<string, double>();
        private Dictionary<string, Dictionary<string, List<Respondent>>> demoLists = new Dictionary<string, Dictionary<string, List<Respondent>>>();

        public Analyzer(Survey survey, SurveyResults surveyResults)
        {
            this.survey = survey;
            this.surveyResults = surveyResults;
        }

        public void CreateLists()
        {
            foreach (string poolName in poolNames)
            {
                demoLists.Add(poolName, new Dictionary<string, List<Respondent>>());
                demoPools.Add(poolName, new List<Respondent>());
                foreach (string s in listNames)
                {
                    demoLists[poolName].Add(s, new List<Respondent>());
                }
            }

            demoWeight.Add("13-17 Male", 6.667);
            demoWeight.Add("13-17 Female", 6.833);
            demoWeight.Add("13-17 Other", 0.263);
            demoWeight.Add("All Other", 86.237);

            foreach (Respondent r in surveyResults.Respondents)
            {
                if (r.isBetweenAge(13, 17))
                {
                    if (r.Gender == GenderLabel.Man)
                    {
                        demoPools["13-17 Male"].Add(r);
                    }
                    else if (r.Gender == GenderLabel.Woman)
                    {
                        demoPools["13-17 Female"].Add(r);
                    }
                    else
                    {
                        demoPools["13-17 Other"].Add(r);
                    }
                }
                else
                {
                    demoPools["All Other"].Add(r);
                }
            }

            CreateListsFromPools();

        }

        private void CreateListsFromPools()
        {
            foreach (string poolName in poolNames)
            {
                foreach (Respondent respondent in demoPools[poolName])
                {
                    demoLists[poolName]["Total Sample"].Add(respondent);

                    if (respondent.Gender == GenderLabel.Man && !respondent.isLGBTQ && respondent.GetAnswerValue("Q27r1") == 1)
                    {
                        demoLists[poolName]["Cishet white men"].Add(respondent);
                    }
                    else
                    {
                        demoLists[poolName]["Everyone but cishet white men"].Add(respondent);
                    }

                    // Age
                    if (respondent.isBetweenAge(13, 17))
                    {
                        demoLists[poolName]["Age 13-17"].Add(respondent);
                    }
                    else if (respondent.isBetweenAge(18, 24))
                    {
                        demoLists[poolName]["Age 18-24"].Add(respondent);
                    }
                    else if (respondent.isBetweenAge(25, 34))
                    {
                        demoLists[poolName]["Age 25-34"].Add(respondent);
                    }
                    else if (respondent.isBetweenAge(35, 44))
                    {
                        demoLists[poolName]["Age 35-44"].Add(respondent);
                    }
                    else if (respondent.isBetweenAge(45, 55))
                    {
                        demoLists[poolName]["Age 45-55"].Add(respondent);
                    }

                    if (respondent.isBetweenAge(13, 35))
                    {
                        demoLists[poolName]["Age 13-35"].Add(respondent);
                    }
                    else if (respondent.isBetweenAge(36, 55))
                    {
                        demoLists[poolName]["Age 36-55"].Add(respondent);
                    }

                    // Gender
                    if (respondent.Gender == GenderLabel.Man)
                    {
                        demoLists[poolName]["Male"].Add(respondent);
                    }
                    else if (respondent.Gender == GenderLabel.Woman)
                    {
                        demoLists[poolName]["Female"].Add(respondent);
                    }
                    else if (respondent.Gender == GenderLabel.Nonbinary)
                    {
                        demoLists[poolName]["Nonbinary"].Add(respondent);
                    }

                    // Trans
                    if (respondent.isTrans)
                    {
                        demoLists[poolName]["Transgender"].Add(respondent);
                    }

                    if (respondent.Orientation == OrientationLabel.Lesbian || respondent.Orientation == OrientationLabel.Gay)
                    {
                        demoLists[poolName]["Gay/Lesbian"].Add(respondent);
                    }
                    else if (respondent.Orientation == OrientationLabel.Bisexual || respondent.Orientation == OrientationLabel.Pansexual)
                    {
                        demoLists[poolName]["Bisexual/Pansexual"].Add(respondent);
                    }

                    // Income
                    int Q24 = respondent.GetAnswerValue("Q24");
                    if (Q24 == 1)
                    {
                        demoLists[poolName]["Less than $25,000"].Add(respondent);
                    }
                    else if (Q24 >= 2 && Q24 <= 6)
                    {
                        demoLists[poolName]["25,000-49,999"].Add(respondent);
                    }
                    else if (Q24 == 7)
                    {
                        demoLists[poolName]["50,000-74,999"].Add(respondent);
                    }
                    else if (Q24 == 8)
                    {
                        demoLists[poolName]["75,000-99,999"].Add(respondent);
                    }
                    else if (Q24 >= 9)
                    {
                        demoLists[poolName]["$100,000+"].Add(respondent);
                    }

                    // Race/ethnicity
                    int Q26 = respondent.GetAnswerValue("Q26");
                    if (respondent.GetAnswerValue("Q27r1") == 1 && Q26 == 1)
                    {
                        demoLists[poolName]["White and Non-Hispanic"].Add(respondent);
                    }
                    if ((respondent.GetAnswerValue("Q27r1") == 0 && respondent.GetAnswerValue("Q27r97") == 0) || (Q26 >= 2 && Q26 <= 5))
                    {
                        demoLists[poolName]["Non-White/Hispanic"].Add(respondent);
                    }
                    if (Q26 >= 2 && Q26 <= 5)
                    {
                        demoLists[poolName]["Hispanic/Latino/Spanish"].Add(respondent);
                    }
                    if (respondent.GetAnswerValue("Q27r2") == 1)
                    {
                        demoLists[poolName]["Black"].Add(respondent);
                    }
                    if (respondent.GetAnswerValue("Q27r3") == 1)
                    {
                        demoLists[poolName]["Asian/Pacific Islander"].Add(respondent);
                    }
                    if (respondent.GetAnswerValue("Q27r4") == 1 || respondent.GetAnswerValue("Q27r5") == 1)
                    {
                        demoLists[poolName]["Native American or Other Ethnicity"].Add(respondent);
                    }

                    // LGBTQ
                    if (respondent.isLGBTQ)
                    {
                        demoLists[poolName]["LGBTQ"].Add(respondent);
                    }
                    else
                    {
                        demoLists[poolName]["Non-LGBTQ"].Add(respondent);
                    }
                    

                    // Game category
                    int Q10R1 = respondent.GetAnswerValue("Q10r1"),
                        Q10R2 = respondent.GetAnswerValue("Q10r2"),
                        Q10R3 = respondent.GetAnswerValue("Q10r3");
                    if (Q10R1 == 1 || Q10R1 == 2)
                    {
                        demoLists[poolName]["Play Single Player Often or Sometimes"].Add(respondent);
                    }
                    if (Q10R2 == 1 || Q10R2 == 2)
                    {
                        demoLists[poolName]["Play Cooperative Often or Sometimes"].Add(respondent);
                    }
                    if (Q10R3 == 1 || Q10R3 == 2)
                    {
                        demoLists[poolName]["Play Competitive Often or Sometimes"].Add(respondent);
                    }

                    // Montly spend
                    int Q13 = respondent.GetAnswerValue("Q13");
                    if (Q13 == 1 || Q13 == 2 || Q13 == 7)
                    {
                        demoLists[poolName]["Low Monthly Spend on Games ($10 or less)"].Add(respondent);
                    }
                    if (Q13 == 3 || Q13 == 4)
                    {
                        demoLists[poolName]["Medium Monthly Spend on Games ($11 - $40)"].Add(respondent);
                    }
                    if (Q13 == 5 || Q13 == 6)
                    {
                        demoLists[poolName]["High Monthly Spend on Games ($40+)"].Add(respondent);
                    }

                    // Key states
                    if (respondent.isInKeyState)
                    {
                        demoLists[poolName]["Resident of a Key State"].Add(respondent);
                    }
                    else
                    {
                        demoLists[poolName]["Not Resident of a Key State"].Add(respondent);
                    }

                    int mobileHours = 0, conpcHours = 0;
                    string[] rowKeys = ((QuestionMultipart)survey.GetQuestion("Q8")).GetRowKeys();
                    foreach (string key in rowKeys)
                    {
                        QuestionSelect q = (QuestionSelect)survey.GetQuestion(key);
                        if (q.Text != "Other" && q.Text != "None of these" && respondent.GetAnswerValue(key) != 99)
                        {
                            if (q.Text == "Mobile phone" || q.Text == "Tablet")
                            {
                                mobileHours += respondent.GetAnswerValue(key);
                            }
                            else
                            {
                                conpcHours += respondent.GetAnswerValue(key);
                            }
                        }

                    }

                    if (mobileHours < 4)
                    {
                        demoLists[poolName]["Mobile Gaming <4 hr/wk"].Add(respondent);
                    }
                    else if (mobileHours >= 4 && mobileHours <= 10)
                    {
                        demoLists[poolName]["Mobile Gaming 4-10 hr/wk"].Add(respondent);
                    }
                    else if (mobileHours > 10)
                    {
                        demoLists[poolName]["Mobile Gaming >10 hr/wk"].Add(respondent);
                    }

                    if (conpcHours < 4)
                    {
                        demoLists[poolName]["PC/Console Gaming <4 hr/wk"].Add(respondent);
                    }
                    else if (conpcHours >= 4 && conpcHours <= 10)
                    {
                        demoLists[poolName]["PC/Console Gaming 4-10 hr/wk"].Add(respondent);
                    }
                    else if (conpcHours > 10)
                    {
                        demoLists[poolName]["PC/Console Gaming >10 hr/wk"].Add(respondent);
                    }

                    string openEnd = respondent.GetAnswerText("Q29");
                    if (openEnd != "")
                    {
                        demoLists[poolName]["Provided open-end response"].Add(respondent);
                    }
                }
            }
        }

        public List<Respondent> GetList(string listName)
        {
            List<Respondent> list = new List<Respondent>();
            foreach (string poolName in poolNames)
            {
                foreach (Respondent r in demoLists[poolName][listName])
                {
                    list.Add(r);
                }
            }
            return list;
        }

        public string GetAnalysis()
        {
            StringBuilder sb = new StringBuilder();
            string listStr = JoinArray(listNames);

            sb.AppendLine("," + listStr);
            sb.Append("\"Sample Size\",");
            foreach(string listName in listNames)
            {
                int count = 0;
                foreach (string poolName in poolNames)
                {
                    count += demoLists[poolName][listName].Count;
                }

                sb.Append(count);
                sb.Append(",");
            }
            sb.AppendLine(",,\n,,");

            foreach (Question q in survey.Questions)
            {
                if (q is QuestionOpenEnd)
                {
                    continue;
                }

                sb.AppendLine(String.Format("\"{0} - {1}\",{2}", q.Label, q.Text, listStr));
                if (q is QuestionSelect)
                {
                    QuestionSelect qs = (QuestionSelect)q;
                    int[] optionKeys = qs.GetOptionKeys();
                    foreach (int optionKey in optionKeys)
                    {
                        sb.Append(String.Format("\"{0}\",", qs.GetLabel(optionKey)));
                        foreach (string listName in listNames)
                        {
                            double percent = 0.0, totalPercent = 0.0;
                            int combinedTotal = 0;
                            foreach (string poolName in demoPools.Keys)
                            {
                                double count = 0, total = 0;
                                foreach (Respondent r in demoLists[poolName][listName])
                                {
                                    Answer a = r.GetAnswer(q.Label);
                                    if (a != null)
                                    {
                                        ++total;
                                        ++combinedTotal;
                                        if (a.NumericValue == optionKey)
                                        {
                                            ++count;
                                        }
                                    }
                                }

                                if (total > 0)
                                {
                                    percent += (count / total) * demoWeight[poolName];
                                    totalPercent += demoWeight[poolName];
                                }
                            }

                            percent /= totalPercent;

                            if (combinedTotal > 0 && totalPercent > 0)
                            {
                                sb.Append(String.Format("{0},", percent));
                            }
                            else
                            {
                                sb.Append(',');
                            }
                        }
                        sb.AppendLine();
                    }
                }
                else if (q is QuestionNumeric)
                {
                    sb.Append("Average,");
                    QuestionNumeric qn = (QuestionNumeric)q;
                    foreach (string listName in listNames)
                    {
                        double average = 0.0, totalWeight = 0;
                        int combinedTotal = 0;
                        foreach (string poolName in demoPools.Keys)
                        {
                            double sum = 0, total = 0;
                            foreach (Respondent r in demoLists[poolName][listName])
                            {
                                ++total;
                                ++combinedTotal;
                                sum += r.GetAnswer(q.Label).NumericValue;
                            }

                            if (total > 0)
                            {
                                average += (sum / total) * demoWeight[poolName];
                                totalWeight += demoWeight[poolName];
                            }
                        }

                        average /= totalWeight;

                        if (combinedTotal > 0)
                        {
                            sb.Append(String.Format("{0},", average));
                        }
                        else
                        {
                            sb.Append(',');
                        }
                    }
                    sb.AppendLine();
                }
                else if (q is QuestionMultipart)
                {
                    QuestionMultipart qm = (QuestionMultipart)q;
                    string[] rowKeys = qm.GetRowKeys();
                    foreach (string rowKey in rowKeys)
                    {
                        Question row = qm.GetRow(rowKey);
                        if (row is QuestionSelect)
                        {
                            QuestionSelect qs = (QuestionSelect)row;
                            int[] optionKeys = qs.GetOptionKeys();
                            foreach (int optionKey in optionKeys)
                            {
                                if (qs.GetLabel(optionKey) == "Checked")
                                {
                                    sb.Append(String.Format("\"{0}\",", qs.Text));
                                }
                                else if (qs.GetLabel(optionKey) == "Unchecked")
                                {
                                    continue;
                                }
                                else
                                {
                                    sb.Append(String.Format("\"{0} - {1}\",", qs.Text, qs.GetLabel(optionKey)));
                                }
                                
                                foreach (string listName in listNames)
                                {
                                    double percent = 0.0, totalPercent = 0.0;
                                    int combinedTotal = 0;
                                    foreach (string poolName in demoPools.Keys)
                                    {
                                        double count = 0, total = 0;
                                        foreach (Respondent r in demoLists[poolName][listName])
                                        {
                                            Answer a = r.GetAnswer(qs.Label);
                                            if (a != null)
                                            {
                                                ++total;
                                                ++combinedTotal;
                                                if (a.NumericValue == optionKey)
                                                {
                                                    ++count;
                                                }
                                            }
                                        }

                                        if (total > 0)
                                        {
                                            percent += (count / total) * demoWeight[poolName];
                                            totalPercent += demoWeight[poolName];
                                        }
                                    }

                                    percent /= totalPercent;

                                    if (combinedTotal > 0 && totalPercent > 0)
                                    {
                                        sb.Append(String.Format("{0},", percent));
                                    }
                                    else
                                    {
                                        sb.Append(',');
                                    }
                                }
                                sb.AppendLine();
                            }
                        }
                    }
                    sb.AppendLine();
                }
                sb.AppendLine(",,");
            }

            return sb.ToString();
        }

        public string JoinArray(string[] array)
        {
            string[] arraycopy = new string[array.Length];
            for (int i = 0; i < array.Length; ++i)
            {
                if (array[i].Contains(' ') || array[i].Contains(','))
                {
                    arraycopy[i] = String.Format("\"{0}\"", array[i]);
                }
                else
                {
                    arraycopy[i] = array[i];
                }
            }
            return string.Join(',', arraycopy);
        }
    }
}
