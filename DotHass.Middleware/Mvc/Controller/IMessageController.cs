using Microsoft.Owin;
using System;

namespace DotHass.Middleware.Mvc.Controller
{
    public interface IMessageController : IDisposable
    {
        OwinContext Context { get; set; }
    }
}