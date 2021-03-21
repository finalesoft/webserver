using System.Reflection;

namespace Finale.WebServer.Descriptors {
    class MethodDescriptor {
        public string Name;
        public MethodInfo MethodInfo;
        public ParameterDescriptor[] Parameters;

        public MethodDescriptor(MethodInfo m) {
            this.MethodInfo = m;
            this.Name = m.Name.ToLowerInvariant();

            var paramsDefinitions = m.GetParameters();
            this.Parameters = new ParameterDescriptor[paramsDefinitions.Length];
            for (int i = 0; i < paramsDefinitions.Length; ++i) this.Parameters[i] = new ParameterDescriptor(paramsDefinitions[i]);
        }
    }
}