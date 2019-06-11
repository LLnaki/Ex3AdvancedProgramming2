using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Text;
namespace Ex3.Models
{
    /**
     * This class is a model(in mvc pattern), providing functionality and data
     * about a current flight of a plane in "FlightGear" flight simulator.
     */
    public class Model
    {
        //Default path of directory with files which consist data about a flight.
        private const string SCENARIO_FILE = "~/App_Data/{0}.txt";
        /*
         * This client is responsible for communication with a server on flight simulator, 
         * which sends data to the application.
         * */
        private TcpClient Client { get; set; }
        //Indicates whether the Client is connected to the simulator or not.
        private bool IsConnectedToSimulator {get; set;}
        //A writer which writes to the stream of connection between this model and the simulator.
        private BinaryWriter Writer { set; get; }
        //A reader which read from the stream of connection between this model and the simulator.
        private StreamReader Reader { set; get; }
        //The stream of connection between this model and the simulator.
        private NetworkStream SimulatorStream { set; get; }
        //Will be used for synchronization(when a number of users read/write info to the model)
        private static Mutex mutex = new Mutex();
        //Longtitude of a plane in a current flight.
        public string Lon
        {
            get
            {
                ExecuteCommand("get /position/longitude-deg\r\n");
                string str = ReadDataFromClient();
                string[] arr = str.Split(new char[] { '\'', '\'' }, StringSplitOptions.None);
                return arr[1];
            }
        }
        //Latitude of a plane in a current flight.
        public string Lat
        {
            get
            {
                ExecuteCommand("get /position/latitude-deg\r\n");
                string str = ReadDataFromClient();
                //the simulator sends data in the format which requires this spliting.
                string[] arr = str.Split(new char[] { '\'', '\'' }, StringSplitOptions.None);
                return arr[1];
            }
        }
        //Throttle value of a plane in a current flight.
        public string Throttle
        {
            get
            {
                ExecuteCommand("get controls/engines/current-engine/throttle\r\n");
                string str = ReadDataFromClient();
                //the simulator sends data in the format which requires this spliting.
                string[] arr = str.Split(new char[] { '\'', '\'' }, StringSplitOptions.None);
                return arr[1];
            }
        }
        //Rudder value of a plane in a current flight.
        public string Rudder
        {
            get
            {
                ExecuteCommand("get controls/flight/rudder\r\n");
                string str = ReadDataFromClient();
                //the simulator sends data in the format which requires this spliting.
                string[] arr = str.Split(new char[] { '\'', '\'' }, StringSplitOptions.None);
                return arr[1];
            }
        }
        /*
         * Connectis to a server of the simulator which is located on given ip address and port.
         * This function runs untill the connection is established succesfully.
         * */
        public void Connect(string ip, int port)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            while (!IsConnectedToSimulator)
                {
                    try
                    {
                        Client = new TcpClient();
                        Client.Connect(iPEndPoint);
                        SimulatorStream = Client.GetStream();
                        Writer = new BinaryWriter(SimulatorStream);
                        Reader = new StreamReader(SimulatorStream);
                        IsConnectedToSimulator = true;
                    }
                    catch (Exception) { }
                }
        }

      /**
       * This function executes a passed command(as a string) in the simulator.
       */
        private void ExecuteCommand(string command)
        {
            /*
             * blocking with mutex in order to prevent cases in which several users send commands
             * and results written by the simulator to the connection streak interfer each other and mix
             * the information. The mutex will be realized when reading a result of an executed command.
             */
            mutex.WaitOne();
            if (IsConnectedToSimulator)
            {
                Writer.Write(Encoding.ASCII.GetBytes(command));
                Writer.Flush();
            }
        }
        /*
         * This function reads data(as a string) from the connection stream.
         */
        private string ReadDataFromClient()
        {

            string readStr;
            while ((readStr = Reader.ReadLine()) == "") ;
            /*
             * To ensure that no trash is left and nothing will be interferring
             * next reading/writing of data to the stream.
             */
            SimulatorStream.Flush();
            //realizing mutex which was closed by the writing to the stream function.
            mutex.ReleaseMutex();
            return readStr;
            
        }

        /*
         * This function saves information about a flight to a path specified in a field "SCENARIO_FILE". 
         * If a file with such a name(a name passed as a parameter) already exists, then its content will be
         * and a new information about a flight will be written to it.
         */
        public static void SaveFlightInfo(string infoJson, string fileName)
        {
            string path = HttpContext.Current.Server.MapPath(String.Format(SCENARIO_FILE, fileName));
            if (File.Exists(path))
                File.Delete(path);
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(infoJson);
                sw.Close();
            }
        }

        //This function loads a content from a file located in directory "SCENARIO_FILE" and returns it as string.
        public static string LoadFlightInfo(string fileName)
        {
            string path = HttpContext.Current.Server.MapPath(String.Format(SCENARIO_FILE, fileName));
            if (!File.Exists(path))
                throw new IOException("LoadFlightInfo tries to load a file which doesn't exist");
            return File.ReadAllText(path);
        }
        //This function closes the connection with simulator.
        private void DisconectFromSimulator()
        {
            //if the connection was fully established.
            if (IsConnectedToSimulator)
            {
                Client.Close();
            }


        }
        ~Model()
        {
            DisconectFromSimulator();
        }
    }
}