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

using Android.Database.Sqlite;

using Android.Util;
using Xamarin.Essentials;

namespace final_project
{
    class DBHelper : SQLiteOpenHelper
    {
        private static string DBName = "DB";
        private static int DBVersion = 1;
        private static SQLiteDatabase database = null;

        public DBHelper(Context context) : base(context, DBName, null, DBVersion)
        { }

        public override void OnCreate(SQLiteDatabase db)
        {
            db.ExecSQL("CREATE TABLE ACCOUNT (_id INTEGER PRIMARY KEY AUTOINCREMENT," +
                "USERNAME TEXT, PASSWORD TEXT, PHONENUMBER TEXT);");

            db.ExecSQL("CREATE TABLE BOOK (_id INTEGER PRIMARY KEY AUTOINCREMENT," +
                "TITLE TEXT, AUTHOR TEXT, USERNAME TEXT, PAGES INTEGER);");

            database = db;
        }

        public static Tuple<string, string, string> DBAccountFromUsername(SQLiteDatabase db, string username)
        {
            // https://stackoverflow.com/questions/10600670/sqlitedatabase-query-method
            var cursor = db.Query("ACCOUNT", new string[] { "_id", "USERNAME", "PASSWORD", "PHONENUMBER" }, "USERNAME = ?", new string[] { username }, null, null, null);
            
            if (cursor.MoveToFirst())
            {
                return new Tuple<string, string, string>(
                    cursor.GetString(cursor.GetColumnIndex("USERNAME")),
                    cursor.GetString(cursor.GetColumnIndex("PASSWORD")),
                    cursor.GetString(cursor.GetColumnIndex("PHONENUMBER"))
                    );
            }
            return new Tuple<string, string, string>("", "", "");
        }

        public static Tuple<string, string, int> DBBookFromTitleAuthorUser(SQLiteDatabase db, string title, string author, string username)
        {
            // https://stackoverflow.com/questions/10600670/sqlitedatabase-query-method
            var cursor = db.Query("BOOK", new string[] { "_id", "TITLE", "AUTHOR", "PAGES", "USERNAME" }, 
                "TITLE = ? AND AUTHOR = ? AND USERNAME = ?", new string[] { title, author, username }, null, null, null);

            if (cursor.MoveToFirst())
            {
                return new Tuple<string, string, int>(
                    cursor.GetString(cursor.GetColumnIndex("TITLE")),
                    cursor.GetString(cursor.GetColumnIndex("AUTHOR")),
                    cursor.GetInt(cursor.GetColumnIndex("PAGES"))
                    );
            }
            return new Tuple<string, string, int>("", "", -1);
        }

        public static bool InsertAccount(SQLiteDatabase db, string username, string password, string phonenumber)
        {
            var accountInfo = DBAccountFromUsername(db, username);
            if (accountInfo.Item1 == username) // that username already exists in the database
                return false; 

            ContentValues value = new ContentValues();
            value.Put("USERNAME", username);
            value.Put("PASSWORD", password);
            value.Put("PHONENUMBER", phonenumber);
            Log.Info("dbhelper", String.Format("Insert Account: {0} {1} {2}", username, password, phonenumber));
            db.Insert("ACCOUNT", null, value);
            return true;
        }

        public static bool InsertBook(SQLiteDatabase db, string username, string title, string author, int pages)
        {
            var bookInfo = DBBookFromTitleAuthorUser(db, title, author, username);
            if (bookInfo.Item1 == title && bookInfo.Item2 == author) // book already exists
                return false;

            ContentValues value = new ContentValues();
            value.Put("TITLE", title);
            value.Put("AUTHOR", author);
            value.Put("PAGES", pages);
            value.Put("USERNAME", username);
            Log.Info("dbhelper", String.Format("Insert Book: {0} {1} {2} {3}", title, author, pages, username));
            db.Insert("BOOK", null, value);
            return true;
        }

        public static bool DeleteBook(SQLiteDatabase db, string username, string title, string author)
        {
            db.Delete("BOOK", "USERNAME = ? AND TITLE = ? AND AUTHOR = ?", new string[] { username, title, author });
            Preferences.Set(username + title + author, 0); // clear pages read for this book
            return true;
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            if (oldVersion < 2)
            {

            }
        }
    }
}