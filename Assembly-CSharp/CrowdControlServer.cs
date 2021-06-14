using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using LM2RandomiserMod.Patches;

namespace LM2RandomiserMod.CrowdControl {


    public class CrowdControlServer : MonoBehaviour {
        #if DEV
        private DevUI DevUI;
#endif

        #region server info
        private TcpClient socketConnection;
        private Thread clientReceiveThread;
        #endregion
        #region game info
        private patched_L2System sys;
        #endregion
        #region statuses
        private int Success = 0;
        private int Failed = 1;
        private int NotAvail = 2;
        private int TempFail = 3;
        #endregion

        private class CrowdControlMessage
        {
            public int id { get; set; }
            public string code { get; set; }
            public string viewer { get; set; }
            public int type { get; set; }
            public string parameters { get; set; }

        }

        public CrowdControlServer (patched_L2System system) {
            sys = system;
            this.Start();
        }

        // Use this for initialization
        void Start()
        {
            ConnectToTcpServer();
        }
#if DEV
        public void AddDebug(DevUI devUI)
        {
            this.DevUI = devUI;
        }
#endif
        /// <summary> 	
        /// Setup socket connection. 	
        /// </summary> 	
        private void ConnectToTcpServer()
        {
            try
            {
                clientReceiveThread = new Thread(new ThreadStart(ListenForData));
                clientReceiveThread.IsBackground = true;
                clientReceiveThread.Start();
            }
            catch (Exception e)
            {
                Debug.Log("On client connect exception " + e);
            }
        }

        /// <summary> 	
        /// Runs in background clientReceiveThread; Listens for incomming data. 	
        /// </summary>     
        private void ListenForData()
        {
            try
            {
                socketConnection = new TcpClient("localhost", 43384);
                Byte[] bytes = new Byte[1024];
#if DEV
                this.DevUI.ccConnected = "CC Found";
#endif
                while (true)
                {
                    // Get a stream object for reading 				
                    using (NetworkStream stream = socketConnection.GetStream())
                    {
                        int length;
                        // Read incomming stream into byte arrary. 					
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);
                            // Convert byte array to string message. 						
                            string serverMessage = Encoding.ASCII.GetString(incommingData);
#if DEV
                            this.DevUI.ccMessage = serverMessage;
#endif
                            this.handleMessage(serverMessage);
                        }
                    }
                }
            }
            catch (Exception socketException)
            {
                Debug.Log("Socket exception: " + socketException);
#if DEV
                this.DevUI.ccMessage = socketException.ToString();
#endif
            }
        }
       
        /// <summary> 	
        /// Send message to client using socket connection. 	
        /// </summary> 	
        private void SendMessage(int id, int status)
        {
            if (socketConnection == null)
            {
                return;
            }

            try
            {
                // Get a stream object for writing. 			
                NetworkStream stream = socketConnection.GetStream();
                if (stream.CanWrite)
                {
                string serverMessage = $"{{\"id\":{id},\"status\":{status}}}";
                    // Convert string message to byte array.                 
                    byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                    // Write byte array to socketConnection stream.               
                    stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log("Socket exception: " + socketException);
            }
        }

        private void handleMessage(string message) {
            CrowdControlMessage ccEvent = this.ParseJson(message);
#if DEV
            this.DevUI.UpdateCCMessage(ccEvent.id, ccEvent.code, ccEvent.type);
#endif
            this.doEffect(ccEvent);
        }

        private void doEffect(CrowdControlMessage ccEvent)
        {
            int status = this.Failed;
            switch(ccEvent.code)
            {
                case "trip":
                    sys.getPlayer().changeCostume(3);
                    sys.getPlayer().setPlayerAnimeOverride(L2Base.Charcter.PLAYERANIME.dead, 0);
                    status = this.Success;
                    break;
                default:
                    break;
            }
            this.SendMessage(ccEvent.id, status);
        }

        private CrowdControlMessage ParseJson(string msg)
        {
            return JsonUtility.FromJson<CrowdControlMessage>(msg);
        }
    }
}