using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Vml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Formats.Tar;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Printing;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;
using Image = System.Windows.Controls.Image;

namespace Wiring
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Data myData = new Data();

        private Wire _wire = new Wire();
        private int _findedCabinetIndex = 0;
        public static MainWindow MyWindow { get; private set; }
        private static List<string> ListOfNames = new List<string>();

        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            MyWindow = this;
            LoadDataFromExcel(); //pobieranie danych z listy excel
            FileOperations.ReadMemory(ref _findedCabinetIndex, myData.ListOfImportedCabinets, @"memory.txt"); // czytanie danych na temat ostatniej robionej szafy

            listView.ItemsSource = myData.ListOfImportedCabinets[_findedCabinetIndex]; //wyświetlanie danych z listy jako listview

            MoveDownSelectedItemFromList(listView); //odświeżenie wyświerlanych danych na aplikacji

            Dispatcher.Invoke(new Action(() => textBlockSet.Text = $"Set:{Data.SetNumber}")); //wyświetlanie numeru seta

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1); // Set the delay time here (1 second in this example)
            timer.Tick += Timer_Tick;
        }


        private void LoadDataFromExcel() 
        {
            // string fileName = "\\\\KWIPUBV04\\General$\\Enercon\\Shared\\wiring\\PrzewodyProgramWszystkie.xlsx"; //śzieżka pod którą jest lista excel z której są pobierane dane
            //  string fileName = "\\\\KWIPUBV04\\General$\\Enercon\\Shared\\mounting\\PrzewodyProgramWszystkie.xlsx"; //śzieżka pod którą jest lista excel z której są pobierane dane
            string fileName = "C:\\Users\\2281209\\Downloads\\PrzewodyProgramWszystkie.xlsx";
            using (var excelWorkbook = new XLWorkbook(fileName)) //otwiera podany plik excel
            {
                //           var nonEmptyDataRows = excelWorkbook.Worksheet(2).RowsUsed();

                myData.ListOfImportedCabinets = new List<List<Wire>>(); //tworzy nową listę szaf w której są listy przewodów do zrobienia

                var counter = 0;
                foreach (var item in excelWorkbook.Worksheets)
                {
                    ListOfNames.Add(item.Name); // lista nazw szaf potrzebna do wyboru szafy poprzez combobox

                    if (!item.Name.Equals("Podsumowanie"))  // nazwa zakładki nie może być: "Podsumowanie"
                    {
                        var nonEmptyDataRows = item.RowsUsed(); //czytamy tylko wiersze które nie są puste
                        myData.ListOfImportedCabinets.Add(new List<Wire>());  //dodajemy nową listę np. szafę xxxx1

                        foreach (var dataRow in nonEmptyDataRows) //iterujemy po każdym wierszu który załadowaliśmy z aplikacji
                        {
                            if (dataRow.RowNumber() >= 3) //zaczyna od 3 wiersza
                            {
                                _wire = new Wire(); //tworzymy nowy przewód i dodajemy do niego atrybuty:

                                _wire.NameOfCabinet = item.Name; //nazwa szafy brana jest z nazwy zakładki
                                _wire.Number = dataRow.Cell(1).Value.GetText(); //czytamy pierwszą kolumnę jako numer itd
                                _wire.Nc = dataRow.Cell(2).Value.GetText();
                                _wire.Torque = dataRow.Cell(3).Value.GetText();
                                _wire.Descriptions = dataRow.Cell(4).Value.GetText();
                                _wire.Bus = dataRow.Cell(5).Value.GetText();
                                _wire.Box = dataRow.Cell(6).Value.GetText();

                                //_wire.DtSource = dataRow.Cell(4).Value.GetText();
                                //_wire.WireEndDimensionSource = dataRow.Cell(7).Value.GetText();
                                //_wire.WireEndTerminationSource = dataRow.Cell(6).Value.GetText();

                                //_wire.DtTarget = dataRow.Cell(8).Value.GetText();
                                //_wire.WireEndTerminationTarget = dataRow.Cell(10).Value.GetText();
                                //_wire.WireEndDimensionTarget = dataRow.Cell(11).Value.GetText();
                                //_wire.Colour = dataRow.Cell(12).Value.GetText();
                                //_wire.CrossSection = ParseFromStringToDouble(dataRow.Cell(13).Value.GetText());
                                //_wire.Type = dataRow.Cell(14).Value.GetText();
                                //_wire.Lenght = ParseFromStringToDouble(dataRow.Cell(16).Value.GetText());



                                myData.ListOfImportedCabinets[counter].Add(_wire); //finalne dodanie przewodu do listy
                            }
                        }

                        counter++;
                    }
                }
                for (int i = 0; i < myData.ListOfImportedCabinets.Count; i++)
                {
                    myData.ListOfImportedCabinets[i] = myData.ListOfImportedCabinets[i].OrderBy(x => x.Bus).ToList();
                }


            }
            comboBox.ItemsSource = ListOfNames; //kopiowanie nazw szaf do comboboxa żeby były one do wyboru
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            expander.IsExpanded = false; // Hide the ListView when the timer ticks
            timer.Stop(); // Stop the timer after hiding
        }
        private void Expander_MouseEnter(object sender, MouseEventArgs e)
        {
          //  listView.Width = 300;
            expander.IsExpanded = true;
            timer.Stop();
        }

        private void Expander_MouseLeave(object sender, MouseEventArgs e)
        {
          //  listView.Width = 10;
            timer.Start();
           // expander.IsExpanded = false;
        }
        private void expander_GotMouseCapture(object sender, MouseEventArgs e)
        {
          //  listView.Width = 500;
            expander.IsExpanded = true;
            timer.Stop();
        }

        public double ParseFromStringToDouble(string stringToParse)
        {
            if (stringToParse.Contains("m"))// czasami potrafiło się pojawić m w listach (podawanie długości w metrach zamiast mm)
                stringToParse = stringToParse.Substring(0, stringToParse.Length - 3); // usuwanie wtedy 3 ostatnich liter -> mm2
            double result;
            if (Double.TryParse(stringToParse, out result)) //parsowanie danych na double 
                return result;
            else return 0.0;  //jeśli się nie uda to zwraca 0.0
        }

        public void ClearAllConfirms() //czyści listę i ładuje na nowo z danych z excela
        {
            myData.ListOfImportedCabinets.Clear();
            LoadDataFromExcel();
        }


        private void ChooseImage(int index, Image image, int PictureNumber) //wyświetlanie konkretnego zdjęcia w podanej kontrolce Image
        {

            var folderCabinetName = myData.ListOfImportedCabinets[_findedCabinetIndex][index].NameOfCabinet;
            var folderWireName = myData.ListOfImportedCabinets[_findedCabinetIndex][index].Number;

            var nameOfImage = @$"\{folderCabinetName}\{folderWireName}\{PictureNumber}.png";

            if(File.Exists(AppDomain.CurrentDomain.BaseDirectory
                            + nameOfImage))
            {
                try
                {
                    Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() => {
                            image.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory
                            + nameOfImage, UriKind.Absolute));
                        }));
                }
                catch (Exception)
                {
                    ;
                }
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => {
                    image.Source = null;
                }));
            }


        }



        //public void HideImages() nieużywane ale zostawię bo można użyć do chowania wyświetlanych zdjęć na które się kliknie
        //{
        //    foreach (Window item in App.Current.Windows)
        //    {
        //        if (item != this)
        //        {
        //            Dispatcher.Invoke(new Action(() => item.Close()));

        //        }
        //    }
        //}

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //odpalane przy wyborze nowej safy poprzez combobox
        {
            if (comboBox.SelectedIndex != -1)
            {
                Window1 subWindow = new Window1();
                subWindow.ShowDialog();

                if (Data.SetNumber == null)
                {
                    MessageBox.Show("Nie podano numeru seta!");
                    return;
                }


                Dispatcher.Invoke(new Action(() => textBlockSet.Text = $"Set:{Data.SetNumber}"));
                //   string inputRead = new InputBox("Insert something", "Title", "Arial", 20).ShowDialog();

                ClearAllConfirms();
                Dispatcher.Invoke(new Action(() => labelPotwierdzonoWszystkiePrzewody.Visibility = Visibility.Hidden));

                _findedCabinetIndex = comboBox.SelectedIndex;

                var name = myData.ListOfImportedCabinets[_findedCabinetIndex][0].NameOfCabinet;

                if (File.Exists($@"{name}_{Data.SetNumber}")) //sprawdzanie czy już dana szafa była robiona ->jeśli była to ładuje dane na temat potwierdzonych przewodów
                {
                    FileOperations.ReadMemory(ref _findedCabinetIndex, myData.ListOfImportedCabinets, $@"{name}_{Data.SetNumber}"); // dane są zapisywane w pliku nazwaszafy_numerseta
                }
                else
                {
                 //   myData.ListOfImportedCabinets[_findedCabinetIndex][1].IsConfirmed = true; 
                 //   listView.ItemsSource = myData.ListOfImportedCabinets[_findedCabinetIndex];
                }
                myData.ListOfImportedCabinets[_findedCabinetIndex][1].IsConfirmed = true;

                    listView.ItemsSource = myData.ListOfImportedCabinets[_findedCabinetIndex]; // ładuje nową szafę do listview

                    MoveDownSelectedItemFromList(listView); //odświeżanie widoku aplikcaji




            }

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var item = listView.SelectedItem;
            if (item != null)
            {
                // MessageBox.Show(item.ToString());
            }
            else
                return;

            int index = listView.Items.IndexOf(item);

            var hux = myData.ListOfImportedCabinets[_findedCabinetIndex][index].IsConfirmed = true;

            MoveDownSelectedItemFromList(listView);
            listView.Items.Refresh();

                var allValid = myData.ListOfImportedCabinets[_findedCabinetIndex].Any() && myData.ListOfImportedCabinets[_findedCabinetIndex].All(item => item.IsConfirmed);
                
                if(allValid)
                {
                  Dispatcher.Invoke(new Action(() => labelPotwierdzonoWszystkiePrzewody.Visibility = Visibility.Visible));
                }
                else
                    Dispatcher.Invoke(new Action(() => labelPotwierdzonoWszystkiePrzewody.Visibility = Visibility.Hidden));


        }

        private void MoveDownSelectedItemFromList(ListView listView)
        {
            if (listView.SelectedIndex < listView.Items.Count - 1)
            {
                listView.SelectedIndex = listView.SelectedIndex + 1;
            }
        }

        private void RefreshList(ListView listView)
        {
            if (listView.SelectedIndex < listView.Items.Count - 1)
            {
                listView.SelectedIndex = listView.SelectedIndex + 1;
                listView.SelectedIndex = listView.SelectedIndex - 1;
            }
            else
            {
                listView.SelectedIndex = listView.SelectedIndex - 1;
                listView.SelectedIndex = listView.SelectedIndex + 1;
            }
        }

        private void listView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = listView.SelectedItem; //sprawdzanie czy mamy jakieś przewody do zatwierdzenia
            if (item != null)
            {
                // MessageBox.Show(item.ToString());
            }
            else
                return;

            int index = listView.Items.IndexOf(item);
            myData.ListOfImportedCabinets[_findedCabinetIndex][index].Start = DateTime.Now; //sprawdzanie statusu wykonania przewodu
            //////////var item = (sender as ListView).SelectedItem;
            //////////if (item != null)
            //////////{
            //////////   // MessageBox.Show(item.ToString());
            //////////}
            //////////else
            //////////    return;

            //////////int index = listView.Items.IndexOf(item);

            //////////var hux = myData.ListOfImportedCabinets[_findedCabinetIndex][index].IsConfirmed = true;
            //////////listView.Items.Refresh();

            //           ShowImage(_findedCabinetIndex, index);

            //foreach (var itemf in myData.ListOfImportedCabinets)
            //{

            //}

        }


        private void TextBlock_TargetUpdated(object sender, DataTransferEventArgs e) //wykorzystywane do wyświetlania obrazków
        {
            var selectedNumber = listView.SelectedIndex;
            if (selectedNumber >= 0)
            {

                ChooseImage(selectedNumber, image_All, 1);

            }
                
        }

        private void image_Source_GotMouseCapture(object sender, MouseEventArgs e) //po kliknięcu na dany obrazek odpala się zdjęcie, które kliknęlismy w windowsowym edytorze zdjęć
        {
            Image image = (Image)sender;
            int PictureNumberToShow = 0;
            switch (image.Name)
            {
                case "image_Source":
                    PictureNumberToShow = 2;
                    break;
                case "image_All":
                    PictureNumberToShow = 1;
                    break;
                case "image_Target":
                    PictureNumberToShow = 3;
                    break;
                default:
                    PictureNumberToShow = 1;
                    break;
            }

            var folderCabinetName = myData.ListOfImportedCabinets[_findedCabinetIndex][0].NameOfCabinet;
            var folderWireName = myData.ListOfImportedCabinets[_findedCabinetIndex][0].Number;

            var nameOfImage = @$"\{folderCabinetName}\{folderWireName}\{PictureNumberToShow}.png";

            var selectedNumber = listView.SelectedIndex;
            if (selectedNumber >= 0)
            {
                try
                {
                    Process.Start(new ProcessStartInfo(@$"{AppDomain.CurrentDomain.BaseDirectory}\{folderCabinetName}\{selectedNumber + 1}\{PictureNumberToShow}.png") { UseShellExecute = true });
                }
                catch (Exception)
                {
                    ;
                }
            }
        }

        private void SourceConfirm_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            if(Data.LoggedPerson == null || Data.LoggedPerson.Length < 2)
            {
                MessageBox.Show("Operacja wymaga zalogowania się!");
                return;
            }

            myData.TextVisibility ^= true;

            var item = listView.SelectedItem; //sprawdzanie czy mamy jakieś przewody do zatwierdzenia
            if (item != null)
            {
                // MessageBox.Show(item.ToString());
            }
            else
                return;

            int index = listView.Items.IndexOf(item);
            var statusValue = myData.ListOfImportedCabinets[_findedCabinetIndex][index].WireStatus; //sprawdzanie statusu wykonania przewodu

            var timespan = DateTime.Now - myData.ListOfImportedCabinets[_findedCabinetIndex][index].Start;
            var seconds = timespan.TotalSeconds;
            myData.ListOfImportedCabinets[_findedCabinetIndex][index].Seconds += seconds;

            switch (btn.Name) // sprawdzanie który przysk wybraliśmy i w zależności od niego dodajemy do parametru wireStatus wartość 1 = potwierdzone source,2 = potwierdzone target,3 = potwierdzone wszystko
            {
                case "btnSourceConfirm":
                    if(statusValue != (int?)Data.Status.SourceConfirmed && statusValue < (int?)Data.Status.AllConfirmed)
                    {
                        myData.ListOfImportedCabinets[_findedCabinetIndex][index].WireStatus += (int?)Data.Status.SourceConfirmed;
                      //  Dispatcher.Invoke(new Action(() => btnSourceConfirm.Content = "Odznacz Source"));
                    }
                    else if(statusValue == (int?)Data.Status.SourceConfirmed || statusValue == (int?)Data.Status.AllConfirmed)
                    {
                        myData.ListOfImportedCabinets[_findedCabinetIndex][index].WireStatus -= (int?)Data.Status.SourceConfirmed;
                      //  Dispatcher.Invoke(new Action(() => btnSourceConfirm.Content = "Potwierdz Source"));
                    }
                    listView.Items.Refresh();
                    break;
                case "btnTargetConfirm":
                    if (statusValue != (int?)Data.Status.TargetConfirmed && statusValue < (int?)Data.Status.AllConfirmed)
                    {
                        myData.ListOfImportedCabinets[_findedCabinetIndex][index].WireStatus += (int?)Data.Status.TargetConfirmed;
                      //  Dispatcher.Invoke(new Action(() => btnTargetConfirm.Content = "Odznacz Target"));
                    }
                    else if (statusValue == (int?)Data.Status.TargetConfirmed || statusValue == (int?)Data.Status.AllConfirmed)
                    {
                        myData.ListOfImportedCabinets[_findedCabinetIndex][index].WireStatus -= (int?)Data.Status.TargetConfirmed;
                      //  Dispatcher.Invoke(new Action(() => btnTargetConfirm.Content = "Potwierdź Target"));
                    }
                    listView.Items.Refresh();
                    break;
                case "btnConfirmBoth":
                    if (statusValue != (int?)Data.Status.AllConfirmed)
                    {
                        myData.ListOfImportedCabinets[_findedCabinetIndex][index].WireStatus = (int?)Data.Status.AllConfirmed;
                    }

                    else
                    {
                        myData.ListOfImportedCabinets[_findedCabinetIndex][index].WireStatus = (int?)Data.Status.Unconfirmed;
                     //   btnConfirmBoth.Content = "asdasd";
                       // Dispatcher.Invoke(new Action(() => btnConfirmBoth.Content = "Potwierdź wszystkie"));
                    }
                    listView.Items.Refresh();
                    break;

                default:
                    break;
            }
            myData.ListOfImportedCabinets[_findedCabinetIndex][index].MadeBy = Data.LoggedPerson;

            double countOfProgress = 0;
            for (int i = 0; i < myData.ListOfImportedCabinets[_findedCabinetIndex].Count; i++)
            {
                if (myData.ListOfImportedCabinets[_findedCabinetIndex][i].WireStatus == (int?)Data.Status.SourceConfirmed ||
                    myData.ListOfImportedCabinets[_findedCabinetIndex][i].WireStatus == (int?)Data.Status.TargetConfirmed)
                    countOfProgress++;
                else if (myData.ListOfImportedCabinets[_findedCabinetIndex][i].WireStatus == (int?)Data.Status.AllConfirmed)
                    countOfProgress += 2;

            }

            myData.ListOfImportedCabinets[_findedCabinetIndex].ForEach(x => x.Progress = Math.Round(  (countOfProgress / (myData.ListOfImportedCabinets[_findedCabinetIndex].Count * 2) * 100), 2));


            if (myData.ListOfImportedCabinets[_findedCabinetIndex][index].WireStatus == (int?)Data.Status.AllConfirmed) //sprawdzenie czy przewód ma wszystko już potwierdzone
                MoveDownSelectedItemFromList(listView); //jeśli tak to przechodzimy do kolejnego przewodu
            else
                RefreshList(listView); // jeśli nie to odświeżamy tylko widok aplikacji

            listView.Items.Refresh();
            myData.ListOfImportedCabinets[_findedCabinetIndex][index].DateOfFinish = DateTime.Now;
           
            FileOperations.WriteListStatusToFile(_findedCabinetIndex, myData.ListOfImportedCabinets[_findedCabinetIndex]); //zapisywanie do pamięci danych o statusie potwierdzeń wszystkich przewodów w danej szafie


            var allValid = myData.ListOfImportedCabinets[_findedCabinetIndex].Any() && myData.ListOfImportedCabinets[_findedCabinetIndex].All(item => item.WireStatus == 3);


            if (allValid) //sprawdzanie czy wykonaliśmy już wszystkie przeowdy
            {
                Dispatcher.Invoke(new Action(() => labelPotwierdzonoWszystkiePrzewody.Visibility = Visibility.Visible));
                FileOperations.SaveLog(myData.ListOfImportedCabinets[_findedCabinetIndex][0].NameOfCabinet, myData.ListOfImportedCabinets[_findedCabinetIndex]);
            }
            else
                Dispatcher.Invoke(new Action(() => labelPotwierdzonoWszystkiePrzewody.Visibility = Visibility.Hidden));
        }

        public static List<List<string>> orginalItems;
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
            //    var tempList = new List<Wire>();
            //    tempList = myData.ListOfImportedCabinets[_findedCabinetIndex];
            //    listView.ItemsSource = tempList.Select(item => item.DtSource.Contains(textBox.Text) || item.DtTarget.Contains(textBox.Text));


                var snToFind = textBox.Text.ToUpper();  // poczukiwany tekst to ten który został wpisany do kontorlki

              //  int curIndex = myData.ListOfImportedCabinets[_findedCabinetIndex].FindIndex(a => a.DtSource.ToUpper().Contains(snToFind));
                int curIndex = myData.ListOfImportedCabinets[_findedCabinetIndex].FindIndex(a => $"{a.DtSource.ToUpper()} <> {a.DtTarget.ToUpper()}".Equals(snToFind));

                if (curIndex >= 0) // jeśli index jest znaleziony
                {
                    listView.SelectedIndex = curIndex;
                    listView.Items.Refresh();
                    listView.Focus();
                    // listView.SetSelected(curIndex, true);
                }
                textBox.Text = string.Empty;


            }
        }

        private void buttonLogging_Click(object sender, RoutedEventArgs e)
        {
            if(buttonLogging.Content != null)
            {
                if (buttonLogging.Content.ToString().ToLower().Equals("zaloguj"))
                {
                    buttonLogging.Content = "Wyloguj";
                }

                else if (Data.LoggedPerson.Length > 0)
                {
                    Data.LoggedPerson = "";
                    Dispatcher.Invoke(new Action(() => textBlockLogged.Text = $"{Data.LoggedPerson}"));
                    buttonLogging.Content = "Zaloguj";
                    return;
                }
            }

            buttonLogging.Visibility = Visibility.Hidden;
            Window2 subWindow = new Window2();
            subWindow.ShowDialog();

            if (Data.LoggedPerson == null)
            {
                MessageBox.Show("Logowanie się nie powiodło!");
                return;
            }
            Dispatcher.Invoke(new Action(() => textBlockLogged.Text = $"Zalogowany/a: {Data.LoggedPerson}"));
            buttonLogging.Visibility = Visibility.Visible;
        }
    }
}
