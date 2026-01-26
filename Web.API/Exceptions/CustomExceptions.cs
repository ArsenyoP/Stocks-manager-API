using System.Security.Principal;

namespace Web.API.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class IdentityException : Exception
    {
        public IdentityException(string message) : base(message) { }
    }
}