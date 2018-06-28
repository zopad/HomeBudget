using HomeBudget.Model;
using HomeBudget.View;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Xml;

namespace HomeBudget.ViewModel.CurrencyConverter
{
    class XmlWebApiReader
    {

        public static List<Currency> ReadXml()
        {
            List<Currency> returnList = new List<Currency>();
            XmlDocument doc = new XmlDocument();
            string url = "http://api.napiarfolyam.hu";  //"api.napiarfolyam.hu.xml";//?valuta=eur";
            try
            {
                doc.Load(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                url = "api.napiarfolyam.hu.xml";        //nem sikerült az api olvasás, használjuk a letöltött adatokat
                doc.Load(url);
                MessageBox.Show("A napiarfolyam.hu nem elérhető! Friss adatokért próbálja újra később.");
             //   StartWindow sw = new StartWindow();
             //   sw.Show();
            }

            XmlNodeList itemNodes = doc.DocumentElement.SelectNodes("/arfolyamok/valuta/item");
                
            foreach (XmlNode node in itemNodes)
            {
                string bankName = node.SelectSingleNode("bank").InnerText;
                string name = node.SelectSingleNode("penznem").InnerText;
                string date = node.SelectSingleNode("datum").InnerText;
                double buy = double.Parse(node.SelectSingleNode("vetel").InnerText.ToString(), CultureInfo.InvariantCulture);
                double sell = double.Parse(node.SelectSingleNode("eladas").InnerText.ToString(), CultureInfo.InvariantCulture);
                returnList.Add(new Currency(bankName, name, Convert.ToDecimal(buy), Convert.ToDecimal(sell), date));
            }
        return returnList;
        }

    }
}

