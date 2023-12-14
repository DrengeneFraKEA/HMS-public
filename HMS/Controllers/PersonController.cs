using HMS.Data;
using HMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace HMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly PersonService personService = new PersonService();

        [HttpPost("{id}")]
        public string GetPersonData(int id)
        {
            return personService.GetPersonData(id);
        }

        [Authorize]
        [HttpGet("person/{cpr}")]
        public bool DoesPersonExistByCPR(string cpr)
        {
            return personService.DoesPersonExistByCPR(cpr);
        }
    }
}