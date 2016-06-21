

namespace Gravity
{
    using System;
    using System.Drawing;
    using System.Collections;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    //using System.Linq;
    using System.Collections.Generic;
    using System.Text;

    //public class test
    //{
    //    static void Main()
    //    {
    //        PowerNotifications pn = new PowerNotifications();
    //        pn.Start();
    //        Console.WriteLine("starting to sleep");
    //        Thread.Sleep(20000);
    //        pn.Stop();
    //        Console.WriteLine("finally done");
    //    }
    //}

    public class PowerNotifications
    {

        public delegate void PowerStatusChangeEventHandler(object sender, System.EventArgs e, string strPowerMessage);


        public event PowerStatusChangeEventHandler PowerStatusChanged;

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

            Console.WriteLine("starting loop");
            try
            {
                while (!done)
                {
                    res = WaitForSingleObject(ptr, 1000);
                    if (res == 0)
                    {
                        ReadMsgQueue(ptr, buf, (uint)buf.Length, ref nRead, -1, ref flags);
                        //Console.WriteLine("message: " + ConvertByteArray(buf, 0) + " flag: " + ConvertByteArray(buf, 4));
                        uint flag = ConvertByteArray(buf, 4);
                        string msg = null;
                        switch (flag)
                          {
                        //    case 65536:
                        //        msg = "Power On";
                        //        break;
                            case 0x00020000:
                                msg = "Power Off";
                                System.EventArgs p = new System.EventArgs();
                                PowerStatusChanged(this, p, msg);
                                break;                             
                            //case 262144:
                            //    msg = "Power Critical";
                            //    break;
                            //case 524288:
                            //    msg = "Power Boot";
                            //    break;
                            //case 1048576:
                            //    msg = "Power Idle";
                            //    break;
                            //case 2097152:
                            //    msg = "Power Suspend";
                            //    break;
                            //case 8388608:
                            //    msg = "Power Reset";
                            //    break;
                            case 0:
                                // non power transition messages are ignored
                                break;
                            default:
                                msg = "Unknown Flag: " + flag;
                                break;
                        }
                        if (msg != null)
                            Console.WriteLine(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!done)
                {
                    Console.WriteLine("Got exception: " + ex.ToString());
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
