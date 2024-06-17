using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Wiring
{
    public class Wire
    {
        public string NameOfCabinet { get; set; } = "";
        public string Number { get; set; } = "";
        public string Nc { get; set; } = "";
        public string Torque { get; set; } = "";
        public string Descriptions { get; set; } = "";

        public string Bus { get; set; } = "";
        public string Box { get; set; } = "";


        public double CrossSection { get; set; } = 0.0;
        public string Type { get; set; } = "";
        public double Lenght { get; set; } = 0.0;




        public string DtSource { get; set; } = "";
        public string WireEndTerminationSource { get; set; } = "";
        public string DtTarget { get; set; } = "";
        public string WireEndDimensionSource { get; set; } = "";
        public string WireEndDimensionTarget { get; set; } = "";
        public string WireEndTerminationTarget { get; set; } = "";
        public string Colour { get; set; } = "";
        public double? Progress { get; set; } = 0;
        public DateTime Start { get; set; } = DateTime.Now;
        public DateTime DateOfFinish { get; set; } = DateTime.Now;
        public string? MadeBy { get; set; } = "";

        public bool IsConfirmed { get; set; } = false;
        public int? WireStatus { get; set; } = 0;
        public double Seconds { get; set; } = 0;


        public override string ToString()
        {
            return this.Number + ", " + this.DtSource + "";
        }


    }
}
