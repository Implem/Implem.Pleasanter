namespace Implem.Pleasanter.Libraries.Responses
{
    public class Response
    {
        public string Method;
        public string Target;
        public object Value;
        public string Options;

        public Response(
            string method,
            string target,
            object value,
            string options)
        {
            Method = method;
            Target = target;
            Value = value;
            Options = options;
        }
    }
}