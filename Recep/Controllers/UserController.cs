using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mini.Common.Models;
using Mini.Wms.Abstraction.Services;
using Mini.Wms.DomainMessages;
using Mini.Wms.MongoDbImplementation.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Recep.Controllers;

[AllowAnonymous]
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
    public async Task<PagedData<UserRecord>> GetAsync()
    {
        PagedDataOptions pagedDataOptions = new(Request.Query);

        var result = await userService.PageAsync(pagedDataOptions);

        var userList = result.Data.Select(r => new UserRecord
        {
            Username = r.Username,
            FirstName = r.FirstName,
            LastName = r.LastName,
        });

        return new PagedData<UserRecord>
        {
            TotalRecordCount = result.TotalRecordCount,
            Data = userList,
            Page = pagedDataOptions.Page,
            PageSize = pagedDataOptions.PageSize
        };
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
        User userDbObject = new()
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
