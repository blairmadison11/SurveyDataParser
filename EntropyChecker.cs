using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SurveyDataParser
{
    internal class EntropyChecker
    {
        private Survey survey;
        private SurveyResults results;

        private Dictionary<string, Dictionary<string, List<Tuple<double, double>>>> ansPairs = new Dictionary<string, Dictionary<string, List<Tuple<double, double>>>>();
        private Dictionary<string, Dictionary<string, double>> qCoefficients = new Dictionary<string, Dictionary<string, double>>();
        private List<string> qNames = new List<string>();
        private Dictionary<Respondent, double> entropyQuotients = new Dictionary<Respondent, double>();
        private Dictionary<int, double> simpleCoefficients = new Dictionary<int, double>()
        {
            { 4, -1 },
            { 3, -0.5 },
            { 2, 0 },
            { 1, 0.5 },
            { 0, 1 }
        };

        public List<Respondent> HighEntropy = new List<Respondent>();

        private string[] questionNames =
        {
            "Q14",
            "Q15",
            "Q16",
            "Q17",
            "Q18",
            "Q19",
            "Q20",
            "Q21"
        };

        public EntropyChecker(Survey survey, SurveyResults results)
        {
            this.survey = survey;
            this.results = results;
        }

        public void CalculateCoefficients()
        {
            foreach (string name in questionNames)
            {
                Question q = survey.GetQuestion(name);
                if (q is QuestionMultipart)
                {
                    foreach (string row in ((QuestionMultipart)q).GetRowKeys())
                    {
                        qNames.Add(row);
                    }
                }
            }

            foreach (string name1 in qNames)
            {
                ansPairs.Add(name1, new Dictionary<string, List<Tuple<double, double>>>());
                qCoefficients.Add(name1, new Dictionary<string, double>());
                foreach (string name2 in qNames)
                {
                    ansPairs[name1].Add(name2, new List<Tuple<double, double>>());
                    foreach (Respondent r in results.Respondents)
                    {
                        int ans1 = r.GetAnswerValue(name1);
                        int ans2 = r.GetAnswerValue(name2);
                        ansPairs[name1][name2].Add(new Tuple<double, double>(ans1, ans2));
                    }

                    //Calculate averages
                    double avgX = 0, avgY = 0, sumX = 0, sumY = 0;
                    foreach (Tuple<double, double> pair in ansPairs[name1][name2])
                    {
                        sumX += pair.Item1;
                        sumY += pair.Item2;
                    }
                    avgX = sumX / ansPairs[name1][name2].Count;
                    avgY = sumY / ansPairs[name1][name2].Count;

                    //Calculate standard deviations
                    double stdDevX = 0, stdDevY = 0;
                    sumX = 0; sumY = 0;
                    foreach (Tuple<double, double> pair in ansPairs[name1][name2])
                    {
                        sumX += Math.Pow(pair.Item1 - avgX, 2);
                        sumY += Math.Pow(pair.Item2 - avgY, 2);
                    }
                    stdDevX = Math.Sqrt(sumX / (ansPairs[name1][name2].Count - 1));
                    stdDevY = Math.Sqrt(sumY / (ansPairs[name1][name2].Count - 1));

                    //Calculate correlation coefficient
                    double stdSum = 0;
                    foreach (Tuple<double, double> pair in ansPairs[name1][name2])
                    {
                        stdSum += ((pair.Item1 - avgX) / stdDevX) * ((pair.Item2 - avgY) / stdDevY);
                    }

                    qCoefficients[name1].Add(name2, Math.Round(stdSum / ((double)(ansPairs[name1][name2].Count) - 1.0), 3, MidpointRounding.AwayFromZero));
                }
            }
        }

        public void CalculateEntropyQuotients()
        {
            foreach (Respondent r in results.Respondents)
            {
                double EQ = 0;
                foreach (string name1 in qNames)
                {
                    foreach(string name2 in qNames)
                    {
                        int ans1 = r.GetAnswerValue(name1);
                        int ans2 = r.GetAnswerValue(name2);
                        if (!(ans1 == 3 && ans2 == 3))
                        {
                            int absDiff = Math.Abs(ans1 - ans2);
                            double simpleCoef = simpleCoefficients[absDiff];
                            double coef = qCoefficients[name1][name2];
                            double eqDiff = Math.Abs(simpleCoef - coef);
                            EQ += eqDiff;
                        }                        
                    }
                }
                entropyQuotients.Add(r, EQ);
                if (EQ > 1400)
                {
                    HighEntropy.Add(r);
                }
            }
        }

        public string GetCoefficientCSV()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(',');
            sb.AppendLine(string.Join(',', qNames));

            foreach (string name1 in qNames)
            {
                sb.Append(name1);
                sb.Append(',');
                foreach (string name2 in qNames)
                {
                    sb.Append(qCoefficients[name1][name2]);
                    sb.Append(',');
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string GetEntropyCSV()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Respondent,Entropy Quotient");
            foreach (Respondent r in results.Respondents)
            {
                sb.Append("Record #");
                sb.Append(r.RecordNum);
                sb.Append(',');
                sb.Append(entropyQuotients[r]);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public string GetPairsCSV(string q1, string q2)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<Tuple<double, double>, int> counts = new Dictionary<Tuple<double, double>, int>();
            foreach (Tuple<double,double> pair in ansPairs[q1][q2])
            {
                if (counts.ContainsKey(pair))
                {
                    ++counts[pair];
                }
                else
                {
                    counts.Add(pair, 1);
                }
            }

            foreach (Tuple<double,double> pair in counts.Keys)
            {
                sb.Append(pair.Item1);
                sb.Append(",");
                sb.Append(pair.Item2);
                sb.Append(",");
                sb.Append(counts[pair]);
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
