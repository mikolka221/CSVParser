using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace CSVParser
{
    public class CSV
    {
        //**********************
        //Fields
        //**********************
        private string FilePath { get; set; }
        private string _FileName;
        private string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                //Check if file extension is added in FileName
                if (String.IsNullOrEmpty(Path.GetExtension(value)))
                {
                    _FileName = value + ".csv";
                }
                //Check if file extension is in fact *.csv
                else if (Path.GetExtension(value).ToUpper() == ".CSV")
                {
                    _FileName = value;
                }
                else //Throw exception if both conditions fail
                    throw new ArgumentException("Input file type is of wrong kind (should be *.csv).", "FileName");
            }
        }
        public readonly List<List<String>> Data = new List<List<string>>();
        private string ListSeparator;
        private int Rows=0;
        private int Columns=0;

        //**********************
        //Constructors
        //**********************
        public CSV(string fileName)
        {
            //Check if file name contains name of the file without path
            if (String.IsNullOrEmpty(Path.GetDirectoryName(fileName)))
            {
                //File path is set to application directory
                FilePath = Directory.GetCurrentDirectory() + "\\";

                //File name is set to input file name
                FileName = fileName;
            }
            //Check if path is full path, so Path root is at the beginning
            else if (!String.IsNullOrEmpty(Path.GetPathRoot(fileName)))
            {
                FilePath = Path.GetDirectoryName(fileName) + "\\";
                FileName = Path.GetFileName(fileName);
            }
            else  //File name was entered as relative path
            {
                FilePath = Directory.GetCurrentDirectory() + Path.GetDirectoryName(fileName) + "\\";
                FileName = Path.GetFileName(fileName);
            }

            //Test to see if file exist
            if (!File.Exists(FilePath + FileName))
            {
                //ERROR - file does not exist
                throw new FileNotFoundException("CSV Error: File is missing or mistyped.", FilePath + FileName);
            }

            //Detect text speciffic regional settings
            DetectRegionalSettings();

            //Generate Data
            GenerateData();
        }

        public CSV(string fileName, string filePath)
        {
            FileName = fileName;

            //If file path is empty string, path is set to application directory
            if (!String.IsNullOrEmpty(filePath))
            {
                FilePath = Directory.GetCurrentDirectory() + "\\";
            }
            //If file path is relative, than add path to directory to it
            else if (String.IsNullOrEmpty(Path.GetPathRoot(fileName)))
            {
                FilePath = Directory.GetCurrentDirectory() + "\\" + filePath;
            }

            //Test to see if file exist
            if (!File.Exists(FilePath + FileName))
            {
                //ERROR - file does not exist
                throw new FileNotFoundException("CSV Error: File is missing or mistyped.", FilePath + FileName);
            }

            //Detect text speciffic regional settings
            DetectRegionalSettings();

            //Generate Data
            GenerateData();
        }

        public CSV(string fileName, string filePath, string listSeparator)
        {
            FileName = fileName;

            //If file path is empty string, path is set to application directory
            if (!String.IsNullOrEmpty(filePath))
            {
                FilePath = Directory.GetCurrentDirectory() + "\\";
            }
            //If file path is relative, than add path to directory to it
            else if (String.IsNullOrEmpty(Path.GetPathRoot(fileName)))
            {
                FilePath = Directory.GetCurrentDirectory() + "\\" + filePath;
            }

            //Test to see if file exist
            if (!File.Exists(FilePath + FileName))
            {
                //ERROR - file does not exist
                throw new FileNotFoundException("CSV Error: File is missing or mistyped.", FilePath + FileName);
            }

            ListSeparator = listSeparator;

            //Generate Data
            GenerateData();
        }

        //**********************
        //Methods
        //**********************
        private void DetectRegionalSettings()
        {
            System.Globalization.CultureInfo currentUserCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (String.IsNullOrEmpty(ListSeparator))
            {
                //Detect region speciffic list separator
                ListSeparator = currentUserCulture.TextInfo.ListSeparator;
            }
        }

        private void GenerateData()
        {
            try
            {
                using (var reader = new StreamReader(FilePath + FileName))
                {
                    while (!reader.EndOfStream)
                    {
                        //line is string with whole line
                        string line = reader.ReadLine();
                        //values is string array with sepparated values
                        string[] values = line.Split(ListSeparator.ToCharArray());

                        Data.Add(values.ToList());
                        if (values.Length > Columns)
                            Columns = values.Length;
                        Rows++;
                    }
                }
            }
            catch (Exception e)
            {
                throw new ParserException("CSV parser was unable to parse the file.", e);
            }
            Data.TrimExcess();
        }

        public string[,] GetStringArray()
        {
            string[,] output = new string[Rows,Columns];
            int rowCount = 0;
            foreach (List<String> Row in Data)
            {
                int colCount = 0;
                foreach (String Item in Row)
                {
                    output[rowCount, colCount] = Item;
                    colCount++;
                }
                rowCount++;
            }
            return output;
        }

        public DataTable GetDataTable()
        {
            // New table.
            DataTable table = new DataTable();

            // Add columns
            for (int i = 0; i < Columns; i++)
            {
                table.Columns.Add();
            }

            // Add rows
            foreach (List<String> Row in Data)
            {
                table.Rows.Add(Row.ToArray());
            }

            return table;

        }
    }
}
