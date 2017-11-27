using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Globalization;

namespace TechJobs.Models
{
    class JobData
    {
        static List<Dictionary<string, string>> AllJobs = new List<Dictionary<string, string>>();
        static bool IsDataLoaded = false;

        public static List<Dictionary<string, string>> FindAll()
        {
            LoadData();
            var allJobsCopy = new List<Dictionary<string, string>>(AllJobs); //Make a copy of the AllJobs list of dictionaries.
                                                                             //If allJobsCopy gets altered, this will NOT affect the original data.
            return AllJobs;
        }

        /*
         * Returns a list of all values contained in a given column,
         * without duplicates. 
         */
        public static List<string> FindAll(string column)
        {
            LoadData();

            List<string> values = new List<string>();
            TextInfo option = new CultureInfo("en-US", false).TextInfo;

            foreach (Dictionary<string, string> job in AllJobs)
            {
                string aValue = job[option.ToTitleCase(column)];

                if (!values.Contains(aValue))
                {
                    values.Add(aValue);
                }
            }

            values.Sort();
            return values;
        }

        /**
         * Search all columns for the given term
         */
        public static List<Dictionary<string, string>> FindByValue(string value)
        {
            // load data, if not already loaded
            LoadData();

            List<Dictionary<string, string>> jobs = new List<Dictionary<string, string>>();

            foreach (Dictionary<string, string> row in AllJobs)
            {

                foreach (string key in row.Keys)
                {
                    string aValue = row[key];

                    if (aValue.ToLower().Contains(value.ToLower()))
                    {
                        jobs.Add(row);

                        // Finding one field in a job that matches is sufficient
                        break;
                    }
                }
            }

            return jobs;
        }

        /**
         * Returns results of search the jobs data by key/value, using
         * inclusion of the search term.
         *
         * For example, searching for employer "Enterprise" will include results
         * with "Enterprise Holdings, Inc".
         */
        public static List<Dictionary<string, string>> FindByColumnAndValue(string column, string value)
        {
            // load data, if not already loaded
            LoadData();

            List<Dictionary<string, string>> jobs = new List<Dictionary<string, string>>();
            TextInfo option = new CultureInfo("en-US", false).TextInfo;

            if (value == null)
            {
                return jobs;
            }

            foreach (Dictionary<string, string> row in AllJobs)
            {
                string aValue = row[option.ToTitleCase(column)];

                if (aValue.ToLower().Contains(value.ToLower()))
                {
                    jobs.Add(row);
                }
            }

            return jobs;
        }

        /*
         * Load and parse data from job_data.csv
         */
        private static void LoadData()
        {

            if (IsDataLoaded)
            {
                return;
            }

            List<string[]> rows = new List<string[]>();
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

            using (StreamReader reader = File.OpenText("Models/job_data.csv"))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();
                    string[] rowArrray = CSVRowToStringArray(line);
                    if (rowArrray.Length > 0)
                    {
                        rows.Add(rowArrray);
                    }
                }
            }

            string[] headers = rows[0];
            rows.Remove(headers);

            for (int x = 0; x < headers.Length; x++) 
            {
                headers[x] = myTI.ToTitleCase(headers[x]);
            }

            // Parse each row array into a more friendly Dictionary
            foreach (string[] row in rows)
            {
                Dictionary<string, string> rowDict = new Dictionary<string, string>();

                for (int i = 0; i < headers.Length; i++)
                {
                    rowDict.Add(headers[i], row[i]);
                }
                AllJobs.Add(rowDict);
            }

            IsDataLoaded = true;
        }

        /*
         * Parse a single line of a CSV file into a string array
         */
        private static string[] CSVRowToStringArray(string row, char fieldSeparator = ',', char stringSeparator = '\"')
        {
            bool isBetweenQuotes = false;
            StringBuilder valueBuilder = new StringBuilder();
            List<string> rowValues = new List<string>();

            // Loop through the row string one char at a time
            foreach (char c in row.ToCharArray())
            {
                if ((c == fieldSeparator && !isBetweenQuotes))
                {
                    rowValues.Add(valueBuilder.ToString());
                    valueBuilder.Clear();
                }
                else
                {
                    if (c == stringSeparator)
                    {
                        isBetweenQuotes = !isBetweenQuotes;
                    }
                    else
                    {
                        valueBuilder.Append(c);
                    }
                }
            }

            // Add the final value
            rowValues.Add(valueBuilder.ToString());
            valueBuilder.Clear();

            return rowValues.ToArray();
        }
    }
}
