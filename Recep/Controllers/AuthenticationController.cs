using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Recep.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    // GET: api/<AuthenticationController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET api/<AuthenticationController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<AuthenticationController>
    [HttpPost]
    public OkObjectResult Post([FromBody] LoginRequest value)
    {
        // 

        // Setup claims

        var authClaims = new List<Claim>();

        //authClaims.Add(new Claim(ClaimTypes.Name, op.Payload.Username));
        //authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        //foreach (var userRole in userRoles)
        //    authClaims.Add(new Claim(ClaimTypes.Role, userRole));

        string issuer = "https://localhost:7001/";
        string audience = "https://localhost:7001/";
        string secretKey = "secret09172301287";


        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );


        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        var res = new LoginResponse
        {
            Jwt = jwtSecurityTokenHandler.WriteToken(token),
            ExpiryDateTime = token.ValidTo
        };

        return Ok(res);

    }

    // PUT api/<AuthenticationController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<AuthenticationController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
