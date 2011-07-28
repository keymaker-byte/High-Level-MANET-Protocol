using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace SystemInteropCompact
{
    /// <summary>
    /// Clase que maneja las notificaciones del sistema operativo para cuando la máquina pasa a estados alterados de energía
    /// Usa librerías nativas del sistema operativo
    /// </summary>
    public class PowerNotifications
    {
        IntPtr ptr = IntPtr.Zero;
        Thread t = null;
        bool done = false;

        [DllImport("coredll.dll")]
        private static extern IntPtr RequestPowerNotifications(IntPtr hMsgQ, uint Flags);

        [DllImport("coredll.dll")]
        private static extern uint WaitForSingleObject(IntPtr hHandle, int wait);

        [DllImport("coredll.dll")]
        private static extern IntPtr CreateMsgQueue(string name, ref MsgQOptions options);

        [DllImport("coredll.dll")]
        private static extern bool ReadMsgQueue(IntPtr hMsgQ, byte[] lpBuffer, uint cbBufSize, ref uint lpNumRead, int dwTimeout, ref uint pdwFlags);

        public PowerNotifications()
        {
            MsgQOptions options = new MsgQOptions();
            options.dwFlags = 0;
            options.dwMaxMessages = 20;
            options.cbMaxMessage = 10000;
            options.bReadAccess = true;
            options.dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(options);
            ptr = CreateMsgQueue("Test", ref options);
            RequestPowerNotifications(ptr, 0xFFFFFFFF);
            t = new Thread(new ThreadStart(DoWork));
        }

        public void Start()
        {
            t.Start();
        }

        public void Stop()
        {
            done = true;
            t.Abort();
        }

        private void DoWork()
        {
            byte[] buf = new byte[10000];
            uint nRead = 0, flags = 0, res = 0;

            while (!done)
            {
                res = WaitForSingleObject(ptr, 1000);
                if (res == 0)
                {
                    ReadMsgQueue(ptr, buf, (uint)buf.Length, ref nRead, -1, ref flags);
                    uint flag = ConvertByteArray(buf, 4);
                    string msg = null;
                    switch (flag)
                    {
                        case 65536:
                            msg = "Power On";
                            break;
                        case 131072:
                            msg = "Power Off";
                            break;
                        case 262144:
                            msg = "Power Critical";
                            break;
                        case 524288:
                            msg = "Power Boot";
                            break;
                        case 1048576:
                            msg = "Power Idle";
                            break;
                        case 2097152:
                            msg = "Power Suspend";
                            break;
                        case 8388608:
                            msg = "Power Reset";
                            break;
                        case 0:
                            // non power transition messages are ignored
                            break;
                        default:
                            msg = "Unknown Flag: " + flag;
                            break;
                    }
                    if (msg != null)
                    {
                        //MessageBox.Show(message);
                    }

                }
            }
        }

        uint ConvertByteArray(byte[] array, int offset)
        {
            uint res = 0;
            res += array[offset];
            res += array[offset + 1] * (uint)0x100;
            res += array[offset + 2] * (uint)0x10000;
            res += array[offset + 3] * (uint)0x1000000;
            return res;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MsgQOptions
        {
            public uint dwSize;
            public uint dwFlags;
            public uint dwMaxMessages;
            public uint cbMaxMessage;
            public bool bReadAccess;
        }
    }
}
