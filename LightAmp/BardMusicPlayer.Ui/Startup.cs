using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace BardMusicPlayer.Ui
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes(); // Add this line at the top of Configuration()
            config.Routes.MapHttpRoute(
                 name: "DefaultApi",
                 routeTemplate: "{controller}/{id}",
            defaults: new
            {
                controller = "playback",
                id = RouteParameter.Optional
            }
             );
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings =
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            app.UseWebApi(config);
        }
    }
}
