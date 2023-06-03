using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace OwinAppDemo.Controller.ActionFilters
{
    public class APIKeyAuthorization : AuthorizeAttribute
    {
        private const string ApiKeyHeaderName = "ApiKey";

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            IEnumerable<string> apiKeyHeaderValues = null;

            if (actionContext.Request.Headers.TryGetValues(ApiKeyHeaderName, out apiKeyHeaderValues))
            {
                bool isValidAPIKey = false;
                IEnumerable<string> lsHeaders;

                var checkApiKeyExists = actionContext.Request.Headers.TryGetValues(ApiKeyHeaderName, out lsHeaders);

                if (checkApiKeyExists)
                {
                    if (lsHeaders.FirstOrDefault().Equals(ConfigurationManager.AppSettings["ApiKey"]))
                    {
                        isValidAPIKey = true;
                    }
                }

                if (!isValidAPIKey)
                {
                    return false;
                }

                return true;
            }
            return base.IsAuthorized(actionContext);
        }
    }
}