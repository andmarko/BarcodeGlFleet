using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace DWGettingStartedXamarin
{
    [Activity(Label = "GL Fleet", MainLauncher = true)]
    public class LoginActivity : Activity
    {

        EditText emplNum;
        public bool isTire { get; set; }
        public string descrip { get; set; }
        public string itemType { get; set; }
        public string brand { get; set; }
        public string item { get; set; }
        ProgressBar loadingSpinner;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login);
            emplNum = FindViewById<EditText>(Resource.Id.emplNum);
            Button btnAddItem = (Button)FindViewById(Resource.Id.btnAdd);
            btnAddItem.Click += AddItem;
            Button btnOutItem = (Button)FindViewById(Resource.Id.btnOut);
            btnOutItem.Click += OutItem;
            RadioButton radioTire = FindViewById<RadioButton>(Resource.Id.radioTires);
            radioTire.Click += RadioClick;
            RadioButton radioParts = FindViewById<RadioButton>(Resource.Id.radioParts);
            radioParts.Click += RadioClick;
        }

        public async void AddItem(object sender, EventArgs e)
        {
            HideKeyboard(this);
            loadingSpinner = FindViewById<ProgressBar>(Resource.Id.loadingSpinner);
            loadingSpinner.Visibility = ViewStates.Visible;
            await System.Threading.Tasks.Task.Run(() =>
            {
                RunOnUiThread(() =>
                {
                    GetItemInfo();
                    Intent i = new Intent(this, typeof(MainActivity));
                    i.PutExtra("Type", itemType);
                    i.PutExtra("InputType", "In");
                    i.PutExtra("EmplNum", emplNum.Text);
                    StartActivity(i);
                });
            });
        }
        public async void OutItem(object sender, EventArgs e)
        {
            HideKeyboard(this);
            loadingSpinner = FindViewById<ProgressBar>(Resource.Id.loadingSpinner);
            loadingSpinner.Visibility = ViewStates.Visible;
            await System.Threading.Tasks.Task.Run(() =>
            {
                RunOnUiThread(() =>
                {
                    GetItemInfo();
                    Intent i = new Intent(this, typeof(MainActivity));
                    i.PutExtra("Type", itemType);
                    i.PutExtra("InputType", "Out");
                    i.PutExtra("EmplNum", emplNum.Text);
                    StartActivity(i);
                });
            });
        }

        public void RadioClick(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Text == "Tires")
            {
                isTire = true;
            }
            else if (rb.Text == "Parts")
            {
                isTire = false;
            }
            itemType = rb.Text;
        }
        public void GetItemInfo()
        {
            try
            {
                string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var now = DateTime.Now.TimeOfDay;
                TimeSpan end = new TimeSpan(10, 0, 0);
                if (now < end)
                {
                    var webRequest = WebRequest.Create("https://uploadinventoryfunc20221113125647.azurewebsites.net/api/masterList") as HttpWebRequest;
                    if (webRequest == null)
                    {
                        return;
                    }

                    webRequest.ContentType = "application/json";
                    webRequest.UserAgent = "Nothing";

                    using (var s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (var sr = new StreamReader(s))
                        {
                            var listAsJson = sr.ReadToEnd();
                            var masterList = JsonConvert.DeserializeObject<List<ItemBarcodeMasterList>>(listAsJson);

                            if (masterList != null)
                            {
                                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folderPath, "Inv.db3")))
                                {
                                    connection.DropTable<ItemBarcodeMasterList>();
                                    connection.CreateTable<ItemBarcodeMasterList>();
                                    connection.InsertAll(masterList);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "GetItemInfo failed! " + ex.Message.ToString(), ToastLength.Long).Show();
            }
        }
        public void HideKeyboard(Activity activity)
        {
            var currentFocus = activity.CurrentFocus;
            if (currentFocus != null)
            {
                Android.Views.InputMethods.InputMethodManager inputMethodManager = (Android.Views.InputMethods.InputMethodManager)activity.GetSystemService(Context.InputMethodService);
                inputMethodManager.HideSoftInputFromWindow(currentFocus.WindowToken, Android.Views.InputMethods.HideSoftInputFlags.None);
            }
        }
    }
}