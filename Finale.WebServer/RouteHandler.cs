using Finale.WebServer.Descriptors;
using System;
using System.Web.Routing;

namespace Finale.WebServer {
    /// <summary>
    /// A RouteHandler responsible for matching and creating controller instances
    /// </summary>
    public class RouteHandler : IRouteHandler{
        static Type _notFoundHttpHandler = typeof(NotFoundHttpHandler);

        /// <summary>
        /// Gets or sets the Type which implements IHttpHandler to initiate when no matching route is found
        /// </summary>
        public static Type NotFoundHttpHandler {
            get {
                return _notFoundHttpHandler;
            }
            set {
                if (!typeof(System.Web.IHttpHandler).IsAssignableFrom(value)) throw new ArgumentException("Type does not implement IHttpHandler");
                _notFoundHttpHandler = value;
            }
        }


        ControllerDescriptorCollection _controllerDescriptors;

        /// <summary>
        /// Initiates a new instance of RouteHandler.
        /// </summary>
        /// <param name="namespaces">Namespaces of controllers to be handled by this route handler</param>
        public RouteHandler(string[] namespaces) {
            _controllerDescriptors = new ControllerDescriptorCollection();

            foreach (var n in namespaces) _controllerDescriptors.AddRange(ControllerDescriptorCollection.GetControllers(n));
        }



        System.Web.IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext) {
            var typename = (requestContext.RouteData.GetRequiredString("controller") + "controller").ToLowerInvariant();
            ControllerDescriptor targetController = null;
            foreach (var cd in _controllerDescriptors) {
                if (cd.Name == typename) {
                    targetController = cd;
                    break;
                }
            }
            if (targetController == null) return Activator.CreateInstance(NotFoundHttpHandler) as System.Web.IHttpHandler;

            var controllerInstance = Activator.CreateInstance(targetController.Type) as Controller;
            controllerInstance.RequestContext = requestContext;
            controllerInstance.MethodsDescriptors = targetController.Methods;
            return controllerInstance;
        }
    }
}
