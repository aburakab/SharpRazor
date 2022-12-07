using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;

namespace SharpRazorEngine.TemplateEngine;

public class RazorCompiledViewsHelper
{
    private readonly ApplicationPartManager _applicationPartManager;

    public IDictionary<string, string> TemplateViews { get; set; } = new Dictionary<string, string>();

    public RazorCompiledViewsHelper(ApplicationPartManager applicationPartManager)
    {
        _applicationPartManager = applicationPartManager;

        var feature = new ViewsFeature();
        _applicationPartManager.PopulateFeature(feature);
        foreach (var item in feature.ViewDescriptors)
        {
            int pos = item.RelativePath.LastIndexOf("/") + 1;
            var key = item.RelativePath.Substring(pos, item.RelativePath.Length - pos);
            TemplateViews[key] = item.RelativePath;
        }
    }
}