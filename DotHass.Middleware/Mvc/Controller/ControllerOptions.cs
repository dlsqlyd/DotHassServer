using DotHass.Middleware.Mvc.Action.Result;
using System;

namespace DotHass.Middleware.Mvc.Controller
{
    public class ControllerOptions
    {
        public string HandlerBeforeMethodName { get; set; } = "HandlerBefore";

        public string HandlerMethodName { get; set; } = "Handler";

        public string HandlerAfterMethodName { get; set; } = "HandlerAfter";

        public string ContractIdName { get; set; } = "ContractId";

        public Type HandlerRetureType { get; set; } = typeof(IActionResult);
    }
}