namespace Finale.WebServer.Mvc {
    class HtmlContent : IRazorContent {
        string _htmlString;
        public HtmlContent(string htmlString) { this._htmlString = htmlString; }

        public void Execute(System.IO.StreamWriter writer) {
            writer.Write(this._htmlString);
        }
    }
}