using Finale.WebServer;

namespace WebServerExample {
    public class DefaultController : Controller {
        public void GetDefault() {
            Response.ContentType = "text/plain";
            Response.Write("Hello World from Default Controller");
        }
    }
}