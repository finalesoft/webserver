using System;
using System.Collections.Generic;

namespace Finale.WebServer.Descriptors {
    class ControllerDescriptor {
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public IEnumerable<MethodDescriptor> Methods { get; private set; }

        public ControllerDescriptor(Type t) {
            this.Type = t;
            this.Name = t.Name.ToLowerInvariant();

            var methods = new List<MethodDescriptor>();
            foreach (var m in t.GetMethods()) {
                if (m.DeclaringType == typeof(object)) continue; //ignore inherited methods from Object

                methods.Add(new MethodDescriptor(m));
            }
            this.Methods = methods;
        }
    }
}