using Finale.WebServer.Descriptors;
using System;
using System.Reflection;
using System.Web;
using System.Web.Routing;

namespace Finale.WebServer {
    /// <summary>
    /// An abstract class for implementing a generic controller
    /// </summary>
    public abstract class Controller : IHttpHandler {
        internal System.Collections.Generic.IEnumerable<MethodDescriptor> MethodsDescriptors;

        /// <summary>
        /// Whether to append the HTTP verb before the action name when resolving action methods
        /// </summary>
        protected bool AppendHttpVerbToAction = true;
        /// <summary>
        /// This method is called before the action method executes
        /// </summary>
        /// <returns>Whether execution should continue to action method</returns>
        protected virtual bool PreExecute() { return true; }
        /// <summary>
        /// This method is called after the action method executes
        /// </summary>
        protected virtual void PostExecute() { }
        /// <summary>
        /// This method is called when the requested action cannot be located in this controller
        /// </summary>
        /// <param name="action">Action name</param>
        protected virtual void MethodNotFound(string action) { Response.StatusCode = 404; }
        /// <summary>
        /// This method is called when an exception occurs
        /// </summary>
        /// <param name="exception"></param>
        /// <returns>Whether the exception is handled</returns>
        protected virtual bool HandleException(Exception exception) { return false; }

        /// <summary>
        /// Gets the Request object of the current HTTP request
        /// </summary>
        protected HttpRequest Request { get; private set; }
        /// <summary>
        /// Gets the Response object of the current HTTP request
        /// </summary>
        protected HttpResponse Response { get; private set; }
        /// <summary>
        /// Gets the RequestContext object of the current HTTP request
        /// </summary>
        protected internal RequestContext RequestContext { get; internal set; }
        /// <summary>
        /// Gets the route data of the current HTTP request
        /// </summary>
        protected RouteData RouteData { get { return RequestContext.RouteData; } }
        /// <summary>
        /// Gets the name of the area of the current route
        /// </summary>
        protected string Area { get { return RouteData.DataTokens["area"] as string; } }


        object GetMethodParameterValue(ParameterDescriptor info) {
            var value = (string)RouteData.Values[info.Name] ?? Request.QueryString[info.Name] ?? Request.Form[info.Name];
            if (value == null && (info.IsRequired || !info.IsNullableType)) throw new ArgumentNullException();

            if (info.IsNullableType) return value == null ? null : Convert.ChangeType(value, info.Type.GetGenericArguments()[0]);
            return Convert.ChangeType(value, info.Type);
        }
        void WriteError(int httpStatus, string message) {
            Response.Clear();
            Response.ContentType = "text/plain";
            Response.StatusCode = httpStatus;
            Response.Write(message);
            Response.End();
        }

        bool IHttpHandler.IsReusable { get { return false; } }
        void IHttpHandler.ProcessRequest(HttpContext context) {
            Request = context.Request;
            Response = context.Response;

            //construct action name
            var actionName = RouteData.Values["action"] as string ?? string.Empty;
            if (AppendHttpVerbToAction) actionName = Request.HttpMethod + actionName;
            actionName = actionName.ToLowerInvariant();

            //find method in this controller to execute
            MethodDescriptor methodDescriptor = null;
            foreach (var md in MethodsDescriptors) {
                if (md.Name == actionName) {
                    if (methodDescriptor == null) methodDescriptor = md;
                    else throw new AmbiguousMatchException(string.Format("There are multiple methods which match the name {0}", actionName));
                }
            }

            //execute not found method if necessary
            if (methodDescriptor == null) {
                try {
                    MethodNotFound(actionName);
                } catch (Exception ex) {
                    if (!HandleException(ex)) throw;
                }
                return;
            }


            try {
                if (!PreExecute()) return;
            } catch (Exception ex) {
                if (!HandleException(ex)) throw;
            }

            //get method parameters
            var paramsValues = new object[methodDescriptor.Parameters.Length];
            for (int i = 0; i < methodDescriptor.Parameters.Length; i++) {
                try {
                    paramsValues[i] = GetMethodParameterValue(methodDescriptor.Parameters[i]);
                } catch (FormatException) {
                    WriteError(400, string.Format("Parameter \"{0}\" is invalid", methodDescriptor.Parameters[i].Name));
                    return;
                } catch (ArgumentNullException) {
                    WriteError(400, string.Format("Parameter \"{0}\" is missing", methodDescriptor.Parameters[i].Name));
                    return;
                }
            }

            try {
                methodDescriptor.MethodInfo.Invoke(this, paramsValues);
            } catch (TargetInvocationException ex) {
                if (!HandleException(ex.InnerException)) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            try {
                PostExecute();
            } catch (Exception ex) {
                if (!HandleException(ex)) throw;
            }
        }
    }
}