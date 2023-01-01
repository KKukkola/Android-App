using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Util;
using Android.Preferences;
using Xamarin.Essentials;

namespace final_project
{
    public class MyEventArgs : EventArgs
    {
        public string title { get; set; }
        public string author { get; set; }
        public int pages { get; set; }
        public int progress { get; set; }
        public MyEventArgs(string title, string author, int pages, int progress = 0)
        {
            this.title = title;
            this.author = author;
            this.pages = pages;
            this.progress = progress;
        }
    };

    public class BooksCursorAdapter : CursorAdapter
    {
        Activity context;
        public event EventHandler<MyEventArgs> EditClicked;
        public event EventHandler<MyEventArgs> DeleteClicked;
        
        public BooksCursorAdapter(Activity context, ICursor c) : base(context, c)
        {
            this.context = context;
        }

        // Given a view, update it to display the data in the provided cursor.
        public override void BindView(View view, Context context, ICursor cursor)
        {
            Log.Info("bookscursoradapter", "bindview");
            var titletext = view.FindViewById<TextView>(Resource.Id.titletext);
            var authortext = view.FindViewById<TextView>(Resource.Id.authortext);
            var deletebutton = view.FindViewById<Button>(Resource.Id.deletebutton);
            var editbutton = view.FindViewById<Button>(Resource.Id.editbutton);
            var progressbar = view.FindViewById<ProgressBar>(Resource.Id.progressbar);

            var username = cursor.GetString(cursor.GetColumnIndex("USERNAME"));
            var totalPages = cursor.GetInt(cursor.GetColumnIndex("PAGES"));

            titletext.Text = cursor.GetString(cursor.GetColumnIndex("TITLE"));
            authortext.Text = cursor.GetString(cursor.GetColumnIndex("AUTHOR"));

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
            progressbar.Max = totalPages;
            progressbar.Progress = pagesRead;
            //


            editbutton.Click += delegate
            {
                OnEditClicked(new MyEventArgs(
                    titletext.Text, 
                    authortext.Text, 
                    totalPages
                    ));
            };

            deletebutton.Click += delegate
            {
                OnDeleteClicked(new MyEventArgs(
                    titletext.Text, 
                    authortext.Text, 
                    totalPages
                    ));
            };

        }

        public void OnEditClicked(MyEventArgs e)
        {
            EventHandler<MyEventArgs> handler = EditClicked;
            if (null != handler) handler(this, e);
        }

        public void OnDeleteClicked(MyEventArgs e)
        {
            EventHandler<MyEventArgs> handler = DeleteClicked;
            if (null != handler) handler(this, e);
        }

        public override View NewView(Context context, ICursor cursor, ViewGroup parent)
        {
            Log.Info("bookscursoradapter", "NewView");
            return this.context.LayoutInflater.Inflate(Resource.Layout.listrow_bookentry, parent, false);
        }
    }
}