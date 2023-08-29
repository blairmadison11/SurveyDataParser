namespace SurveyDataParser
{
    internal class Program
    {
        const string SURVEY_DATA_PATH = "",
            SURVEY_RESULTS_PATH = "",
            CORRELATION_OUTPUT_PATH = "",
            ANALYSIS_OUTPUT_PATH = "",
            RECORDS_OUTPUT_PATH = "";

        const bool PRINT_CORRELATIONS = false,
            PRINT_ANALYSIS = true,
            PRINT_RECORDS = false;

        private static void Main(string[] args)
        {
            Console.WriteLine("Reading data map...");
            Survey survey = new Survey(SURVEY_DATA_PATH);
            survey.ReadMap();
            //survey.PrintQuestions();


            Console.WriteLine("Reading survey results...");
            SurveyResults results = new SurveyResults(SURVEY_RESULTS_PATH);
            results.ParseResults(survey);

            
            Analyzer analyzer = new Analyzer(survey, results);
            analyzer.CreateLists();

            if (PRINT_CORRELATIONS)
            {
                Console.WriteLine("Calculating correlations...");
                EntropyChecker ec = new EntropyChecker(survey, results);
                ec.CalculateCoefficients();
                ec.CalculateEntropyQuotients();
                using (StreamWriter writer = new StreamWriter(CORRELATION_OUTPUT_PATH))
                {
                    writer.Write(ec.GetCoefficientCSV());
                }
            }

            if (PRINT_ANALYSIS)
            {
                Console.WriteLine("Analyzing...");
                using (StreamWriter writer = new StreamWriter(ANALYSIS_OUTPUT_PATH))
                {
                    writer.Write(analyzer.GetAnalysis());
                }
            }

            if (PRINT_RECORDS)
            {
                Console.WriteLine("Printing records...");
                using (StreamWriter writer = new StreamWriter(RECORDS_OUTPUT_PATH))
                {
                    writer.Write(results.GetResultsStringFromCustomList("Total Sample", analyzer.GetList("Total Sample")));
                }
                Console.WriteLine("Done!");
            }
        }
    }
}