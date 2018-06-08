using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ArcSoft_Face_Demo_X32
{
    public class IDR_D6_API
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct stCardInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string Name;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public string Sex;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
            public string Nation;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
            public string Birthday;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 71)]
            public string Address;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 19)]
            public string ID;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
            public string Department;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
            public string StartDate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
            public string EndDate;
        }

        private const String DLL_RFID = "termb.dll";

        [DllImport(DLL_RFID, EntryPoint = "IDR_InitComm", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int IDR_InitComm(int iPort);

        [DllImport(DLL_RFID, EntryPoint = "IDR_CloseComm", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int IDR_CloseComm();

        [DllImport(DLL_RFID, EntryPoint = "IDR_Authenticate", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int IDR_Authenticate();

        [DllImport(DLL_RFID, EntryPoint = "IDR_Read_Content", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int IDR_Read_Content(int iActive);

        [DllImport(DLL_RFID, EntryPoint = "IDR_GetIdCardTxtInfo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int IDR_GetIdCardTxtInfo(ref stCardInfo info, string filename);

        [DllImport(DLL_RFID, EntryPoint = "IDR_SaveCardData2Bmp", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool IDR_SaveCardData2Bmp(ref stCardInfo info, String fileName, int iFace);
    }
}


