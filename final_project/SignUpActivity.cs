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

using Android.Util;

using Android.Database.Sqlite;
using System.Threading;

namespace final_project
{
    [Activity(Label = "SignUpActivity")]
    public class SignUpActivity : Activity
    {
        TextView infotext;
        EditText usernameInput;
        EditText passwordInput;
        EditText phoneInput;
        Button createAccountButton;

        int infotextid = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_signup);

            infotext = FindViewById<TextView>(Resource.Id.infotext);
            usernameInput = FindViewById<EditText>(Resource.Id.usernameinput);
            passwordInput = FindViewById<EditText>(Resource.Id.passwordinput);
            phoneInput = FindViewById<EditText>(Resource.Id.phoneinput);
            createAccountButton = FindViewById<Button>(Resource.Id.createaccountbutton);

            createAccountButton.Click += delegate
            {
                string err = "";
                if (usernameInput.Text == "") { err += "Invalid Username\n"; }
                if (passwordInput.Text == "") { err += "Invalid Password\n"; }
                if (phoneInput.Text == "") { err += "Invalid PhoneNumber\n"; }
                if (err != "")
                {
                    SetErrorText(err);
                    return;
                }

                DBHelper dbhelper = new DBHelper(this);
                bool success = DBHelper.InsertAccount(dbhelper.ReadableDatabase, usernameInput.Text, passwordInput.Text, phoneInput.Text);
                if (success)
                {
                    Toast.MakeText(this, "Account created successfully", ToastLength.Long).Show();
                    Finish();
                }
                else
                {
                    SetErrorText("That username already exists");
                }
            };
        }

        private void SetErrorText(string text)
        {
            int id = new Random().Next();
            infotextid = id;
            infotext.Text = text;
            
            Timer timer = new Timer((e) => 
            {
                RunOnUiThread(() => 
                {
                    if (infotextid == id)
                        infotext.Text = "";
                });
            }, null, 2000, Timeout.Infinite);
        }
    }
}







