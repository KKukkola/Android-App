using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.AppCompat;
using AndroidX.Fragment;
using Xamarin.Essentials;

namespace final_project
{
    public class EditBookFragment : AndroidX.Fragment.App.Fragment
    {
        public string username;
        public string title;
        public string author;
        public int totalpages;
        public string seektext;
        TextView seekbartext;

        public event EventHandler ProgressChanged;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (savedInstanceState != null)
            {
                title = savedInstanceState.GetString("Title");
                author = savedInstanceState.GetString("Author");
                totalpages = savedInstanceState.GetInt("TotalPages", 0);
                seektext = savedInstanceState.GetString("SeekText");
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View myView = inflater.Inflate(Resource.Layout.activity_editbook, container, false);

            // Edit the View
            var titletext = myView.FindViewById<TextView>(Resource.Id.titletext);
            var authortext = myView.FindViewById<TextView>(Resource.Id.authortext);
            var pagesseeker = myView.FindViewById<SeekBar>(Resource.Id.seekbar);
            seekbartext = myView.FindViewById<TextView>(Resource.Id.seekbartext);

            titletext.Text = title;
            authortext.Text = author;
            pagesseeker.Max = totalpages;

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
            if (seektext != null)
            {
                seekbartext.Text = seektext;
            } 
            else
            {
                seekbartext.Text = pagesseeker.Progress.ToString();
            }
            //
            
            pagesseeker.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) =>
            {
                if (e.FromUser)
                {
                    Preferences.Set(username + titletext.Text + authortext.Text, e.Progress);
                    seekbartext.Text = e.Progress.ToString();
                    if (ProgressChanged != null)
                    {
                        ProgressChanged(this, EventArgs.Empty);
                    }
                }
            };

            //

            return myView;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {

            outState.PutString("Title", title);
            outState.PutString("Author", author);
            outState.PutString("SeekText", seekbartext.Text);
            outState.PutInt("TotalPages", totalpages);

            base.OnSaveInstanceState(outState);
        }

    }
}