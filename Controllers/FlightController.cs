using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Ex3.Models;
namespace Ex3.Controllers
{
    public class FlightController : Controller
    {
        /*
         * This dictionary is a mapping from ip address and port, stored as string in a format "<ip>,<port>"
         * to models that represents information about a current flight in flight simulator.
         * ip and port - a network location of a certain simulator. For each simulator(different location)
         * will be used different Model object and thus we get independent connection with every location.
         */
       private static Dictionary<string,  Model> fromIpPortToModel = new Dictionary<string, Model>();
      

        //first mission-display
        [HttpGet]
        ///<summary>
        ///This function is an action, which returns a view that displays a current location of a plane in the flight
        ///simulator on a map.
        ///</summary>
        ///<param name="ip">
        ///ip of a network location of the simulator
        ///</param>
        ///<param name="port">
        ///port of a network location of the simulator
        /// </param>
        public ActionResult DisplayCurrentPlanePossition(string ip, int port)
        {
            IPAddress ipParsed;
            //If it is not ip, then it can be call for another action which uses the same number and types of parameters:
            //"LoadPathFromFileAndDisplayIt".
            if (!IPAddress.TryParse(ip, out ipParsed))
                //If the call was intended for  LoadPathFromFileAndDisplayIt, so we will redirect it to there.
                return RedirectToAction("LoadPathFromFileAndDisplayIt", new { fileName = ip, numPerSeconds = port });
            string key = ip + "," + port.ToString();
            //If a connection with a server of a simulator with such ip and port yet has'nt been established,
            // we will establish it.
            if (!fromIpPortToModel.ContainsKey(key))
            {
                Model model = new Model();
                model.Connect(ip, port);
                fromIpPortToModel[key] = model;
            }


            Session["xOfPlane"] = fromIpPortToModel[key].Lon;
            Session["yOfPlane"] = fromIpPortToModel[key].Lat;
            //A view should have a key in order to communicate with appropriate model throught this controller further.
            Session["key"] = key;
            return View();
        }


        //second mission-display
        [HttpGet]
        ///<summary>
        ///This function is an action, which returns a view that displays as animation a current flight of a plane in
        ///the simulator on 2d world map.
        /// </summary>
        ///<param name="ip">
        ///ip of a network location of the simulator
        ///</param>
        ///<param name="port">
        ///port of a network location of the simulator
        /// </param>
        /// <param name="numPerSeconds">
        /// a frequency  of updating a path in the animation as times per second.
        /// </param>
        public ActionResult DisplayPathContinuously(string ip, int port, int numPerSeconds)
        {
            string key = ip + "," + port.ToString();
            //If it is not ip, then it can be call for another action which uses the same number and types of parameters:
            //"LoadPathFromFileAndDisplayIt".
            if (!fromIpPortToModel.ContainsKey(key))
            {
                Model model = new Model();
                model.Connect(ip, port);
                fromIpPortToModel[key] = model;
            }
            Session["xOfPlane"] = fromIpPortToModel[key].Lon;
            Session["yOfPlane"] = fromIpPortToModel[key].Lat;
            Session["numPerSeconds"] =  numPerSeconds;
            //A view should have a key in order to communicate with appropriate model throught this controller further.
            Session["key"] = key;
            return View();
        }

        //third mission-save
        [HttpGet]
        ///<summary>
        ///This function is an action which returns a view which tracks a path information of a current flight in flightGear simulator,
        ///shows this path as animation and then sends to the server(to model throught this controller) 
        ///a path information as string in order to store it in a file.
        ///</summary>
        public ActionResult TrackThePathAndSaveItToFile(string ip, int port, int numPerSeconds, int numOfSeconds, string fileName)
        {
            string key = ip + "," + port.ToString();
            if (!fromIpPortToModel.ContainsKey(key))
            {
                Model model = new Model();
                model.Connect(ip, port);
                fromIpPortToModel[key] = model;
            }
            Session["numPerSeconds"] = numPerSeconds;
            Session["seconds"] = numOfSeconds;
            //A view should have a key in order to communicate with appropriate model throught this controller further.
            Session["key"] = key;

            //The first sampling of flight data.
            Session["xOfPlane"] = fromIpPortToModel[key].Lon;
            Session["yOfPlane"] = fromIpPortToModel[key].Lat;
            Session["fileName"] = fileName;
            Session["throttle"] = fromIpPortToModel[key].Throttle;
            Session["rudder"] = fromIpPortToModel[key].Rudder;
            return View();
        }


