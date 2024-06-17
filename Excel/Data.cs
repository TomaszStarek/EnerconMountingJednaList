using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Wiring
{
    public class Data
    {
        public List<List<Wire>> ListOfImportedCabinets { get; set; }
        public static string? SetNumber { get; set; }
        public static string? LoggedPerson { get; set; }

        public enum Status : int
        {
            Unconfirmed,
            SourceConfirmed,
            TargetConfirmed,
            AllConfirmed
        }

        private bool textVisibility;

        public bool TextVisibility
        {
            get { return textVisibility; }
            set
            {
                //if (value.Length == 11)
                //{
                //    _data.SerialNumber = value;
                //    StatusInfo = "Barcode OK";
                //}                   
                //else
                //{
                //    _data.SerialNumber = "";
                //    StatusInfo = "Zła ilość znaków";
                //}

                textVisibility = value;
                OnPropertyChanged(nameof(TextVisibility));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
