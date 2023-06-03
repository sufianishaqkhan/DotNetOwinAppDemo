using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using Swashbuckle.Application;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http;
using System.Xml.Linq;

[assembly: OwinStartup("DevConfiguration", typeof(OwinAppDemo.Startup))]

namespace OwinAppDemo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();

            ConfigureCors(app);

            ConfigureWebApi(httpConfig, app);

            ConfigureSwashbucket(httpConfig);

            app.Run(RedirectToSwaggerUi);
        }

        private void ConfigureCors(IAppBuilder app)
        {
            //app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            var policy = new CorsPolicy
            {
                AllowAnyHeader = true,
                AllowAnyMethod = true,
                AllowAnyOrigin = true,
                SupportsCredentials = true
            };

            policy.ExposedHeaders.Add("X-Pagination");
            app.UseCors(new CorsOptions
            {
                PolicyProvider = new CorsPolicyProvider
                {
                    PolicyResolver = context => Task.FromResult(policy)
                }
            });
        }

        private void ConfigureWebApi(HttpConfiguration httpConfig, IAppBuilder app)
        {
            ConfigureRoutes(httpConfig);

            ConfigureFormatters(httpConfig);

            app.UseWebApi(httpConfig);
        }

        private void ConfigureRoutes(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }

        private void ConfigureFormatters(HttpConfiguration config)
        {
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();

            jsonFormatter.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            jsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;

            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        private void ConfigureSwashbucket(HttpConfiguration httpConfiguration)
        {
            httpConfiguration
                .EnableSwagger(c =>
                {
                    c.ApiKey("apiKey")
                        .Description("API Key Authentication")
                        .Name("ApiKey")
                        .In("header");

                    c.SingleApiVersion("v1", "OwinAppDemo.API");

                    string appdata = AppDomain.CurrentDomain.BaseDirectory + "bin\\app_data";
                    string swaggerFile = appdata + "\\swagger.xml";

                    if (!File.Exists(swaggerFile))
                    {
                        swaggerFile = GetCombinedApiDocumentionFile();
                    }

                    c.IncludeXmlComments(swaggerFile);
                })
                .EnableSwaggerUi(c =>
                {
                    c.EnableApiKeySupport("ApiKey", "header");
                });
        }

        private static string GetCombinedApiDocumentionFile()
        {
            XElement xml = null;
            string appdata = AppDomain.CurrentDomain.BaseDirectory + "bin\\app_data";
            string[] files = Directory.GetFiles(appdata);

            foreach (string fileName in files)
            {
                string qualifiedFileName = fileName;
                if (xml == null)
                {
                    xml = XElement.Load(qualifiedFileName);
                }
                else
                {
                    var dependentXml = XElement.Load(qualifiedFileName);
                    foreach (XElement ele in dependentXml.Descendants())
                    {
                        xml.Add(ele);
                    }
                }
            }

            string swaggerFile = null;
            if (xml != null)
            {
                swaggerFile = appdata + "\\swagger.xml";

                if (File.Exists(swaggerFile))
                {
                    File.Delete(swaggerFile);
                }

                xml.Save(swaggerFile);
            }

            return swaggerFile;
        }

        private async Task RedirectToSwaggerUi(IOwinContext context)
        {
            await Task.Run(() => context.Response.Redirect(ConfigurationManager.AppSettings["RootUrl"] + "/swagger/ui/index"));
        }
    }
}
