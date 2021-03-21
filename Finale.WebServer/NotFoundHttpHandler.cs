using System.Web;

namespace Finale.WebServer {
    class NotFoundHttpHandler : IHttpHandler{
        public bool IsReusable {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context) {
            context.Response.StatusCode = 404;
        }
    }
}