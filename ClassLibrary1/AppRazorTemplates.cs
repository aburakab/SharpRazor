using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using SharpRazorEngine.TemplateEngine;

namespace SharpRazorEngine;

public static class AppRazorTemplates
{
    public static IServiceCollection AddSharpRazor(
        this IServiceCollection services,
        IEnumerable<string> viewsAssemblyPaths)
    {
        var listener = new DiagnosticListener(typeof(AppRazorTemplates).Assembly.FullName); // "Microsoft.AspNetCore"
        services.AddSingleton<DiagnosticSource>(listener);
        services.AddSingleton<DiagnosticListener>(listener);
        services.AddSingleton<IRazorViewEngine, MyRazorViewEngine>();
        services.AddSingleton<RazorTemplatingService>();
        
        var builder = services.AddRazorPages();

        foreach (var assemblyPath in viewsAssemblyPaths)
        {
            var viewsAssembly = GetViewsAssembly(assemblyPath);
            builder.AddApplicationPart(viewsAssembly);
        }

        services.AddSingleton<RazorCompiledViewsHelper>();

        return services;
    }

    private static Assembly GetViewsAssembly(string emailViewsAssemblyLocation)
    {
        var viewsDll = emailViewsAssemblyLocation;
        var viewsAssembly = Assembly.Load(File.ReadAllBytes(viewsDll));
        return viewsAssembly;
    }
}