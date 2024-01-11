namespace zframework.web
{
    public class LoginRequest : BaseRequest
    {
        public override string URI => "/api/login";
        public string id;
        public string password;
    }
}
