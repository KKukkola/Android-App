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
using Xamarin.Essentials;

namespace final_project
{
    [Activity(Label = "EditBookActivity")]
    public class EditBookActivity : Activity
    {
        //public event EventHandler<MyEventArgs> ProgressChanged;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_editbook);

            var titletext = FindViewById<TextView>(Resource.Id.titletext);
            var authortext = FindViewById<TextView>(Resource.Id.authortext);
            var pagesseeker = FindViewById<SeekBar>(Resource.Id.seekbar);
            var seekbartext = FindViewById<TextView>(Resource.Id.seekbartext);

            titletext.Text = Intent.GetStringExtra("TITLE");
            authortext.Text = Intent.GetStringExtra("AUTHOR");

            var username = Intent.GetStringExtra("USERNAME");
            var totalPages = Intent.GetIntExtra("PAGES", 0);
            pagesseeker.Max = totalPages;

            // Using Shared Preferences to update the progress bar
            string pagesReadKey = username + titletext.Text + authortext.Text;
            int pagesRead = 0;
            if (Preferences.ContainsKey(pagesReadKey))
            {
                pagesRead = Preferences.Get(pagesReadKey, 0);
            }
            else
            {
                Preferences.Set(pagesReadKey, 0);
            }
            pagesseeker.Progress = pagesRead;
            //

            seekbartext.Text = pagesseeker.Progress.ToString();

            pagesseeker.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) =>
            {
                if (e.FromUser)
                {
                    Preferences.Set(username + titletext.Text + authortext.Text, e.Progress);
                    seekbartext.Text = e.Progress.ToString();
                }
            };
        }

        protected override void OnStop()
        {
            base.OnStop();

        }

    }
}