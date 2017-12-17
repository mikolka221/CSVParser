using CSVParser;
using System;
using System.Windows.Forms;

namespace Test
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// The program is used to show example how to use CSV parser
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            CSV csv = new CSV("TestData");
            System.Data.DataTable dataTableData = csv.GetDataTable();

            Form1 form1 = new Form1();
            form1.SetDataGridDataSource(dataTableData);

            Application.Run(form1);
            
        }
    }
}
