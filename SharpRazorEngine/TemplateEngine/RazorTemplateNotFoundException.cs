using System;

namespace SharpRazorEngine.TemplateEngine;

public class RazorTemplateNotFoundException : Exception
{
    public RazorTemplateNotFoundException(string message) : base(message)
    {
    }
}