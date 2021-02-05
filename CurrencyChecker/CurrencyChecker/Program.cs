using MySql.Data.MySqlClient;
using System;
using System.Data.SqlClient;
using System.Text;
using System.Xml;

namespace CurrencyChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.GetEncoding("windows-1251");
            Console.WriteLine(ReturnCurrency("USD",DateTime.Now.ToString("yyyy-MM-dd")));
            //Console.WriteLine("1");
            //GetCurrencyDictionary();
            //GetCurrencyDaily();
        }

        public static string ReturnCurrency(string currencyName, string date)
        {
            String connString = "Server=localhost;Database=stone21;User Id='root';";
            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();
            string sql = $@"SELECT value from `currency` where charcode='{currencyName}' and date='{date}'";
            MySqlCommand comm = conn.CreateCommand();
            comm.CommandText = sql;
            MySqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                return reader[0].ToString();
            }
            return null;
        }

        private static void GetCurrencyDictionary()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("http://www.cbr.ru/scripts/XML_valFull.asp");
            XmlElement xRoot = xDoc.DocumentElement;
            XmlNodeList childnodes = xRoot.SelectNodes("*[local-name()='Item']");
            String connString = "Server=localhost;Database=stone21;User Id='root';";
            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();
            foreach (XmlNode n in childnodes)
            {
                string sql = $@"INSERT INTO `currency_dictionary`(`name`, `engname`, `nominal`, `parentcode`, `iso_num_code`, `iso_char_code`) " +
                    $@"VALUES ('{n.SelectSingleNode("*[local-name()='Name']").InnerText}',
                               '{n.SelectSingleNode("*[local-name()='EngName']").InnerText}',
                               '{n.SelectSingleNode("*[local-name()='Nominal']").InnerText}',
                               '{n.SelectSingleNode("*[local-name()='ParentCode']").InnerText}',
                               '{n.SelectSingleNode("*[local-name()='ISO_Num_Code']").InnerText}',
                               '{n.SelectSingleNode("*[local-name()='ISO_Char_Code']").InnerText}')";
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = sql;
                comm.ExecuteNonQuery();
            }
            conn.Close();
        }
        private static void GetCurrencyDaily()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("http://www.cbr.ru/scripts/XML_daily.asp?date_req=" + DateTime.Now.ToShortDateString().ToString());
            XmlElement xRoot = xDoc.DocumentElement;
            XmlNodeList childnodes = xRoot.SelectNodes("*[local-name()='Valute']");
            String connString = "Server=localhost;Database=stone21;User Id='root';";
            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();
            foreach (XmlNode n in childnodes)
            {
                string sql = $@"INSERT INTO `currency`(`numcode`, `charcode`, `nominal`, `name`, `value`, `date`) " +
                    $@"VALUES ('{n.SelectSingleNode("*[local-name()='NumCode']").InnerText}',
                               '{n.SelectSingleNode("*[local-name()='CharCode']").InnerText}',
                               '{n.SelectSingleNode("*[local-name()='Nominal']").InnerText}',
                               '{n.SelectSingleNode("*[local-name()='Name']").InnerText}',
                               '{n.SelectSingleNode("*[local-name()='Value']").InnerText}',
                               '{DateTime.Now.ToString("yyyy-MM-dd")}')";
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = sql;
                comm.ExecuteNonQuery();
            }
            conn.Close();
        }


    }
}
