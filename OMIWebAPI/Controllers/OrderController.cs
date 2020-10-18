using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using OMIWebAPI.DataSource;
using OMIWebAPI.Models;

namespace OMIWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        // GET api/order
        [HttpGet]
        public ActionResult<string> Get()
        {
            DBHandler dbHandler = new DBHandler();
            List<Order> orders = new List<Order>();
            using (var connection = new SqliteConnection(dbHandler.GetConnectingString()))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                SELECT * FROM PurchasedOrder ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Order purchasedorder = new Order();
                        purchasedorder.ID = reader.GetInt32(0);
                        purchasedorder.ProductID = reader.GetInt32(1);
                        purchasedorder.ProductName = reader.GetString(2);
                        purchasedorder.Quantity = reader.GetInt32(3);
                        purchasedorder.Price = reader.GetInt32(4);
                        purchasedorder.PurchasedDate = Convert.ToDateTime(reader.GetString(5));
                        purchasedorder.Status = reader.GetString(6);
                        purchasedorder.StatusDate = Convert.ToDateTime(reader.GetString(7));

                        orders.Add(purchasedorder);
                    }
                }
            }

            string JSONString = JsonConvert.SerializeObject(orders);
            return new ActionResult<string>(JSONString);
        }

        // GET api/order/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            DBHandler dbHandler = new DBHandler();
            List<Order> orders = new List<Order>();
            using (var connection = new SqliteConnection(dbHandler.GetConnectingString()))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                SELECT * FROM PurchasedOrder whereID = " + id + " ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Order purchasedorder = new Order();
                        purchasedorder.ID = reader.GetInt32(0);
                        purchasedorder.ProductID = reader.GetInt32(1);
                        purchasedorder.ProductName = reader.GetString(2);
                        purchasedorder.Quantity = reader.GetInt32(3);
                        purchasedorder.PurchasedDate = reader.GetDateTime(4);
                        purchasedorder.Status = reader.GetString(3);
                        purchasedorder.StatusDate = reader.GetDateTime(6);

                        orders.Add(purchasedorder);
                    }
                }
            }
            string JSONString = JsonConvert.SerializeObject(orders);
            return new ActionResult<string>(JSONString);
        }

        // POST api/order
        [HttpPost]
        public void Post([FromBody] string value)
        {
            DBHandler dbHandler = new DBHandler();
            Order purchasedorder = JsonConvert.DeserializeObject<Order>(value);

            if (purchasedorder.ID == null)
            {
                purchasedorder.ID = 0;
            }


            using (var connection = new SqliteConnection(dbHandler.GetConnectingString()))
            {
                connection.Open();

                var command = connection.CreateCommand();
                try
                {
                    if (purchasedorder.ID == 0)
                    {

                        Product product = GetProduct(purchasedorder.ProductID);

                        if (product.QuantityInStock < purchasedorder.Quantity)
                        {
                            throw new Exception("QuantityInStock is "+ product.QuantityInStock+ ". Please select Quantity is same or less than QuantityInStock.");
                        }
                        else
                        {
                            command.CommandText =
                            @"  INSERT into PurchasedOrder(ProductID,ProductName, Quantity,Price , PurchasedDate,Status,StatusDate) "
                            + " select " + purchasedorder.ProductID + ", '" + purchasedorder.ProductName + "', " + purchasedorder.Quantity + "," + purchasedorder.Price + ", '"
                            + purchasedorder.PurchasedDate + "', '" + purchasedorder.Status + "', '" + purchasedorder.StatusDate + "' "
                            + " WHERE NOT EXISTS (SELECT 1 FROM PurchasedOrder WHERE ID = " + purchasedorder.ID + ") ";

                            command.ExecuteNonQuery();


                            command.CommandText =
                           @"   Update Product set QuantityInStock= (QuantityInStock - " + purchasedorder.Quantity + ")  "
                           + " WHERE  ID = " + purchasedorder.ProductID + " ";

                            command.ExecuteNonQuery();
                        }
                    }

                    if (purchasedorder.ID > 0)
                    {
                        Product product = GetProduct(purchasedorder.ProductID);
                        int OrderID = 0;
                        Int32.TryParse(purchasedorder.ID.ToString(), out OrderID);
                        Order order = GetProductOrder(OrderID);

                        if ((product.QuantityInStock+ order.Quantity) < purchasedorder.Quantity)
                        {
                            throw new Exception("QuantityInStock is " + product.QuantityInStock + ". Please select Quantity of max "+ (product.QuantityInStock + order.Quantity));
                        }
                        else
                        {
                        //    command.CommandText =
                        //@"   Update Product set QuantityInStock= ((QuantityInStock+ (select Quantity from PurchasedOrder where ID = " + purchasedorder.ID + ")) - " + purchasedorder.Quantity + ")  "
                        //+ " WHERE  ID = " + purchasedorder.ProductID + " ";

                            command.CommandText =
                            @"   Update Product set QuantityInStock= ((QuantityInStock+ "+order.Quantity+") - " + purchasedorder.Quantity + ")  "
                            + " WHERE  ID = " + purchasedorder.ProductID + " ";

                            command.ExecuteNonQuery();

                            command.CommandText =
                           @"  Update PurchasedOrder " +
                           " set ProductID=" + purchasedorder.ProductID + ",ProductName= '" + purchasedorder.ProductName + "', Quantity=" + purchasedorder.Quantity + ", Price=" + purchasedorder.Price + ", "
                           + "PurchasedDate='" + purchasedorder.PurchasedDate + "',Status='" + purchasedorder.Status + "',StatusDate='" + purchasedorder.StatusDate + "' "
                           + " WHERE ID = " + purchasedorder.ID + " ";

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
        }

        // PUT api/order/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/order/5
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
                        command.CommandText = @"  Delete from PurchasedOrder WHERE  ID = " + id + " ";

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }


        private Product GetProduct(int id)
        {
            DBHandler dbHandler = new DBHandler();
            Product product = new Product();
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
                       
                        product.ID = reader.GetInt32(0);
                        product.Name = reader.GetString(1);
                        product.QuantityInStock = reader.GetInt32(2);
                        product.Price = reader.GetDecimal(3);

                    }
                }
            }
            return product;
        }

        private Order GetProductOrder(int id)
        {
            DBHandler dbHandler = new DBHandler();
            Order purchasedorder = new Order();
            using (var connection = new SqliteConnection(dbHandler.GetConnectingString()))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"   SELECT * FROM PurchasedOrder whereID = " + id + " ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        purchasedorder.ID = reader.GetInt32(0);
                        purchasedorder.ProductID = reader.GetInt32(1);
                        purchasedorder.ProductName = reader.GetString(2);
                        purchasedorder.Quantity = reader.GetInt32(3);
                        purchasedorder.PurchasedDate = reader.GetDateTime(4);
                        purchasedorder.Status = reader.GetString(3);
                        purchasedorder.StatusDate = reader.GetDateTime(6);

                    }
                }
            }
            return purchasedorder;
        }
    }
}