namespace Splenduel.Core.Global
{
    public class DefaultResponse
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public object Object { get; set; }

        public DefaultResponse(bool success, string? error="")
        {
            Success = success;
            Error = error;
        }
        public DefaultResponse(bool success, object result) : this(success)
        {
            Object = result;
        }
        public static DefaultResponse ok =>new DefaultResponse(true);
        public static DefaultResponse Ok(string message="") => new DefaultResponse(true,message);
        public static DefaultResponse Nok(string error) => new DefaultResponse(false,error);
    }
}
