using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Widget;
using Android.Util;
using Android.Content;

using Android.Database.Sqlite;
using System.Threading;
using System;
namespace final_project
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        TextView infotext;
        EditText usernameInput;
        EditText passwordInput;
        Button signInButton;
        Button createAccountButton;

        int infotextid = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            infotext = FindViewById<TextView>(Resource.Id.infotext);
            usernameInput = FindViewById<EditText>(Resource.Id.usernameinput);
            passwordInput = FindViewById<EditText>(Resource.Id.passwordinput);
            signInButton = FindViewById<Button>(Resource.Id.signinbutton);
            createAccountButton = FindViewById<Button>(Resource.Id.createaccountbutton);

            signInButton.Click += delegate {
                SignIn();
            };

            createAccountButton.Click += delegate
            {
                Intent intent = new Intent(this, typeof(SignUpActivity));
                // intent.PutExtra("info", 1);
                StartActivity(intent);
                Log.Info("mainactivity", "clicked");
            };

        }

        private void SignIn()
        {
            DBHelper dbhelper = new DBHelper(this);
            var accountInfo = DBHelper.DBAccountFromUsername(dbhelper.ReadableDatabase, usernameInput.Text);
           
            if (accountInfo.Item1 == "") { SetErrorText("username does not exist"); return; }
            if (accountInfo.Item2 != passwordInput.Text) { SetErrorText("wrong password"); return; }

            Log.Info("mainactivity", "SIGN IN SUCCESSFULL - Going to BooksActivity");

            Intent intent = new Intent(this, typeof(BooksActivity));
            intent.PutExtra("USERNAME", accountInfo.Item1);
            intent.PutExtra("PASSWORD", accountInfo.Item2);
            intent.PutExtra("PHONENUMBER", accountInfo.Item3);
            StartActivity(intent);
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

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}