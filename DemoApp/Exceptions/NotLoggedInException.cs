using System;

namespace DemoApp.Exceptions
{
    public class NotLoggedInException : Exception
    {
        public NotLoggedInException() : base("User is not logged in yet. Use SimpleSessionManager.Login()") { }
    }
}