        //fourth mission-display
        [HttpGet]
        ///<summary>
        ///This function is an action which loads a path from a file and returns a view that shows it.
        ///</summary>
        public ActionResult LoadPathFromFileAndDisplayIt(string fileName, int numPerSeconds)
        {
            Session["numPerSeconds"] = numPerSeconds;
            ViewBag.info = Model.LoadFlightInfo(fileName);
            return View();
        }


        [HttpPost]
        ///<summary>
        ///This function gets from the model a current plane possition(longtitude and latitude)
        ///and returns it.
        ///</summary>
        ///<param name="key">
        ///the key for dictionary "fromIpPortToModel", containing models. 
        ///This dictionary maps from a key to appropriate to it model.
        ///</param>
        public string UpdateXandYofPlane(string key)
        {
            string x = fromIpPortToModel[key].Lon;
            string y = fromIpPortToModel[key].Lat;
            return x + "," + y;

        }
        /// <summary>
        /// asks a model to save a passed as a string flight information.
        /// </summary>
        /// <param name="key">
        ///the key for dictionary "fromIpPortToModel", containing models. 
        ///This dictionary maps from a key to appropriate to it model.
        /// </param>
        /// <param name="infoJson">
        /// An information about a flight in Json format.
        /// </param>
        /// <param name="fileName">
        /// name of file to which the information will be written. If the file exists, its
        /// content will be rewritten. If doesn't exist, it will be created and the info
        /// will be written to it.
        /// </param>
        public void SaveFlightInfo(string key, string infoJson,string fileName)
        {
            Model.SaveFlightInfo(infoJson,fileName);
        }
        [HttpPost]
        ///<summary>
        ///This function gets from the model an information about a current possition of a plane.
        ///in a current flight.)
        ///and returns it.
        ///</summary>
        ///<param name="key">
        ///the key for dictionary "fromIpPortToModel", containing models. 
        ///This dictionary maps from a key to appropriate to it model.
        ///</param>
        public string GetInfoAboutPlainKey(string key)
        {
            string result = UpdateXandYofPlane(key);
            result += ",";
            result += fromIpPortToModel[key].Throttle + ",";
            result += fromIpPortToModel[key].Rudder;
            return result;
        }

        [HttpPost]
        ///<summary>
        /// This function is an action that gets an information about some flight of a plane in the flight simulator
        /// and returns it as a json string.
        /// for each information unit(that is lot, lan, rudder an throttle), it stored as CSV in the next form:
        /// "valueInSampe1,valueInSample2,valueInSample3,...,valueInSample4" therefore, for example,
        /// if we want all information about a plane in a sample number 3,
        /// so we should get third value from each unit, which is a string.
        /// For example, if all units are:
        /// lot: 1,2,3,4,5,6
        /// lan: 2,3,4,5,6,7
        /// rudder: 3,4,5,6,7,8
        /// throttle: 4,5,6,7,8,9
        /// Then for third sample information is: 
        /// lot=3, lan = 4, rudder = 5, throttle = 6.
        /// 
        ///</summary>
        ///<param name="key">
        ///the key for dictionary "fromIpPortToModel", containing models. 
        ///This dictionary maps from a key to appropriate to it model.
        ///</param>
        ///<param name="fileName">
        ///A name of a file from which the information will be retrived.
        ///</param>
        public string GetInfoAboutPlain(string key, string fileName)
        {
            return Model.LoadFlightInfo(fileName);
        }
    }

}