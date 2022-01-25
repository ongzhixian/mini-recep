namespace Recep.Controllers
{
    internal class LoginResponse
    {
        public object Jwt { get; set; }
        public object ExpiryDateTime { get; set; }
    }
}