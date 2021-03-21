using Finale.WebServer;

namespace WebServerExample {
    public class DefaultController : Controller {
        protected override bool PreExecute() {
            /*
             * This method is called before the action method. Return false to stop the progression.
             * E.g.
             * if(AuthenticationFail()) {
             *     Response.StatusCode = 403;
             *     return false;
             * }
             * return true;
             */
            return true;
        }

        protected override void PostExecute() {
            /*
             * This method is called after the action method. Can be used for resource cleanup / logging etc.
             */
        }

        public void GetDefault() {
            Response.ContentType = "text/plain";
            Response.Write("Hello World from Default Controller");
        }

        protected override void MethodNotFound(string action) {
            Response.StatusCode = 404;
            Response.Write(string.Format("Action \"{0}\" is defined on Default Controller"));
        }
    }
}