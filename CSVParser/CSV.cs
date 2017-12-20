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
        private char _ListSeparator= System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator[0];
        private char ListSeparator
        {
            get
            {
                return _ListSeparator;
            }
            set
            {
                if (char.IsPunctuation(value))
                    _ListSeparator = value;
            }
        }
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

            //Generate Data
            GenerateData();
        }

        public CSV(string fileName, string filePath, char listSeparator)
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
        private void GenerateData()
        {
            try
            {
                using (var reader = new StreamReader(FilePath + FileName))
                {
                    while (!reader.EndOfStream)
                    {
                        //Read single line
                        string line = ReadLine(reader);

                        //Parse single line to List of strings
                        List<string> values = ParseLine(line);
                        
                        //Add values to Data
                        Data.Add(values);

                        //Count number of Columns and Rows
                        if (values.Capacity > Columns)
                            Columns = values.Capacity;
                        Rows++;
                    }
                }
            }
            catch (Exception e)
            {
                throw new ParserException("CSV parser was unable to parse the file.", e);
            }
        }

        private List<string> ParseLine(string line)
        {
            //Idea is to go charachter by character and check if there
            //  any cell embraced with doublecolons, else new line is
            //  indicated with lineseparator
            List<string> values = new List<string>();
            bool doublecolon = false;
            int start = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (doublecolon)
                {
                    if (line[i].Equals('\"'))
                    {
                        values.Add(line.Substring(start, i - start));
                        doublecolon = false;
                        start = ++i + 1;
                    }
                }
                else if (line[i].Equals('\"'))
                {
                    start = i + 1;
                    doublecolon = true;
                }
                else if (line[i].Equals(ListSeparator))
                {
                    values.Add(line.Substring(start, i - start));
                    start = i + 1;
                }
            }
            values.TrimExcess();
            return values;
        }

        private string ReadLine(StreamReader reader)
        {
            //line is string with whole line
            string line = reader.ReadLine();
            //check if there is odd number of "
            if (AllIndexesOf(line, "\"").Count % 2 == 1)
            {
                do
                {
                    string temp = reader.ReadLine();
                    line += Environment.NewLine + temp;
                } while (AllIndexesOf(line, "\"").Count % 2 == 1);
            }
            return line;
        }

        private List<int> AllIndexesOf(string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("The string to find may not be empty","value");
            List<int> indexes = new List<int>();
            for(int index=0; ;index+=value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
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
