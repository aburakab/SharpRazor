using System.Diagnostics;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SharpRazorEngine.TemplateEngine;

public class MyRazorViewEngine : RazorViewEngine
{
    public MyRazorViewEngine(
        IRazorPageFactoryProvider pageFactory,
        IRazorPageActivator pageActivator,
        HtmlEncoder htmlEncoder,
        IOptions<RazorViewEngineOptions> optionsAccessor,
        ILoggerFactory loggerFactory,
        DiagnosticListener diagnosticListener) :
        base(pageFactory, pageActivator, htmlEncoder, optionsAccessor, loggerFactory, diagnosticListener)
    {
    }
}