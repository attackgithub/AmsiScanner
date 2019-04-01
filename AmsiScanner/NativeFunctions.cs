﻿using System;
using System.Runtime.InteropServices;

namespace AmsiScanner
{
    class NativeFunctions
    {
        public enum AMSI_RESULT
        {   
            AMSI_RESULT_CLEAN = 0,   
            AMSI_RESULT_NOT_DETECTED = 1,   
            AMSI_RESULT_DETECTED = 32768   
        }

        [DllImport("Amsi.dll", EntryPoint = "AmsiInitialize", CallingConvention = CallingConvention.StdCall)]   
        public static extern int AmsiInitialize([MarshalAs(UnmanagedType.LPWStr)]string appName, out IntPtr amsiContext);   

        [DllImport("Amsi.dll", EntryPoint = "AmsiUninitialize", CallingConvention = CallingConvention.StdCall)]   
        public static extern void AmsiUninitialize(IntPtr amsiContext);   

        [DllImport("Amsi.dll", EntryPoint = "AmsiOpenSession", CallingConvention = CallingConvention.StdCall)]   
        public static extern int AmsiOpenSession(IntPtr amsiContext, out IntPtr session);   

        [DllImport("Amsi.dll", EntryPoint = "AmsiCloseSession", CallingConvention = CallingConvention.StdCall)]   
        public static extern void AmsiCloseSession(IntPtr amsiContext, IntPtr session);   

        [DllImport("Amsi.dll", EntryPoint = "AmsiScanBuffer", CallingConvention = CallingConvention.StdCall)]   
        public static extern int AmsiScanBuffer(IntPtr amsiContext, string buffer, uint length, string contentName, IntPtr session, out AMSI_RESULT result);   

        [DllImport("Amsi.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]   
        public static extern bool AmsiResultIsMalware(AMSI_RESULT result);
    }
}