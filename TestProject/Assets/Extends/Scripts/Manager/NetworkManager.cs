using LuaInterface;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace CFramework
{
    public class NetworkManager : BaseManager<NetworkManager>
    {
        private static byte[] _result = new byte[1024];
        private static Socket _clientSocket;
        private static LuaFunction _onDisconnectCallback;
        private static LuaFunction _onReceiveMessageCallback;
        //是否已连接的标识  
        public bool IsConnected = false;

        public override void Init()
        {
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>  
        /// 连接指定IP和端口的服务器  
        /// </summary>  
        /// <param name="ip"></param>  
        /// <param name="port"></param>  
        public void Connect(string ip, int port, LuaFunction successCallback, LuaFunction failCallback, LuaFunction onDisconnectCallback, LuaFunction onReceiveMessageCallback)
        {
            IPAddress mIp = IPAddress.Parse(ip);
            IPEndPoint ip_end_point = new IPEndPoint(mIp, port);
            _onReceiveMessageCallback = onReceiveMessageCallback;
            _onDisconnectCallback = onDisconnectCallback;
            try
            {
                _clientSocket.Connect(ip_end_point);
                IsConnected = true;
                Debug.Log("连接服务器成功");

                if (successCallback!= null) {
                    successCallback.Call();
                }

                //开启线程接收数据
                new Thread(onReceiveSocket).Start();

            }
            catch(Exception e)
            {
                IsConnected = false;
                Debug.Log("连接服务器失败:"+e.Message);
                if (successCallback != null)
                {
                    failCallback.Call(e.Message);
                }
                return;
            }
        }

        private byte[] sendData;
        /// <summary>  
        /// 发送数据给服务器  
        /// </summary>  
        public void SendCommand(int commandId, ByteBuffer data = null)
        {
            
            if (IsConnected == false)
                return;
            try
            {
                ByteBuffer buffer = new ByteBuffer();
                buffer.WriteInt(commandId);
                if (data == null)
                {
                    //buffer.WriteBuffer(null);
                }
                else {

                    buffer.WriteBytes(data.ToBytes());
                }
                _clientSocket.Send(buffer.ToBytes());
            }
            catch
            {
                IsConnected = false;
                _clientSocket.Shutdown(SocketShutdown.Both);
                _clientSocket.Close();
            }
        }

        private void onReceiveSocket() {

            while (true) {

                if (!_clientSocket.Connected) {
                    break;
                }

                try {

                    int receiveLength = _clientSocket.Receive(_result);

                    if (receiveLength > 0) {

                        ByteBuffer buffer = new ByteBuffer(_result);
                        int command = buffer.ReadInt();
                        byte[] data = buffer.ReadBytes();
                        DebugManager.Log("服务器指令：" + command);
                        
                        OnReceiveMessage(command, data);
                    }


                } catch (Exception e) {
                    DebugManager.Log(e.Message);
                }

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="bytes"></param>
        public void OnReceiveMessage(int commandId, byte[] bytes)
        {
            try
            {
                if (bytes == null)
                {
                    _onReceiveMessageCallback.Call(commandId, 0);
                }
                else
                {
                    ByteBuffer data = new ByteBuffer(bytes);

                    if (_onReceiveMessageCallback != null)
                        _onReceiveMessageCallback.Call(commandId, data.ReadBuffer());

                    data.Close();
                }
            }
            catch (Exception e)
            {
                DebugManager.LogError("Scoket OnReceiveMessage Error:" + commandId.ToString() + "," + e.Message);
            }
        }

        /// <summary>
        /// 注册PB
        /// </summary>
        /// <param name="path"></param>
        /// <param name="processCallback"></param>
        /// <param name="completeCallback"></param>
        public void RegisterPB(LuaFunction processCallback, LuaFunction completeCallback)
        {
            this.StartCoroutine(this.DoRegisterPB(processCallback, completeCallback));
        }

        IEnumerator DoRegisterPB(LuaFunction processCallback, LuaFunction completeCallback)
        {
            string configFilePath = "";
            string pbDir = "";

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                pbDir = FileUtil.Instance.GetWritePath("protobuf/");
            }
            else
            {
                pbDir = FileUtil.Instance.GetAssetsPath("GameApp/Protobuf/").Replace("file://", "");
            }

            configFilePath = pbDir + "proto.config";

            Debugger.Log(configFilePath);

            string configFileStr = "";

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                WWW www = new WWW(configFilePath);

                yield return www;

                if (www.isDone)
                {
                    configFileStr = www.text;

                    yield return 0;
                }
                else
                {
                    DebugManager.LogError("DoRegisterPB Error:读取文件错误！" + configFilePath);
                }

                www.Dispose();
            }
            else
            {
                if (File.Exists(configFilePath) == true)
                {
                    try
                    {
                        configFileStr = File.ReadAllText(configFilePath);
                    }
                    catch (Exception e)
                    {
                        DebugManager.LogError("DoRegisterPB Error:读取配置文件错误！" + e.Message);

                        yield break;
                    }

                }
                else
                {
                    DebugManager.LogError("DoRegisterPB Error:读取文件错误！" + configFilePath);
                }
            }

            DebugManager.Log("configFileStr:" + configFileStr);

            string[] pClassCommandList = configFileStr.Split(',');
            
            string fpath = pbDir+"message.pb";
            DebugManager.Log("读取注册PB文件:" + fpath);

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                WWW www = new WWW(fpath);

                yield return www;

                if (www.isDone)
                {
                    ByteBuffer data = new ByteBuffer(www.bytes);
                        
                    processCallback.Call(data);
                    data.Close();

                    yield return 0;
                }
                else
                {
                    DebugManager.LogError("DoRegisterPB Error:读取文件错误！" + fpath);
                }

                www.Dispose();
            }
            else
            {
                try
                {
                    ByteBuffer data = new ByteBuffer(File.ReadAllBytes(fpath));
                    processCallback.Call(data);
                    data.Close();
                }
                catch (Exception e)
                {
                    DebugManager.LogError("DoRegisterPB Error:读取文件错误！" + fpath + "\n error:\n" + e.Message);
                }
            }

            completeCallback.Call(pClassCommandList);
            completeCallback.Dispose();

            processCallback.Dispose();
        }

        /// <summary>
        /// 手动断开连接
        /// </summary>
        /// <param name="state"></param>
        public void OnDisconnect()
        {
            _clientSocket.Disconnect(false);
            try
            {
                if (_onDisconnectCallback != null)
                    _onDisconnectCallback.Call();
                
            }
            catch (Exception e)
            {
                DebugManager.LogError("Scoket OnDisconnect Error:" + e.Message);
            }
        }

        /// <summary>  
        /// 数据转换，网络发送需要两部分数据，一是数据长度，二是主体数据  
        /// </summary>  
        /// <param name="message"></param>  
        /// <returns></returns>  
        private static byte[] WriteMessage(byte[] message)
        {
            MemoryStream ms = null;
            using (ms = new MemoryStream())
            {
                ms.Position = 0;
                BinaryWriter writer = new BinaryWriter(ms);
                ushort msglen = (ushort)message.Length;
                writer.Write(msglen);
                writer.Write(message);
                writer.Flush();
                return ms.ToArray();
            }
        }

        internal void Dispose()
        {
        }
    }
}
