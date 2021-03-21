using System.Web.Routing;

namespace Finale.WebServer {
    /// <summary>
    /// Provides extension methods for adding controller-handled routes to the route table
    /// </summary>
    public static class RoutingExtensions {
        static RouteValueDictionary DictFromAnnoymousType(object o) {
            if (o == null) return null;
            var dict = new RouteValueDictionary();
            foreach (var p in o.GetType().GetProperties()) dict[p.Name] = p.GetValue(o, null);
            return dict;
        }


        /// <summary>
        /// Maps a controller-handled route to the route table
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="url">URL pattern</param>
        /// <param name="defaults">default values of variables in URL pattern</param>
        /// <param name="constraints">Constraints of variables in URL pattern for a successful match</param>
        /// <param name="areaName">Name of Area of this route</param>
        /// <param name="namespaces">Namespaces of controllers to map to this route</param>
        /// <returns>Mapped route</returns>
        public static Route MapRoute(this RouteCollection routes, string url, object defaults = null, object constraints = null, string areaName = null, string[] namespaces = null) {
            if (namespaces == null) namespaces = new[] { "*" };

            var r = new Route(
                url: url,
                defaults: DictFromAnnoymousType(defaults),
                constraints: DictFromAnnoymousType(constraints),
                routeHandler: new RouteHandler(namespaces)
            );
            routes.Add(r);
            r.DataTokens = new RouteValueDictionary();
            r.DataTokens["area"] = areaName;
            return r;
        }
    }
}