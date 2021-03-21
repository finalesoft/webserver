using System.Configuration;

namespace Finale.WebServer.Mvc {
    /// <summary>
    /// Configuration for RazorPage behavior
    /// </summary>
    public sealed class Config : ConfigurationSection {
        /// <summary>
        /// Whether to cache the compiled razor pages. Cached pages can only be refreshed by application restart.
        /// </summary>
        [ConfigurationProperty("cacheRazorPages", DefaultValue = false)]
        public bool CacheRazorPages {
            get { return (bool)this["cacheRazorPages"]; }
            set { this["cacheRazorPages"] = value; }
        }

        /// <summary>
        /// Whether to include debug information when compiling razor pages.
        /// </summary>
        [ConfigurationProperty("debugRazorPages", DefaultValue = true)]
        public bool DebugRazorPages {
            get { return (bool)this["debugRazorPages"]; }
            set { this["debugRazorPages"] = value; }
        }
    }
}