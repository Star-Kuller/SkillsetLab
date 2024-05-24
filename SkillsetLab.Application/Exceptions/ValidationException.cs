namespace Application.Exceptions
{
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Failures { get; }

        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Failures = new Dictionary<string, string[]>();
        }

        public ValidationException(string key, string error) : this()
        {
            Failures.Add(key, new []{ error });
        }
    }
}
