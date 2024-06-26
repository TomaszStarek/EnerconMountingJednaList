﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Wiring
{
    public static class FileOperations
    {


        public static int WriteListStatusToFile(int index, List<Wire> list)
        {
           // index = System.Text.RegularExpressions.Regex.Replace(index, @"\s+", string.Empty);

            // textBox1.Text = sn;

            string sciezka = (@"memory.txt");      //definiowanieścieżki do której zapisywane logi
            //var date = DateTime.Now;
            //if (Directory.Exists(sciezka))       //sprawdzanie czy sciezka istnieje
            //{
            //    ;
            //}
            //else
            //    System.IO.Directory.CreateDirectory(sciezka); //jeśli nie to ją tworzy

            try
            {
                using (StreamWriter sw = new StreamWriter(sciezka))
                {
                    sw.WriteLine(Data.SetNumber);
                    sw.WriteLine(index);
                    foreach (var item in list)
                    {
                        sw.WriteLine($"{item.WireStatus};{item.Seconds};{item.DateOfFinish}; {item.MadeBy}") ;
                    }

                }
                File.Copy(sciezka, @$"{list[0].NameOfCabinet}_{Data.SetNumber}",true);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }

            return 1;

        }



        public static int ReadMemory(ref int index, List<List<Wire>> list, string path)
        {
            string sciezka = (path);
            int i = 0;
            try
            {
                using (StreamReader sr = new StreamReader(sciezka))
                {
                    //   ListOfWarnings.Clear();
                    double countOfProgress = 0;

                    while (sr.Peek() >= 0)
                    {
                        if(i==0)
                        {
                            Data.SetNumber = sr.ReadLine();
                        }
                        else
                        {
                            int parsedNumber = 0;
                            double parsedSeconds = 0.0;
                            DateTime parsedDateTime;
                            
                            var data = sr.ReadLine();
                            string[] splitted = {"","","", ""};
                            
                            if (data != null)
                                splitted = data.Split(";");
                            string MadeBy = "";

                            if (splitted.Length > 3)
                                MadeBy = splitted[3];


                            var IsItParseSuccess = int.TryParse(splitted[0], out parsedNumber);
                            var IsItParseSuccess2 = double.TryParse(splitted[0], out parsedSeconds);
                            var IsItParseSuccess3 = DateTime.TryParse(splitted[0], out parsedDateTime);


                            if (IsItParseSuccess)
                            {
                                if (i == 1)
                                {
                                    index = parsedNumber;
                                }

                                else if (i != 0)
                                {                                  
                                    list[index][i - 2].WireStatus = parsedNumber;
                                    if (IsItParseSuccess2)
                                        list[index][i - 2].Seconds = parsedSeconds;
                                    if (IsItParseSuccess3)
                                        list[index][i - 2].DateOfFinish = parsedDateTime;
                                    list[index][i - 2].MadeBy = MadeBy;
                                    if (parsedNumber == 1 || parsedNumber == 2)
                                        countOfProgress += 1;
                                    else if (parsedNumber == 3)
                                        countOfProgress += 2;
                                }

                            }
                            else
                                MessageBox.Show("Parse Error", "Błąd odczytu pamięci!");
                        }
                        i++;


                    }
                    sr.Close();

                    var index2 = index;
                    list[index2].ForEach(x => x.Progress = Math.Round(  (countOfProgress / (list[index2].Count * 2) * 100), 2));
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                //  ListOfScannedBarcodes.Clear();
                return 0;
            }
            return i;
        }

        public static void SaveLog(string NameOfCabinet, List<Wire> list)
        {
            try
            {
                string sciezka = "C:/tars/";      //definiowanieścieżki do której zapisywane logi
                DateTime stop = DateTime.Now;
                if (Directory.Exists(sciezka))       //sprawdzanie czy sciezka istnieje
                {
                    ;
                }
                else
                    System.IO.Directory.CreateDirectory(sciezka); //jeśli nie to ją tworzy


                using (StreamWriter sw = new StreamWriter("C:/tars/" + Data.SetNumber + "-" + NameOfCabinet + "-" + "(" + stop.Day + "-" + stop.Month + "-" + stop.Year + " " + stop.Hour + "-" + stop.Minute + "-" + stop.Second + ")" + ".Tars"))
                {
                    var computerName = System.Environment.MachineName.ToUpper();

                    //   sw.WriteLine($"S{serial}");
                    sw.WriteLine($"Numer seta:{Data.SetNumber}");
                    sw.WriteLine($"Szafa:{NameOfCabinet}");
                    sw.WriteLine($"N{computerName}");
                    foreach (var item in list)
                    {
                        sw.WriteLine($"{item.Number};{item.Nc};{item.Seconds};{item.DateOfFinish};{item.MadeBy}");
                    }
                    sw.WriteLine("[" + stop.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            catch (IOException iox)
            {
                MessageBox.Show(iox.Message);
            }
        }



}
}
