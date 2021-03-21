namespace Finale.WebServer.Mvc {
    /// <summary>
    /// Represents an object that can be rendered on a RazorPage
    /// </summary>
    public interface IRazorContent {
        /// <summary>
        /// Renders the object to the given StreamWriter
        /// </summary>
        /// <param name="writer"></param>
        void Execute(System.IO.StreamWriter writer);
    }
}