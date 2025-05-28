using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace McpNetwork.Charli.Server.Managers.WebService.WebServer
{
    internal class CharliModelConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            var centralPrefix = new AttributeRouteModel(new AttributeRouteModel() { Template = "api/{version}" });
            foreach (var controller in application.Controllers)
            {
                var routeSelector = controller.Selectors.FirstOrDefault(x => x.AttributeRouteModel != null);

                if (routeSelector != null)
                {
                    routeSelector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(centralPrefix, routeSelector.AttributeRouteModel);
                }
                else
                {
                    controller.Selectors.Add(new SelectorModel() { AttributeRouteModel = centralPrefix });
                }
            }
        }
    }
}
