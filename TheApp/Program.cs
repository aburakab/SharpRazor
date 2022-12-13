using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharpRazorEngine;
using SharpRazorEngine.TemplateEngine;
using TheModel;
using TheViews;

Console.WriteLine("Hello, SharpRazor!");

var host = new HostBuilder().ConfigureServices((_, services) =>
{
    services.AddSharpRazor(new List<string>
    {
        typeof(TheViewsAssemblyMarker).Assembly.Location
    });

}).Build();

var engine = host.Services.GetRequiredService<RazorTemplatingService>();

var html = await engine.Parse("MyView.cshtml", new MyViewModel
{
    Id = 10,
    Name = "Ali"
});

Console.WriteLine(html);