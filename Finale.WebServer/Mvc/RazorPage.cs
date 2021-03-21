using System.IO;
namespace Finale.WebServer.Mvc {
    /// <summary>
    /// A page written in Razor
    /// </summary>
    public abstract class RazorPage {
        internal StreamWriter _writer;
        internal System.Collections.Generic.Dictionary<string, System.Action> _sections;

        /// <summary>
        /// Gets the ViewData object.
        /// </summary>
        protected internal System.Collections.Generic.Dictionary<string, object> ViewData { get; internal set; }
        /// <summary>
        /// Gets the Request object of the current request.
        /// </summary>
        protected internal System.Web.HttpRequest Request { get; internal set; }
        /// <summary>
        /// Gets the Response object of teh current request.
        /// </summary>
        protected internal System.Web.HttpResponse Response { get; internal set; }


        /// <summary>
        /// Internal method.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Internal method.
        /// </summary>
        /// <param name="value"></param>
        protected void Write(object value) {
            if (value == null) return; //value is null for generated Write(RenderSection()) code
            if (value is IRazorContent) (value as IRazorContent).Execute(this._writer);
            else System.Web.HttpUtility.HtmlEncode(value.ToString(), this._writer);
        }
        /// <summary>
        /// Internal method.
        /// </summary>
        /// <param name="value"></param>
        protected void WriteLiteral(string value) { this._writer.Write(value); }



        StreamContent _bodyResult; //rendered content of body page is cached here
        RazorPage _layout;
        /// <summary>
        /// Sets the layout page of the current page
        /// </summary>
        protected string Layout {
            set {
                //body is rendered before layout. we need to cache body content so it can be recalled by layout page later.
                _layout = Razor.CreatePage(new[] { value });
                _layout._writer = this._writer;
                _writer = new StreamWriter(new MemoryStream()) { AutoFlush = true };
            }
        }

        internal void RenderEverything() {
            this.Execute();

            if (_layout != null) { //we will only know whether Layout was set after calling Execute()
                _layout._bodyResult = new StreamContent(_writer.BaseStream);
                _layout._sections = this._sections;
                this._writer = _layout._writer; //we switch the writer so Write() and WriteLiteral() calls in sections (if any) are directed to writer of layout page

                _layout.ViewData = this.ViewData;
                _layout.Request = this.Request;
                _layout.Response = this.Response;
                _layout.Execute();
            }
        }

        /// <summary>
        /// Renders the body of the current layout page.
        /// </summary>
        /// <returns></returns>
        protected IRazorContent RenderBody() {
            if (_bodyResult == null) throw new System.InvalidOperationException("Layout page cannot be rendered directly.");
            return _bodyResult;
        }
        /// <summary>
        /// Renders a HTML string directly to the page.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected IRazorContent RenderHtml(string html) {
            return new HtmlContent(html);
        }

        /// <summary>
        /// Internal method.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        protected void DefineSection(string name, System.Action action) {
            _sections[name] = action;
        }
        /// <summary>
        /// Renders a defined section.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        protected object RenderSection(string name, bool required = false) {
            if (_sections.ContainsKey(name)) _sections[name]();
            else if (required) throw new System.Exception(string.Format("Required section \"{0}\" is not defined.", name));
            return null; //there is no need to return any content to Write()
        }
    }
}