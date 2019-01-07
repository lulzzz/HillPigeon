﻿using HillPigeon.ApplicationModels;
using HillPigeon.Orleans.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using System.Linq;

namespace HillPigeon.Orleans.Core
{
    public class OrleansControllerModelConvention : IControllerModelConvention
    {
        private readonly OrleansRouteingOptions _options;
        public OrleansControllerModelConvention(IOptions<OrleansRouteingOptions> options)
        {
            _options = options.Value;
        }
        public void Apply(ControllerModel controllerModel)
        {
            if (!controllerModel.ControllerType.IsGrain())
            {
                return;
            }
            if (_options.ControllerNameRule != null)
            {
                controllerModel.ControllerName = _options.ControllerNameRule(controllerModel);
            }
            if (controllerModel.Attributes.Where(attr => typeof(IRouteTemplateProvider).IsAssignableFrom(attr.GetType())).Count() == 0)
            {
                var routeAttr = new RouteAttribute(controllerModel.ControllerName);
                controllerModel.Attributes.Add(routeAttr);
            }
        }
    }
}
