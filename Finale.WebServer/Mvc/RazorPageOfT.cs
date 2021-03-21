namespace Finale.WebServer.Mvc {
    /// <summary>
    /// A page with model data written in Razor
    /// </summary>
    /// <typeparam name="T">Type of model object</typeparam>
    public abstract class RazorPage<T> : RazorPage {
        /// <summary>
        /// Gets the model object
        /// </summary>
        protected internal T Model { get; internal set; }
    }
}