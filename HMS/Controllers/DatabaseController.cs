using HMS.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace HMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DatabaseController : ControllerBase
    {
        [HttpPost]
        public IActionResult ChangeDatabase([FromBody] JsonObject value) 
        {
            if (value != null && value["value"] != null)
                Database.SelectedDatabase = int.Parse(value["value"].ToString());
            else
                Database.SelectedDatabase = 0; // Default to MySQL

            return Ok();
        }
    }
}
