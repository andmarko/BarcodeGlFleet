using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWGettingStartedXamarin
{
    public class ItemInfo
    {
        public int EmpNum { get; set; }
        public string Type { get; set; } = null!;
        public string Barcode { get; set; } = null!;
        public string ItemNum { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Note { get; set; } = null!;
        public string Truck { get; set; } = null!;
        public int Qty { get; set; }
    }
}