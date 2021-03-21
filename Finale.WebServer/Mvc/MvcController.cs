namespace Finale.WebServer.Mvc {
    /// <summary>
    /// An abstract class for implementing a controller with razor views
    /// </summary>
    public abstract class MvcController : Controller {
        string[] _getViewPaths(string viewName = null) {
            string action = viewName ?? RouteData.GetRequiredString("action");
            var controller = RouteData.GetRequiredString("controller");

            return new[] {
                Area != null ? "~/Areas/" + Area + "/Views/" + controller + "/" + action + ".cshtml" : null,
                Area != null ? "~/Areas/" + Area + "/Views/Shared/" + action + ".cshtml" : null,
                "~/Views/" + controller + "/" + action + ".cshtml",
                "~/Views/Shared/" + action + ".cshtml"
            };
        }

        /// <summary>
        /// The ViewData object that is passed to razor page. Can be used to transmit data from controller to view.
        /// </summary>
        protected System.Collections.Generic.Dictionary<string, object> ViewData = new System.Collections.Generic.Dictionary<string, object>();

        void _setRazorPageData(RazorPage razorPage) {
            razorPage._writer = new System.IO.StreamWriter(Response.OutputStream) { AutoFlush = true };
            razorPage._sections = new System.Collections.Generic.Dictionary<string, System.Action>();
            razorPage.ViewData = this.ViewData;
            razorPage.Request = this.Request;
            razorPage.Response = this.Response;
        }

        /// <summary>
        /// Renders a view to the Response output stream
        /// </summary>
        /// <param name="viewName"></param>
        protected void RenderView(string viewName = null) {
            var razorPage = Razor.CreatePage(_getViewPaths(viewName));
            _setRazorPageData(razorPage);

            Response.ContentType = "text/html; charset=utf-8";
            razorPage.RenderEverything();
        }

        /// <summary>
        /// Renders a view with model data to the Response output stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="viewName"></param>
        protected void RenderView<T>(T model, string viewName = null) {
            var razorPage = Razor.CreatePage<T>(_getViewPaths(viewName), model);
            _setRazorPageData(razorPage);
            razorPage.Model = model;

            Response.ContentType = "text/html; charset=utf-8";
            razorPage.RenderEverything();
        }
    }
}