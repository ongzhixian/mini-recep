using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mini.Wms.DataAbstraction;
using Recep.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Recep.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    // GET: api/<UserController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        // I need to know caller's public key here somehow
        return new string[] { "value1", "value2" };
    }

    // GET api/<UserController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<UserController>
    [HttpPost]
    public void Post([FromBody] IUser<T> user)
    {
        throw new NotImplementedException();
    }

    // PUT api/<UserController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
        throw new NotImplementedException();
    }

    // DELETE api/<UserController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
        throw new NotImplementedException();
    }
}
