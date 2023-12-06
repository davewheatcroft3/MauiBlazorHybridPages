namespace MauiBlazorHybridPages.Navigation
{
    public record MatchedRoute(string Name, Dictionary<string, object> Parameters);

    public class HybridPagesRouteManager
    {
        private List<RouteMapping> _routeRegistry = new();

        public void Register(string appShellRouteName, string razorTemplateRoute)
        {
            _routeRegistry.Add(new RouteMapping(appShellRouteName, razorTemplateRoute));
        }

        public MatchedRoute? MatchFromRazorRoute(string razorStartupRoute)
        {
            // TODO: allow override mechanism to accept or reject match from outside package
            var baseUri = razorStartupRoute;
            var parametersStartIndex = razorStartupRoute.IndexOf("?");
            var queryParameters = string.Empty;
            if (parametersStartIndex >= 0)
            {
                baseUri = razorStartupRoute.Substring(0, parametersStartIndex);
                queryParameters = razorStartupRoute.Substring(parametersStartIndex + 1);
            }

            var componentParts = baseUri
                .TrimStart('/')
                .TrimEnd('/')
                .Split("/");

            foreach (var registeredRoute in _routeRegistry)
            {
                var routeComponentParts = registeredRoute.RazorTemplateRoute
                    .TrimStart('/')
                    .TrimEnd('/')
                    .Split("/");

                if (routeComponentParts.Length != componentParts.Length)
                {
                    // Not same length, dont match
                    continue;
                }

                var variables = new Dictionary<string, object>();
                var partDoesntMatch = false;
                for (var i = 0; i < routeComponentParts.Length; i++)
                {
                    var componentPart = componentParts[i];
                    var routeComponentPart = routeComponentParts[i];
                    
                    if (routeComponentPart.StartsWith("{"))
                    {
                        // Its a variable, add to variable list
                        var variableName = routeComponentPart.Replace("{", "").Replace("}", "");
                        var variableType = "string";
                        if (variableName.Contains(":"))
                        {
                            var variableInfoSplit = variableName.Split(':');
                            variableName = variableInfoSplit[0];
                            variableType = variableInfoSplit[1];
                        }

                        object variableValue = componentPart;
                        if (variableType != "string")
                        {
                            // TODO: support other types...
                            if (variableType == "int")
                            {
                                variableValue = int.Parse(componentPart);
                            }
                        }

                        variables.Add(variableName, variableValue);
                    }
                    else
                    {
                        // Its a route part, if they dont match, abort loop for this route and move to next
                        if (componentPart != routeComponentPart)
                        {
                            partDoesntMatch = true;
                            break;
                        }
                    }
                }
                
                if (partDoesntMatch)
                {
                    continue;
                }

                // Add on query parameters
                queryParameters = queryParameters.Replace("?", "");
                var querySplit = queryParameters.Split("&");
                foreach (var queryParameterPair in querySplit)
                {
                    var queryParameterSplit = queryParameterPair.Split("=");
                    var variableName = queryParameterSplit[0];
                    var variableValue = queryParameterSplit[1];
                    variables.Add(variableName, variableValue);
                }

                return new MatchedRoute(registeredRoute.AppShellRouteName, variables);
            }

            return null;
        }

        public static string MatchParametersToRouteTemplate(string razorRouteTemplatePath, IDictionary<string, object> queryParameters)
        {
            var templatePath = razorRouteTemplatePath;

            // TODO: adding error handling and maybe abstract some of the query code...? (This and above method)
            var addQueryParamCharacter = "?";
            foreach (var keyValue in queryParameters)
            {
                var startIndex = templatePath.IndexOf("{" + keyValue.Key);
                if (startIndex >= 0)
                {
                    var lengthToEnd = 0;
                    while (templatePath[startIndex + lengthToEnd] != '}')
                    {
                        lengthToEnd++;
                    }

                    var subString = templatePath.Substring(startIndex, lengthToEnd + 1);
                    var trimmed = subString
                        .TrimStart('{')
                        .TrimEnd('}');

                    if (trimmed.Contains(":"))
                    {
                        var split = trimmed.Split(":");
                        var variableName = split[0];
                        templatePath = templatePath.Replace(subString, keyValue.Value.ToString());
                    }
                    else
                    {
                        templatePath = templatePath.Replace($"{{{keyValue.Key}}}", keyValue.Value.ToString());
                    }
                }
                else
                {
                    templatePath += $"{addQueryParamCharacter}{keyValue.Key}={keyValue.Value}";
                    if (addQueryParamCharacter == "?")
                    {
                        addQueryParamCharacter = "&";
                    }
                }
            }

            return templatePath;
        }

        private record RouteMapping(
            string AppShellRouteName,
            string RazorTemplateRoute);
    }
}