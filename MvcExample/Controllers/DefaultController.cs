using Finale.WebServer.Mvc;

namespace MvcExample.Controllers {
    public class DefaultController : MvcController{
        public void GetDefault() {
            ViewData["value"] = "123";

            RenderView();
        }
        public void GetNoLayout() {
            ViewData["value"] = "123";

            RenderView();
        }
        public void GetModel() {
            var list = new System.Collections.Generic.List<string>();
            list.Add("apple");
            list.Add("orange");
            list.Add("banana");
            list.Add("pear");

            RenderView(list);
        }
    }
}