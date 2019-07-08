using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models;
using System;
using System.Globalization;
using System.IO;

namespace Api.Filters
{
    public class JsonExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment _env;

        public JsonExceptionFilter(IHostingEnvironment env)
        {
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {

            var strLogText = "";
            var ex = context.Exception;

            context.ExceptionHandled = true;
            var objClass = context;
            strLogText += "Message ---\n{0}" + ex.Message;


            if (context.HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                strLogText += Environment.NewLine + ".Net Error ---\n{0}" + "Check MVC Ajax Code For Error";
            }

            strLogText += Environment.NewLine + "Source ---\n{0}" + ex.Source;
            strLogText += Environment.NewLine + "StackTrace ---\n{0}" + ex.StackTrace;
            strLogText += Environment.NewLine + "TargetSite ---\n{0}" + ex.TargetSite;
            if (ex.InnerException != null)
            {
                strLogText += Environment.NewLine + "Inner Exception is {0}" + ex.InnerException;
                //error prone
            }
            if (ex.HelpLink != null)
            {
                strLogText += Environment.NewLine + "HelpLink ---\n{0}" + ex.HelpLink;//error prone
            }

            StreamWriter log;

            var timestamp = DateTime.Now.ToString("d-MMMM-yyyy", new CultureInfo("en-GB"));

            var errorFolder = Path.Combine(_env.WebRootPath, "ErrorLog");

            if (!Directory.Exists(errorFolder))
            {
                Directory.CreateDirectory(errorFolder);
            }

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (!File.Exists($@"{errorFolder}\Log_{timestamp}.txt"))
            {
                log = new StreamWriter($@"{errorFolder}\Log_{timestamp}.txt");
            }
            else
            {
                log = File.AppendText($@"{errorFolder}\Log_{timestamp}.txt");
            }

            var controllerName = (string)context.RouteData.Values["controller"];
            var actionName = (string)context.RouteData.Values["action"];

            // Write to the file:
            log.WriteLine(Environment.NewLine + DateTime.Now);
            log.WriteLine("------------------------------------------------------------------------------------------------");
            log.WriteLine("Controller Name :- " + controllerName);
            log.WriteLine("Action Method Name :- " + actionName);
            log.WriteLine("------------------------------------------------------------------------------------------------");
            log.WriteLine(objClass);
            log.WriteLine(strLogText);
            log.WriteLine();

            // Close the stream:
            log.Close();

            var error = new ApiError();

            if (_env.IsDevelopment())
            {
                error.Message = context.Exception.Message;
                error.Detail = context.Exception.StackTrace;
            }
            else
            {
                error.Message = "A server error occured.";
                error.Detail = context.Exception.Message;
            }

            context.Result = new ObjectResult(error)
            {
                StatusCode = 500
            };
        }
    }
}
