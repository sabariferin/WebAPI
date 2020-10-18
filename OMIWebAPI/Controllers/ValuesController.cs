using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace OMIWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {

    //        using (var connection = new SqliteConnection("Data Source=hello.db"))
    //        {
    //            connection.Open();

    //            var command = connection.CreateCommand();
    //            command.CommandText =
    //            @"
    //    SELECT name
    //    FROM user
    //    WHERE id = $id
    //";
    //            command.Parameters.AddWithValue("$id", id);

    //            using (var reader = command.ExecuteReader())
    //            {
    //                while (reader.Read())
    //                {
    //                    var name = reader.GetString(0);

    //                    Console.WriteLine($"Hello, {name}!");
    //                }
    //            }
    //        }
        }
    }
}
