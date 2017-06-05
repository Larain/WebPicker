using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebPicker.Helpers;

namespace WebPicker.Controllers.Base
{
    public class ControllerWithLogging : Controller
    {
        /// <summary>
        /// Logs
        /// </summary>
        public ILogger Logger { get; }

        public ControllerWithLogging(ILogger logger)
        {
            Logger = logger;
        }
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string message = $"Request {context.HttpContext.Request.GetDisplayUrl()}; ";
            foreach (var key in context.ActionArguments.Keys)
            {
                message += $"{key} = {context.ActionArguments[key]}; ";
            }

            var action = context.ActionDescriptor.DisplayName;

            Logger.LogAsync("Info", context.HttpContext.User.Identity.Name, action, message);
            return base.OnActionExecutionAsync(context, next);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            string message = $"Request {context.HttpContext.Request.GetDisplayUrl()} ";
            message += context.Canceled ? "Canceled" : "Done";
            message += context.Result + ";";

            var action = context.ActionDescriptor.DisplayName;

            Logger.LogAsync("Info", context.HttpContext.User.Identity.Name, action, message);
        }
    }
}