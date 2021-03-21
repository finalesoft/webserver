using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Finale.WebServer.Descriptors {
    class ControllerDescriptorCollection : IEnumerable<ControllerDescriptor> {
        List<ControllerDescriptor> List = new List<ControllerDescriptor>();

        IEnumerator<ControllerDescriptor> IEnumerable<ControllerDescriptor>.GetEnumerator() {
            return List.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return List.GetEnumerator();
        }


        void AddController(ControllerDescriptor controllerDescriptor) {
            foreach (var cd in List) {
                if (cd.Name == controllerDescriptor.Name) {
                    throw new System.Reflection.AmbiguousMatchException(string.Format("Controllers {0} and {0} cannot coexist in the same route", controllerDescriptor.Name, cd.Name));
                }
            }

            List.Add(controllerDescriptor);
        }

        public void Add(Type t) {
            AddController(new ControllerDescriptor(t));
        }
        public void AddRange(IEnumerable<ControllerDescriptor> controllers) {
            foreach (var cd in controllers) AddController(cd);
        }



        //cache collection lookups
        static Dictionary<string, ControllerDescriptorCollection> GlobalCollections = new Dictionary<string, ControllerDescriptorCollection>();
        public static ControllerDescriptorCollection GetControllers(string _namespace) {
            if (!GlobalCollections.ContainsKey(_namespace)) {
                var collection = new ControllerDescriptorCollection();

                var pattern = "^" + Regex.Escape(_namespace).Replace(@"\*", ".*") + "$"; //match Company.Product.* with Company.Product.Class
                foreach (var a in AppDomain.CurrentDomain.GetAssemblies()) {
                    foreach (var t in a.GetTypes()) {
                        if (!t.IsClass || t.IsAbstract || !t.IsSubclassOf(typeof(Controller))) continue;
                        if (Regex.IsMatch(t.Namespace, pattern)) collection.Add(t);
                    }
                }

                GlobalCollections[_namespace] = collection;
            }
            return GlobalCollections[_namespace];
        }
    }
}
