using System;
using System.Linq;
using System.Net.Http;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SQLite;

namespace DWGettingStartedXamarin
{
    [Activity(Label = "GL Fleet", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false, LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    [IntentFilter(new[] { "com.darryncampbell.datawedge.xamarin.ACTION" }, Categories = new[] { Intent.CategoryDefault })]
    public class MainActivity : AppCompatActivity
    {
        public string type { get; set; }
        public string inputType { get; set; }
        public string emplNum { get; set; }
        public string descrip { get; set; }
        public string brand { get; set; }
        public string item { get; set; }
        public int qtyEnter { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            DWUtilities.CreateDWProfile(this);
            type = Intent.GetStringExtra("Type");
            inputType = Intent.GetStringExtra("InputType");
            emplNum = Intent.GetStringExtra("EmplNum");
            Button btnSubmit = (Button)FindViewById(Resource.Id.btnSubmit);
            btnSubmit.Click += SaveInfo;
            Button btnCancel = (Button)FindViewById(Resource.Id.btnCancel);
            btnCancel.Click += CancelAction;
            TextView outputMainTitle = FindViewById<TextView>(Resource.Id.mainTitle);
            outputMainTitle.Text = "Scan Item Barcode - " + type + " " + inputType;

            if (inputType == "In")
            {
                outputMainTitle.SetTextColor(Color.ParseColor("#84C8FA"));
                TextView outputTruck = FindViewById<TextView>(Resource.Id.textTruck);
                TextView outputTitleTruck = FindViewById<TextView>(Resource.Id.titleTruck);
                outputTruck.Visibility = ViewStates.Gone;
                outputTitleTruck.Visibility = ViewStates.Gone;
            }
            else
            {
                outputMainTitle.SetTextColor(Color.ParseColor("#FF6C60"));
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            DisplayScanResult(intent);
        }

        private void DisplayScanResult(Intent scanIntent)
        {
            //String decodedSource = scanIntent.GetStringExtra(Resources.GetString(Resource.String.datawedge_intent_key_source));
            //String decodedLabelType = scanIntent.GetStringExtra(Resources.GetString(Resource.String.datawedge_intent_key_label_type));
            //String scan = decodedData + " [" + decodedLabelType + "]\n\n";
            String decodedData = scanIntent.GetStringExtra(Resources.GetString(Resource.String.datawedge_intent_key_data));
            String scan = decodedData + "\n\n";
            TextView output = FindViewById<TextView>(Resource.Id.textItemScan);
            output.Text = scan + output.Text;

            if (TextUtils.IsEmpty(output.Text))
            {
                output.SetError("Barcode cannot be empty", null);
                return;
            }
            else
            {
                DispayFormValues(decodedData);
            }
        }

        public void DispayFormValues(string itemBarcode)
        {
            try
            {
                string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folderPath, "Inv.db3")))
                {
                    item = connection.Table<ItemBarcodeMasterList>().Where(x => x.Barcode == itemBarcode).Select(p => p.ItemNum).FirstOrDefault();
                    brand = connection.Table<ItemBarcodeMasterList>().Where(x => x.Barcode == itemBarcode).Select(p => p.Brand).FirstOrDefault();
                    descrip = connection.Table<ItemBarcodeMasterList>().Where(x => x.Barcode == itemBarcode).Select(p => p.Description).FirstOrDefault();
                }

                TextView outputItem = FindViewById<TextView>(Resource.Id.textItemNum);
                outputItem.Text = item;
                TextView outputBrand = FindViewById<TextView>(Resource.Id.textBrand);
                outputBrand.Text = brand;
                TextView outputDescr = FindViewById<TextView>(Resource.Id.textItemDescr);
                outputDescr.Text = descrip;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Display Values failed!", ToastLength.Long).Show();
            }
        }

        public async void SaveInfo(object sender, EventArgs e)
        {
            try
            {
                TextView outputScan = FindViewById<TextView>(Resource.Id.textItemScan);
                TextView outputItem = FindViewById<TextView>(Resource.Id.textItemNum);
                TextView outputBrand = FindViewById<TextView>(Resource.Id.textBrand);
                TextView outputDescr = FindViewById<TextView>(Resource.Id.textItemDescr);
                TextView outputQty = FindViewById<TextView>(Resource.Id.textQty);
                TextView outputNote = FindViewById<TextView>(Resource.Id.textNote);
                TextView outputTruck = FindViewById<TextView>(Resource.Id.textTruck);

                if (TextUtils.IsEmpty(outputScan.Text))
                {
                    outputScan.SetError("Barcode cannot be empty", null);
                    outputScan.SetBackgroundColor(Color.ParseColor("#F35A5A"));
                    Toast.MakeText(this, "Barcode cannot be empty", ToastLength.Long).Show();
                    return;
                }
                else
                {
                    outputScan.SetBackgroundColor(Color.ParseColor("#E1DFDD"));
                }

                if (TextUtils.IsEmpty(outputItem.Text))
                {
                    outputItem.SetError("Item cannot be empty", null);
                    outputItem.SetBackgroundColor(Color.ParseColor("#F35A5A"));
                    Toast.MakeText(this, "Item cannot be empty", ToastLength.Long).Show();
                    return;
                }
                else
                {
                    outputItem.SetBackgroundColor(Color.ParseColor("#E1DFDD"));
                }

                if (TextUtils.IsEmpty(outputBrand.Text))
                {
                    outputBrand.SetError("Brand cannot be empty", null);
                    outputBrand.SetBackgroundColor(Color.ParseColor("#F35A5A"));
                    Toast.MakeText(this, "Brand cannot be empty", ToastLength.Long).Show();
                    return;
                }
                else
                {
                    outputBrand.SetBackgroundColor(Color.ParseColor("#E1DFDD"));
                }

                if (TextUtils.IsEmpty(outputDescr.Text))
                {
                    outputDescr.SetError("Description cannot be empty", null);
                    outputDescr.SetBackgroundColor(Color.ParseColor("#F35A5A"));
                    Toast.MakeText(this, "Description cannot be empty", ToastLength.Long).Show();
                    return;
                }
                else
                {
                    outputDescr.SetBackgroundColor(Color.ParseColor("#E1DFDD"));
                }

                if (TextUtils.IsEmpty(outputQty.Text))
                {
                    outputQty.SetError("Qty cannot be empty", null);
                    outputQty.SetBackgroundColor(Color.ParseColor("#F35A5A"));
                    Toast.MakeText(this, "Quantity cannot be empty", ToastLength.Long).Show();
                    return;
                }
                else
                {
                    outputQty.SetBackgroundColor(Color.ParseColor("#E1DFDD"));
                }

                using (HttpClient client = new HttpClient())
                {
                    ItemInfo itm = new ItemInfo();
                    itm.EmpNum = int.Parse(emplNum);
                    itm.Type = type;
                    itm.Barcode = outputScan.Text;
                    itm.ItemNum = outputItem.Text;
                    itm.Description = outputDescr.Text;
                    itm.Brand = outputBrand.Text;
                    itm.Note = outputNote.Text;
                    itm.Truck = outputTruck.Text;
                    itm.Qty = int.Parse(outputQty.Text);
                    var json = JsonConvert.SerializeObject(itm);
                    var data = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    var baseUrl = "https://uploadinventoryfunc20221113125647.azurewebsites.net/api/item";
                    var response = await client.PostAsync(baseUrl, data);
                    if (!response.IsSuccessStatusCode)
                    {
                        BeepError();
                        Toast.MakeText(this, "SaveInfo failed! " + response.ReasonPhrase, ToastLength.Long).Show();
                    }
                    else
                    {
                        //Toast.MakeText(this, "Item added! ", ToastLength.Long).Show();
                        outputScan.Text = string.Empty;
                        outputItem.Text = string.Empty;
                        outputBrand.Text = string.Empty;
                        outputDescr.Text = string.Empty;
                        outputQty.Text = string.Empty;
                        ContinueAlert();
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "SaveInfo failed! " + ex.Message.ToString(), ToastLength.Long).Show();
            }
        }

        public void CancelAction(object sender, EventArgs e)
        {
            StartActivity(typeof(LoginActivity));
        }

        public void ContinueAlert()
        {
            Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = dialog.Create();
            alert.SetTitle("Item added!");
            alert.SetMessage("Scan another item or finish?");
            alert.SetButton("Continue scanning", (c, ev) => { });
            alert.SetButton2("Finish", (c, ev) => {
                StartActivity(typeof(LoginActivity));
            });
            dialog.Show();
        }

        public void BeepError()
        {
            ToneGenerator toneGenerator = new ToneGenerator(Android.Media.Stream.Notification, 100);
            toneGenerator.StartTone(Tone.PropNack);
            SystemClock.Sleep(1000);
            toneGenerator.Release();
        }
    }
}
