using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArcSoft_Face_Demo_X64
{
    class Serial_Device_API
    {
        public struct DeviceSerialPort
        {
            public static SerialPort GateSerialPort;


        }

        public class GateAPI
        {
            public static byte[] gateleft = new byte[5] { 0xeb,0x01,0x40,0x00,0xaa };           // 左向命令
            public static byte[] gateright = new byte[5] { 0xeb, 0x01, 0x41, 0x00, 0xab };      // 右向命令
            public static byte[] receivedate;

            public static void GatePortInit(SerialPort serialport, string portname, int baudrate, int databit, Parity parity, StopBits stopbit, int wirtetimeout, int readtimeout)
            {
                serialport.PortName = portname;
                serialport.BaudRate = baudrate;
                serialport.DataBits = databit;
                serialport.Parity = parity;
                serialport.StopBits = stopbit;
                serialport.WriteTimeout = wirtetimeout;
                serialport.ReadTimeout = readtimeout;
            }


            public static bool GatePortOpen(SerialPort serialport)
            {
                bool portopenflag = true;
                try
                {
                    if (serialport.IsOpen)
                    {
                        serialport.Close();
                    }
                    serialport.Open();
                    if (!serialport.IsOpen)
                    {
                        portopenflag = false;
                    }
                }
                catch
                {
                    portopenflag = false;
                }
                return portopenflag;
            }

            public static bool GatePortClose(SerialPort serialport)
            {
                bool portcloseflag = true;
                try
                {
                    if (serialport.IsOpen)
                    {
                        serialport.Close();
                    }
                    if (serialport.IsOpen)
                    {
                        portcloseflag = false;
                    }
                }
                catch
                {
                    portcloseflag = false;
                }
                return portcloseflag;
            }


            public static bool GateSendData(SerialPort serialport, byte[] sendbuf)
            {
                bool GateSendDataflag =false;
                try
                {
                    if (serialport.IsOpen)
                    {
                        serialport.Write(sendbuf, 0, sendbuf.Length);
                        GateSendDataflag = true;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return GateSendDataflag;
            }


            //调用方法： DeviceSerialPort.GateSerialPort.DataReceived += new SerialDataReceivedEventHandler(GateReceivedData);
          public static void GateReceivedData(object sender, SerialDataReceivedEventArgs e)
            {
                try
                {
                    //接收数据
                    do
                    {
                        int count = DeviceSerialPort.GateSerialPort.BytesToRead;
                        if (count <= 0)
                            break;
                        receivedate = new byte[count];
                        DeviceSerialPort.GateSerialPort.Read(receivedate, 0, count);
                        string str = "";
                        for (int i = 0; i < receivedate.Length; i++)
                        {
                            str += string.Format("{0:X2} ", receivedate[i]);
                        }
                        Console.WriteLine(str);

                    } while (DeviceSerialPort.GateSerialPort.BytesToRead > 0);
                }
                catch (Exception ex)
                {
                   Console.WriteLine("error:接收返回消息异常！具体原因：" + ex.Message);
                }
            }

            



        }
    }
}
