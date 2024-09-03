namespace Splenduel.Core.Global
{
    public class DefaultResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object Object { get; set; }

        public DefaultResponse(bool success, string? message="")
        {
            Success = success;
            Message = message;
        }
        public DefaultResponse(bool success, object result) : this(success)
        {
            Object = result;
        }

        public DefaultResponse(bool success, string? error, object @object) : this(success, error)
        {
            Object = @object;
        }

        public static DefaultResponse ok =>new DefaultResponse(true);
        public static DefaultResponse Ok(string message="") => new DefaultResponse(true,message);
        public static DefaultResponse Nok(string error) => new DefaultResponse(false,error);
    }
}
