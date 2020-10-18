using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMIWebAPI.DataSource
{
  
    //"Data Source=:memory:"; //"Data Source=hello.db"
    public class DBHandler:IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions _options;
        string connectionString = "Data Source=sample.db";

        public DBHandler()
        {
            _connection = new SqliteConnection("datasource=:memory:");
            //_connection.Open();

            _options = new DbContextOptionsBuilder()
                .UseSqlite(_connection)
                .Options;

            //using (var context = new DbContext(_options))
            //    context.Database.EnsureCreated();
            CreateProductTable();

            CreateOrderTable();
           
        }


        public void Dispose()
        {
            _connection.Close();
        }


        public string GetConnectingString()
        {
            return connectionString;
        }

        public void GetConnection()
        {
            _connection.Open();

            using (var context = new DbContext(_options))
                context.Database.EnsureCreated();
        }
        

        private void CreateProductTable()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                DbContextOptions _options = new DbContextOptionsBuilder().UseSqlite(connection).Options;
                using (var context = new DbContext(_options))
                    context.Database.EnsureCreated();

                var command = connection.CreateCommand();

                //command.CommandText =
                //                @"
                //                    drop table 'Product' 
                //                ";
                //command.ExecuteNonQuery();

                command.CommandText =
                @"
                    CREATE TABLE IF NOT EXISTS Product  
                    (  
                        [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(500) NOT NULL UNIQUE, [QuantityInStock] INTEGER NOT NULL, [Price] decimal(15,2) NOT NULL  
                    )  
                ";

                //                command.CommandText =
                //               @"
                //drop table 'Product' 
                //                ";
                command.ExecuteNonQuery();
            }
        }

        private void CreateOrderTable()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();

                //command.CommandText =
                //             @"
                //                drop table 'Product' 
                //             ";

                //command.ExecuteNonQuery();

                //command.CommandText =
                //            @"
                //                drop table 'PurchasedOrder' 
                //             ";

                //command.ExecuteNonQuery();

                command.CommandText =
                @"
                    CREATE TABLE IF NOT EXISTS PurchasedOrder  
                    (  
                        [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, ProductID INTEGER NOT NULL , [ProductName] NVARCHAR(500) NOT NULL, [Quantity] INTEGER NOT NULL, [Price] DECIMAL(15,2) NOT NULL,
                        [PurchasedDate] DATETIME, [Status] NVARCHAR(500) NOT NULL, [StatusDate] DATETIME NOT NULL,FOREIGN KEY (ProductID) REFERENCES Product(Id) 
                    )  
                ";

//                command.CommandText =
//                @"
//drop table 'ProductOrder' 
//                ";

                command.ExecuteNonQuery();
            }
        }
    }


}
