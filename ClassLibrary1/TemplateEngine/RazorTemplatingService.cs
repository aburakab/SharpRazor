using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using TheModel;

namespace SharpRazorEngine.TemplateEngine
{
    public class RazorTemplatingService
    {
        protected readonly IRazorViewEngine _viewEngine;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ITempDataProvider _tempDataProvider;
        protected readonly RazorCompiledViewsHelper _compiledViewsHelper;

        public RazorTemplatingService(IRazorViewEngine viewEngine,
            IServiceProvider serviceProvider,
            ITempDataProvider tempDataProvider,
            RazorCompiledViewsHelper compiledViewsHelper)
        {
            _viewEngine = viewEngine;
            _serviceProvider = serviceProvider;
            _tempDataProvider = tempDataProvider;
            _compiledViewsHelper = compiledViewsHelper;
        }

        public virtual async Task<string> Parse<TModel>(string viewName, TModel model)
        {
            return await Parse("MasterLayoutModel", viewName, model);
        }

        protected async Task<string> Parse<TModel>(string masterLayoutViewDataKey, string viewName, TModel model)
        {
            var actionContext = GetActionContext();
            var view = FindView(actionContext, viewName);

            var masterModel = GetMasterLayoutDto();

            await using var output = new StringWriter();

            var viewContext = new ViewContext(
                actionContext,
                view,
                new ViewDataDictionary(
                    metadataProvider: new EmptyModelMetadataProvider(),
                    modelState: new ModelStateDictionary()),
                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider), output, new HtmlHelperOptions())
                {
                    ViewData =
                    {
                        [masterLayoutViewDataKey] = masterModel,
                        Model = model
                    }
                };

            await view.RenderAsync(viewContext);

            return output.ToString();
        }

        protected object GetMasterLayoutDto()
        {
            return new MasterViewModel
            {
                Name = "Test Master Model"
            };
        }

        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
        private IView FindView(ActionContext actionContext, string viewName)
        {
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var viewPath = _compiledViewsHelper.TemplateViews[viewName];

            viewPath = "~" + viewPath;
            var getViewResult = _viewEngine.GetView(executingFilePath: viewPath, viewPath: viewPath, isMainPage: true);

            if (getViewResult.Success)
            {
                return getViewResult.View;
            }

            var findViewResult = _viewEngine.FindView(actionContext, viewPath, isMainPage: true);
            if (findViewResult.Success)
            {
                return findViewResult.View;
            }

            throw ThrowViewNotFoundException(viewName, getViewResult, findViewResult);
        }
        private static RazorTemplateNotFoundException ThrowViewNotFoundException(string viewName, ViewEngineResult getViewResult, ViewEngineResult findViewResult)
        {
            var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations)); ;

            return new RazorTemplateNotFoundException(errorMessage);
        }

        private static RazorTemplateNotFoundException ThrowViewNotFoundException(string viewName, string viewPath)
        {
            var errorMessage = $"Unable to find view '{viewName}'. The following locations were searched: '{viewPath}'";
            return new RazorTemplateNotFoundException(errorMessage);
        }
    }
}
