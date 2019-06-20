namespace Implem.Pleasanter.Libraries.Responses
{
    public class Response
    {
        public string Method;
        public string Target;
        public object Value;

        public Response(string method, string target, object value)
        {
            Method = method;
            Target = target;
            Value = value;
        }
    }
}