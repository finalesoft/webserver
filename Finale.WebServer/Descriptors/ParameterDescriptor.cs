using System;
using System.Reflection;

namespace Finale.WebServer.Descriptors {
    class ParameterDescriptor {
        public string Name;
        public Type Type;
        public bool IsRequired;
        public bool IsNullableType;

        public ParameterDescriptor(ParameterInfo info) {
            this.Type = info.ParameterType;
            this.Name = info.Name.ToLowerInvariant();
            if (this.Type.IsClass) this.IsRequired = info.GetCustomAttributes(typeof(RequiredAttribute), false).Length > 0;
            else IsNullableType = this.Type.IsGenericType && this.Type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}