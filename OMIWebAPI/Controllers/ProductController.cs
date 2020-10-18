using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OMIWebAPI.DataSource;
using OMIWebAPI.Models;

namespace OMIWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        // GET api/product
        [HttpGet]
        public ActionResult<string> Get()
        {
            DBHandler dbHandler = new DBHandler();
            List<Product> products = new List<Product>();
            using (var connection = new SqliteConnection(dbHandler.GetConnectingString()))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                SELECT * FROM Product ";
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product  = new Product();
                        product.ID = reader.GetInt32(0);
                        product.Name = reader.GetString(1);
                        product.QuantityInStock = reader.GetInt32(2);
                        product.Price = reader.GetDecimal(3);

                        products.Add(product);
                    }
                }
            }
            string JSONString = JsonConvert.SerializeObject(products);
            return new ActionResult<string>(JSONString);
        }

        // GET api/product/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            DBHandler dbHandler = new DBHandler();
            List<Product> products = new List<Product>();
            using (var connection = new SqliteConnection(dbHandler.GetConnectingString()))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                SELECT * FROM Product whereID = " + id + " ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product = new Product();
                        product.ID = reader.GetInt32(0);
                        product.Name = reader.GetString(1);
                        product.QuantityInStock = reader.GetInt32(2);
                        product.Price = reader.GetDecimal(3);

                        products.Add(product);
                    }
                }
            }
            string sJSONResponse = JsonConvert.SerializeObject(products);
            return new ActionResult<string>(sJSONResponse);
        }

        // POST api/product
        [HttpPost]
        public void Post([FromBody] string value)
        {
            DBHandler dbHandler = new DBHandler();
            Product product = JsonConvert.DeserializeObject<Product>(value);

            if(product.ID == null)
            {
                product.ID = 0;
            }

            using (var connection = new SqliteConnection(dbHandler.GetConnectingString()))
            {
                connection.Open();

                var command = connection.CreateCommand();
                try
                {
                    if (product.ID == 0)
                    {
                        command.CommandText =
                        @"  INSERT into Product(Name, QuantityInStock, Price) select '" + product.Name + "', " + product.QuantityInStock + ", " + product.Price + " "
                        + " WHERE NOT EXISTS (SELECT 1 FROM Product WHERE ID = " + product.ID + ") ";

                        command.ExecuteNonQuery();
                    }

                    if (product.ID > 0)
                    {
                        command.CommandText =
                        @"   Update Product set Name='" + product.Name + "', QuantityInStock=" + product.QuantityInStock + ", Price =" + product.Price + " "
                        + " WHERE  ID = " + product.ID + " ";

                        command.ExecuteNonQuery();
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
        }

        // PUT api/product/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/product/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            DBHandler dbHandler = new DBHandler();

            using (var connection = new SqliteConnection(dbHandler.GetConnectingString()))
            {
                connection.Open();

                var command = connection.CreateCommand();
                try
                {
                    if (id > 0)
                    {
                        command.CommandText =  @"  Delete from Product WHERE  ID = " + id + " ";

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}