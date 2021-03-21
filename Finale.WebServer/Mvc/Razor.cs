using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Web.Razor;

namespace Finale.WebServer.Mvc {
    static class Razor {
        static RazorTemplateEngine _templateEngine;
        static CodeDomProvider _codeDomProvider;
        static Config _config;
        static Dictionary<string, Type> _compiledPages = new Dictionary<string, Type>(); //cache compiled razor pages

        static Razor() {
            var host = new RazorEngineHost(new CSharpRazorCodeLanguage()) {
                DefaultBaseClass = typeof(RazorPage).FullName,
                GeneratedClassContext = new System.Web.Razor.Generator.GeneratedClassContext(
                    executeMethodName: "Execute",
                    writeMethodName: "Write",
                    writeLiteralMethodName: "WriteLiteral",
                    writeToMethodName: null,
                    writeLiteralToMethodName: null,
                    templateTypeName: null,
                    defineSectionMethodName: "DefineSection"
                )
            };

            _templateEngine = new RazorTemplateEngine(host);
            _codeDomProvider = Activator.CreateInstance(host.CodeLanguage.CodeDomProviderType) as CodeDomProvider;
            _config = System.Configuration.ConfigurationManager.GetSection("finale.webserver.mvc") as Config ?? new Config();
        }

        static Type CompileRazorPage(string virtualPath, string physicalPath) {
            //generate an unique class name for the file
            string className;
            {
                var sb = new System.Text.StringBuilder(virtualPath.Length);
                for (var i = 0; i < virtualPath.Length; ++i) {
                    switch (virtualPath[i]) {
                        case '~':
                            continue;
                        case '\\':
                        case '/':
                        case '.':
                            sb.Append('_'); break;
                        default:
                            sb.Append(virtualPath[i]); break;
                    }
                }
                className = sb.ToString();
            }

            //parse the code
            var razorCode = System.IO.File.ReadAllText(physicalPath);
            CodeCompileUnit codeDom;
            using (var reader = new System.IO.StringReader(razorCode)) {
                var parseResult = _templateEngine.GenerateCode(reader, className, "GeneratedRazorPage", physicalPath);
                if (!parseResult.Success) {
                    var error = parseResult.ParserErrors[0];
                    throw new System.Web.HttpParseException(error.Message, null, virtualPath, razorCode, error.Location.LineIndex + 1);
                }

                codeDom = parseResult.GeneratedCode;

                //to see the generated code, use the line below
                //using (var writer = new System.IO.StreamWriter("C:\\Files\\" + className + ".cs")) _codeDomProvider.GenerateCodeFromCompileUnit(codeDom, writer, new CodeGeneratorOptions());
            }

            //compile the razor code
            CompilerResults compilerResult;
            {
                var parameters = new CompilerParameters { GenerateInMemory = true };
                parameters.IncludeDebugInformation = _config.DebugRazorPages;
                foreach (var a in AppDomain.CurrentDomain.GetAssemblies()) parameters.ReferencedAssemblies.Add(a.Location);
                compilerResult = _codeDomProvider.CompileAssemblyFromDom(parameters, codeDom);
            }
            if (compilerResult.Errors.HasErrors) throw new System.Web.HttpCompileException(compilerResult, razorCode);

            //find the compiled type and return it
            foreach (var t in compilerResult.CompiledAssembly.GetTypes()) {
                if (t.IsSubclassOf(typeof(RazorPage))) return t;
            }
            throw new Exception("RazorPage type cannot be located in compiled assembly");
        }

        public static RazorPage CreatePage(IEnumerable<string> virtualPaths) {
            foreach (var virtualPath in virtualPaths) {
                if (string.IsNullOrEmpty(virtualPath)) continue;

                //skip paths that point to a non-existing file
                var physicalPath = System.Web.Hosting.HostingEnvironment.MapPath(virtualPath);
                if (!System.IO.File.Exists(physicalPath)) continue;

                //get the underlying Type of RazorPage either from cache or by compiling
                Type pageType;
                if (_compiledPages.ContainsKey(virtualPath)) pageType = _compiledPages[virtualPath];
                else {
                    pageType = CompileRazorPage(virtualPath, physicalPath);
                    if (_config.CacheRazorPages) _compiledPages[virtualPath] = pageType;
                }

                var page = Activator.CreateInstance(pageType) as RazorPage;
                return page;
            }
            
            throw new System.IO.FileNotFoundException("Cannot locate view. The following locations have been tried: " + string.Join(",", virtualPaths));
        }

        public static RazorPage<T> CreatePage<T>(IEnumerable<string> virtualPaths, T model) {
            var page = CreatePage(virtualPaths) as RazorPage<T>;
            if (page == null) throw new ArgumentException(string.Format("RazorPage does not have a model of type \"{0}\"", typeof(T).ToString()));
            page.Model = model;
            return page;
        }
    }
}