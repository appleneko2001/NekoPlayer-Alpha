using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data.SQLite;
using LinqToDB.DataProvider.SQLite;
using LinqToDB.DataProvider;
using System.Data;
using Microsoft.Data.Sqlite;

namespace NekoPlayer.Containers
{
    public class DatabaseContainer : LinqToDB.Data.DataConnection
    {
        public bool ConnectionOpened { get; private set; }
        public object State => this.Connection.State;
        public DatabaseContainer(string root, string filename = "data.db") : base (GetDataProvider(), GetConnection(root, filename))
        {
            this.ConnectionOpened = true;
        }
        private static IDataProvider GetDataProvider()
        {
            return new SQLiteDataProvider();
        }
        private static IDbConnection GetConnection(string root, string filename)
        {
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            string final = Path.Combine(root, filename);
            var builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = final;
            //builder.Version = 3;
            //builder.Add("Mode", "ReadWriteCreate");
            return new SqliteConnection(builder.ConnectionString);
        }
    }
}
