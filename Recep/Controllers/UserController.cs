using Microsoft.AspNetCore.Mvc;
using Mini.Wms.Abstraction.Models;
using Mini.Wms.Abstraction.Services;
using Mini.Wms.DomainMessages;
using Mini.Wms.MongoDbImplementation.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Recep.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService<string, User> userService;

    public UserController(IUserService<string, User> userService)
    {
        this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    // GET: api/<UserController>
    [HttpGet]
    public async Task<IEnumerable<UserRecord>> GetAsync()
    {
        try
        {
            IEnumerable<IUser<string>>? result = await userService.AllAsync();

            IEnumerable<UserRecord>? userList = result.Select(r => new UserRecord
            {
                Username = r.Username,
                FirstName = r.FirstName,
                LastName = r.LastName,
            });

            return userList;
        }
        catch (Exception ex)
        {
            throw;
        }
        
    }

    // GET api/<UserController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<UserController>
    [HttpPost]
    public async Task PostAsync([FromBody] NewUserMessage newUser)
    {
        User userDbObject = new User
        {
            Username = newUser.Username,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName
        };

        await userService.AddAsync(userDbObject);

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
