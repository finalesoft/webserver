# Finale.WebServer

`Finale.WebServer` is a simple and light-weight MVC framework based on ASP.NET. The library is a barebone implementation of the MVC concept, delivering high performance by omitting the superfluous features bundled in the official version. The design exposes the underlying mechanism of ASP.NET as much as possible while maintainig the MVC layout, allowing directly access to the underlyings without middle-layer objects.

Some notable differences with the offical framework include:
- Missing or invalid paramters to controller actions no longer throw HTTP 500 errors.
- Minimal use of reflection and attributes.
- No dependency on other libraries, other than `System.Web.Razor`.

This library is not intended to be a replacement of the official framework; rather it is a light-weight alternative for projects such as API servers and simple web portals, where the full set of MVC features is unnecessary.

# How to use
## Basic setup
This setup is the same for API server projects (i.e. the project does not render HTML responses using a view) and MVC projects.

### Project References
Create an empty ASP.NET project in Visual Studio. Add `Finale.WebServer` to the references. Then remove everything else except the following:
- Microsoft.CSharp
- System
- System.Core
- System.Web

### web.config
```<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.web>
    <compilation debug="true" targetFramework="4.5" />
    <httpRuntime targetFramework="4.5" />
  </system.web>
</configuration>
```

### Routes
Add `Global.asax` to the project and configure your routes. Add `using Finale.WebServer` to use the `MapRoute` extension method. The routing follows standard ASP.NET conventions.
```
using Finale.WebServer;
using System;
using System.Web.Routing;

namespace WebServerExample {
    public class Global : System.Web.HttpApplication {
        protected void Application_Start(object sender, EventArgs e) {
            RouteTable.Routes.MapRoute(
                "{*anything}",
                defaults: new { controller = "Default", action = "Default" }
            );
        }
    }
}
```

### Controllers
Create a controller by adding a class which inherits `Finale.WebServer.Controller`. Define action handlers by prepanding the HTTP method before the action name. E.g. if the action name is "users" and the method handles POST requests, define the method as `public void PostUsers()`. Method names are not case sensitive.
```
public class DefaultController : Controller {
    public void GetDefault() {
        Response.ContentType = "text/plain";
        Response.Write("Hello World from Default Controller");
    }
}
```

Override the `PreExecute()` and `PostExecute()` methods for implementing logic that happens before and/or after the action method. Override the `MethodNotFound()` method to implement the behavior when the request is routed to this controller but no action handler can be found.

[Here is an example of Controller](https://github.com/finalesoft/webserver/blob/main/WebServerExample/DefaultController.cs)

## MVC setup
### System.Web.Razor reference
Add `System.Web.Razor v1.0.0.0` to the project referenecs. The version must be v1.0.0.0, and the dll must be copied to the output directory during compilation. Avoid using the built-in MVC functions of Visual Studio, since Visual Studio sometimes replaces the DLL with v2 or v3 and mess up web.config after certain MVC operations.

### MVC controllers
For controllers that render views, they should inherit from `Finale.WebServer.Mvc.MvcController` instead.

Use `RenderView()` in the action handler method to render the view to the Response output stream. The `ViewData` dictionary can be used to pass data to the view.
```
public class DefaultController : MvcController {
    public void GetDefault() {
        ViewData["value"] = "123";

        RenderView();
    }
}
```
[Here is an example of MvcController](https://github.com/finalesoft/webserver/blob/main/MvcExample/Controllers/DefaultController.cs)

        
### Views
Views are located in the usual MVC folders, i.e. `/Views/{ControllerName}/{ActionName}.cshtml`. Only C#-based razor pages are supported.

Different from standard ASP.NET MVC views, the baseclass of the view must be specified:
```
@inherits Finale.WebServer.Mvc.RazorPage
<!DOCTYPE html>

<html>
<head>
...
```

To use a view with a model, use the generic version of the baseclass:
```
@inherits Finale.WebServer.Mvc.RazorPage<System.Collections.Generic.List<string>>
```

- To specify the layout page, set the Layout property at the top of the view.
- To define sections, use the `@section` directive.
- To render strings directly to the page as HTML (i.e. without encoding), use `@RenderHtml()`

```
@inherits Finale.WebServer.Mvc.RazorPage
@{Layout = "~/Views/Shared/_Layout.cshtml";}

<p>This line is from body page</p>
<p>ViewData value is @ViewData["value"]</p>
@RenderHtml("<p>This line is rendered as raw html string</p>")

@section scripts{
    <p>This line is from the "scripts" section.</p>
    <script>console.log("This script is written from the \"scripts\" section.");</script>
}
```

## Examples
- See [WebServerExample](https://github.com/finalesoft/webserver/tree/main/WebServerExample) for an example for API servers.
- See [MvcExample](https://github.com/finalesoft/webserver/tree/main/MvcExample) for an example with razor views.
