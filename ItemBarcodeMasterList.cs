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
    public class ItemBarcodeMasterList
    {
        public string ItemNum { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Barcode { get; set; } = null!;
    }
}