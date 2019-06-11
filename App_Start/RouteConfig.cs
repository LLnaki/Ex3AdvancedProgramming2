using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ex3
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("DisplayCurrentPlanePossition", "display/{ip}/{port}",
             defaults: new { controller = "Flight", action = "DisplayCurrentPlanePossition" }
         );

            routes.MapRoute(
             "DisplayPathContinuously", "display/{ip}/{port}/{numPerSeconds}",
             defaults: new { controller = "Flight", action = "DisplayPathContinuously" }
         );

            routes.MapRoute(
            "TrackThePathAndSaveItToFile", "save/{ip}/{port}/{numPerSeconds}/{numOfSeconds}/{fileName}",
            defaults: new { controller = "Flight", action = "TrackThePathAndSaveItToFile" }
        );

            routes.MapRoute(
            "LoadPathFromFileAndDisplayIt", "{action}/{fileName}/{numPerSeconds}",
            defaults: new { controller = "Flight", action = "LoadPathFromFileAndDisplayIt" }
        );
  
            //Default.
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Flight", action = "Index", id = UrlParameter.Optional }
            );





        }
    }
}
