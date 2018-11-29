using System;
using System.Collections.Generic;
using System.IO;

namespace AutoCreateCode
{
     class Program
    {
        private static string mainName = "引导活动";
        private static string keyword = "ActivityGuide";
        private static string filePath = "I:\\Desktop\\接口\\test.txt";
        private static string outDir = "I:\\Desktop\\接口\\code\\";
        private static int state = 0;//1.写controller 2.写handler

        private static string end = "\r\n";

        private static string constStr = "";
        private static string messageStr = "";
        private static List<string> controllerStr = new List<string>();
        private static List<string> handlerStr = new List<string>();
        private static List<string> dataStr = new List<string>();

        static void Main(string[] args)
        {

            string[] content = File.ReadAllLines(filePath);
            string note = "";
            int code = 0;
            int code2 = 0;
            string name;
            string upName = "";
            int titleNum = 0;
            int sendNum = 0;
            List<string> itemList = new List<string>();


            constStr += "-- " + mainName + end + end;
            messageStr += "-- " + mainName + end + end;
            messageStr += "Message." + keyword.ToUpper() + " = 9999"+ end;

            controllerStr.Add("local " + keyword + "Controller = class(\"" + keyword + "Controller\", import(\"crazystudio.mvc.BaseController\"))");
            controllerStr.Add(end);
            controllerStr.Add("----------------------");
            controllerStr.Add("--- " + mainName + "Controller --- ");
            controllerStr.Add("----------------------");
            controllerStr.Add(end);

            handlerStr.Add("local " + keyword + "Handler = class(\"" + keyword + "Handler\", import(\"crazystudio.mvc.BaseHandler\"))");
            handlerStr.Add(end);
            handlerStr.Add("----------------------");
            handlerStr.Add("--- " + mainName + "Handler --- ");
            handlerStr.Add("----------------------");
            handlerStr.Add(end);

            dataStr.Add("local " + keyword + "Data = class(\"" + keyword + "Data\", import(\"crazystudio.mvc.BaseData\"))");
            dataStr.Add(end);
            dataStr.Add("----------------------");
            dataStr.Add("--- " + mainName + "Data --- ");
            dataStr.Add("----------------------");
            dataStr.Add(end);

            dataStr.Add("--- <summary>");
            dataStr.Add("--- 初始化");
            dataStr.Add("--- <summary>");
            dataStr.Add("function "+keyword+"Data:onInit()");
            dataStr.Add(end);
            dataStr.Add("end");
            dataStr.Add(end);
            dataStr.Add("return "+ keyword + "Data");
            


            foreach (string line in content) {


                if (line.StartsWith("//"))
                {
                    note = line.Replace("//", "").Trim();

                }
                else if (line.StartsWith("message") && line.Contains("C2M_"))
                {
                    state = 1;

                    string[] temp = line.Split("_");
                    code = int.Parse( temp[2].Substring(0, temp[2].IndexOf("//")).Trim());
                    name = temp[1].Replace("Request", "");
                    upName = getUpNames(name);

                    string tmpConstStr = "Command." + upName + " = " + code;
                    tmpConstStr = tmpConstStr.PadRight(56);
                    tmpConstStr += "-- " + note + end;
                    constStr += tmpConstStr;


                    controllerStr.Add("--- <summary>");
                    controllerStr.Add("--- " + note);
                    controllerStr.Add("--- <summary>");
                    controllerStr.Add("function " + keyword + "Controller:do" + name + "()");
                    titleNum = controllerStr.Count - 1;
                    controllerStr.Add("	api:sendCommand(Command." + upName + ", {})");
                    sendNum = controllerStr.Count - 1;
                    controllerStr.Add("end");

                    itemList = new List<string>();
                }
                else if (line.StartsWith("message") && line.Contains("M2C_"))
                {
                    state = 2;

                    string[] temp = line.Split("_");
                    code2 = int.Parse(temp[2].Replace("//", "").Replace("IActorMessage", "").Trim());
                    name = temp[1].Replace("Response", "");
                    upName = getUpNames(name);

                    string tmpMessageStr = "Message." + upName + " = \"" + upName + "\"";
                    tmpMessageStr = tmpMessageStr.PadRight(92);
                    tmpMessageStr += "-- " + note + end;
                    messageStr += tmpMessageStr;
                    

                    handlerStr.Add("--- <summary>");
                    handlerStr.Add("--- " + note);
                    handlerStr.Add("--- <summary>");
                    handlerStr.Add("function " + keyword + "Handler:HANDLER_" + code2 + "(data)");

                    itemList = new List<string>();
                }
                else if ((line.Contains("optional") || line.Contains("repeated")) && state != 0)
                {
                    string[] temp = line.Split("=")[0].Trim().Split(" ");
                    itemList.Add(temp[2]);
                    //Console.WriteLine(temp[6] + "," + temp.Length + "," + line);
                }
                else if (line.StartsWith("message") && line.Contains("MSG_"))
                {
                    state = 0;
                }
                else if (line.StartsWith("}"))
                {
                    if (state == 1)
                    {

                        string titleStr = "(";
                        string sendStr = "{";
                        foreach (string item in itemList)
                        {
                            if (!titleStr.Equals("("))
                            {
                                titleStr += ", ";
                                sendStr += ", ";
                            }
                            titleStr += item;
                            sendStr += item + " = " + item;
                        }
                        titleStr += ")";
                        sendStr += "}";

                        controllerStr[titleNum] = controllerStr[titleNum].Replace("()", titleStr);
                        controllerStr[sendNum] = controllerStr[sendNum].Replace("{}", sendStr);

                        controllerStr.Add(end);
                    }
                    else if (state == 2)
                    {

                        handlerStr.Add(end);
                        foreach (string item in itemList)
                        {
                            handlerStr.Add("	api." + keyword + "Data." + item + " = data." + item);
                        }


                        handlerStr.Add("	api:sendNotification(Message." + keyword.ToUpper() + ", Message." + upName + ")");
                        handlerStr.Add(end);
                        handlerStr.Add("end");
                        handlerStr.Add(end);
                    }
                }

            }

            handlerStr.Add("return " + keyword + "Handler");
            controllerStr.Add("return " + keyword + "Controller");


            //================================================准备写入文件============================================================



            Console.WriteLine(constStr);
            Console.WriteLine("");
            Console.WriteLine(messageStr);
            Console.WriteLine("");
            Console.WriteLine("    api:registerData(\"" + keyword + "Data\")");
            Console.WriteLine("    api:registerController(\"" + keyword + "Controller\")");
            Console.WriteLine("    api:registerHandler(\"" + keyword + "Handler\")");
            

            string kName = keyword.Substring(0, 1).ToLower() + keyword.Substring(1);
            string controllerPath = outDir + kName + "Controller.lua";
            string handlerPath = outDir + kName + "Handler.lua";
            string dataPath = outDir + kName + "Data.lua";
            
            TextWriter cw = File.CreateText(controllerPath);
            
            foreach (string line in controllerStr)
            {
                if (line.Equals(end))
                {
                    cw.WriteLine();
                }
                else {
                    cw.WriteLine(line);
                }
                
                //Console.WriteLine(line);
            }
            cw.Close();

            TextWriter hw = File.CreateText(handlerPath);
            foreach (string line in handlerStr)
            {
                if (line.Equals(end))
                {
                    hw.WriteLine();
                }
                else
                {
                    hw.WriteLine(line);
                }
                //Console.WriteLine(line);
            }
            hw.Close();

            TextWriter dw = File.CreateText(dataPath);
            foreach (string line in dataStr)
            {
                if (line.Equals(end))
                {
                    dw.WriteLine();
                }
                else
                {
                    dw.WriteLine(line);
                }
                //Console.WriteLine(line);
            }
            dw.Close();

            Console.ReadKey();
        }

        /// <summary>
        /// 把首字母大写单词转为全大写下划线分割单词
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static string getUpNames(string name) {

            string upName = keyword.ToUpper();

            foreach(char c in name) {
                if (c >= 'A' && c <= 'Z' )
                {
                    upName += "_" + c;
                }
                else
                {
                    upName += c.ToString().ToUpper();
                }
            }

            return upName;

        }
    }
}
