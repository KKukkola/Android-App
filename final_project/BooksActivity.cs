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
using AndroidX.AppCompat.App;

namespace final_project
{
    [Activity(Label = "BooksActivity")]
    public class BooksActivity : AppCompatActivity
    {
        Button newbookbutton;
        ListView lview;
        EditBookFragment cEditFragment;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_books);

            newbookbutton = FindViewById<Button>(Resource.Id.newbookbutton);
            lview = FindViewById<ListView>(Resource.Id.listView1);

            RefreshList();

            newbookbutton.Click += delegate
            {
                Log.Info("booksactivity", "new book button clicked");
                Intent intent = new Intent(this, typeof(NewBookActivity));
                intent.PutExtra("USERNAME", Intent.GetStringExtra("USERNAME"));
                intent.PutExtra("PHONENUMBER", Intent.GetStringExtra("PHONENUMBER"));
                StartActivityForResult(intent, 0);
            };
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            Log.Info("booksactivity", string.Format("on activity result: {0} {1} {2}", requestCode, resultCode, data));

            if (requestCode == 0) // New Book Page
            {
                if (resultCode == Result.Ok)
                    RefreshList();
            }

            if (requestCode == 1) // Edit Book Page
            {
                RefreshList();
            }
        }

        private void RefreshList()
        {
            var db = new DBHelper(this).ReadableDatabase;
            var cursor = db.Query("BOOK", new string[] { "_id", "TITLE, AUTHOR, PAGES, USERNAME" },
                "USERNAME = ?", new string[] { Intent.GetStringExtra("USERNAME") }, null, null, null);

            Log.Info("booksactivity", string.Format("RefreshList: {0} entries", cursor.Count.ToString()));
            
            BooksCursorAdapter adapter = new BooksCursorAdapter(this, cursor);
            adapter.EditClicked += EditClicked;
            adapter.DeleteClicked += DeleteClicked;
            lview.Adapter = adapter;
        }
        
        private void EditClicked(object s, MyEventArgs e)
        {
            Log.Info("booksactivity", string.Format("edit clicked for: {0} {1} {2}", e.title, e.author, e.pages));

            View editfragcontainer = FindViewById(Resource.Id.editfragcontainer);
            if (editfragcontainer != null)
            {
                EditBookFragment editfrag = new EditBookFragment();
                editfrag.ProgressChanged += delegate { RefreshList(); };
                editfrag.username = Intent.GetStringExtra("USERNAME");
                editfrag.title = e.title;
                editfrag.author = e.author;
                editfrag.totalpages = e.pages;
                cEditFragment = editfrag;

                AndroidX.Fragment.App.FragmentTransaction trans = SupportFragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.editfragcontainer, editfrag);
                trans.Commit();
            }
            else
            {
                Intent intent = new Intent(this, typeof(EditBookActivity));
                intent.PutExtra("USERNAME", Intent.GetStringExtra("USERNAME"));
                intent.PutExtra("TITLE", e.title);
                intent.PutExtra("AUTHOR", e.author);
                intent.PutExtra("PAGES", e.pages);
                StartActivityForResult(intent, 1);
            }
        }

        private void DeleteClicked(object s, MyEventArgs e)
        {
            Log.Info("booksactivity", string.Format("delete clicked for: {0} {1} {2}", e.title, e.author, e.pages));

            // Prompt user with an alert asking to really delete the book entry
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
            alert.SetTitle("Delete Entry");
            alert.SetMessage("Are you sure?");
            alert.SetPositiveButton("Yes", (senderAlert, args) =>
            {
                SQLiteDatabase db = new DBHelper(this).ReadableDatabase;
                DBHelper.DeleteBook(db, Intent.GetStringExtra("USERNAME"), e.title, e.author);
                if (cEditFragment != null && cEditFragment.title == e.title && cEditFragment.author == e.author)
                {
                    Log.Info("booksactivity", string.Format("Popping the back stack!", cEditFragment.title, cEditFragment.author));
                    AndroidX.Fragment.App.FragmentTransaction trans = SupportFragmentManager.BeginTransaction();
                    trans.Remove(cEditFragment);
                    trans.Commit();
                    cEditFragment = null;
                }
                RefreshList();
            });
            alert.SetNegativeButton("No", (senderAlert, args) =>
            {
                Log.Info("booksactivity", "said no to the delete");
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }

    }
}