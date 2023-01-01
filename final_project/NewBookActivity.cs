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
using System.Threading;
using Android.Util;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace final_project
{
    [Activity(Label = "NewBookActivity")]
    public class NewBookActivity : Activity
    {
        EditText titleInput;
        EditText authorInput;
        EditText pagesInput;
        Button createEntryButton;
        TextView infoText;

        int infotextid;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_newbook);

            titleInput = FindViewById<EditText>(Resource.Id.titleinput);
            authorInput = FindViewById<EditText>(Resource.Id.authorinput);
            pagesInput = FindViewById<EditText>(Resource.Id.pagesinput);
            createEntryButton = FindViewById<Button>(Resource.Id.addbookbutton);
            infoText = FindViewById<TextView>(Resource.Id.infotext);

            createEntryButton.Click += delegate
            {
                string err = "";
                if (titleInput.Text == "") { err += "Invalid Title\n"; }
                if (authorInput.Text == "") { err += "Invalid Author\n"; }
                if (pagesInput.Text == "" || int.Parse(pagesInput.Text) < 1) { err += "Invalid Number of Pages\n"; }
                if (err != "") { SetErrorText(err); return; }

                DBHelper dbhelper = new DBHelper(this);
                bool success = DBHelper.InsertBook(dbhelper.ReadableDatabase, Intent.GetStringExtra("USERNAME"), titleInput.Text, authorInput.Text, int.Parse(pagesInput.Text));
                if (success)
                {
                    Toast.MakeText(this, "Book created successfully", ToastLength.Long).Show();

                    // Opens the messenger app to send a text message
                    string phonenumber = Intent.GetStringExtra("PHONENUMBER");
                    string msg = "Created a New Book!";
                    SendSms(msg, phonenumber); // asynchronous

                    //SmsManager.Default.SendTextMessage(msg, null, msg, null, null);

                    Intent myIntent = new Intent(this, typeof(BooksActivity));
                    myIntent.PutExtra("finished", "Hello!");
                    SetResult(Result.Ok, myIntent);
                    Finish();
                }
                else
                {
                    SetErrorText("That book already exists!");
                }
            };
        }

        private async Task SendSms(string msg, string recipient)
        {
            try
            {
                var message = new SmsMessage(msg, new[] { recipient });
                await Sms.ComposeAsync(message);
                Log.Info("newbookactivity", string.Format("sent text message: {0} {1}", recipient, "Hello World"));
            }
            catch (FeatureNotSupportedException ex)
            {
                // Sms is not supported on this device.
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        private void SetErrorText(string text)
        {
            int id = new Random().Next();
            infotextid = id;
            infoText.Text = text;

            Timer timer = new Timer((e) =>
            {
                RunOnUiThread(() =>
                {
                    if (infotextid == id)
                        infoText.Text = "";
                });
            }, null, 2000, Timeout.Infinite);
        }
    }
}