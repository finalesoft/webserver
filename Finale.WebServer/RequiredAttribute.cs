using System;

namespace Finale.WebServer {
    /// <summary>
    /// Denotes a required value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Parameter)]
    public sealed class RequiredAttribute : Attribute { }
}