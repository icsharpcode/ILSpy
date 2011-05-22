using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;

namespace ICSharpCode.ILSpy
{
    class SplashScreen
    {
        static Dispatcher context = null;
        static bool closeRequest = false;
        #region static methods
        internal static void Start()
        {
            var th = new System.Threading.Thread(splashThread);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        internal static void Stop()
        {
            try
            {
                closeRequest = true;
                if (context != null)
                {
                    context.BeginInvokeShutdown(DispatcherPriority.ApplicationIdle);
                }
            }
            catch
            {
                //no exception on the caller thread
                context = null;
            }
        }


        [STAThread]
        static void splashThread(object state)
        {
            context = Dispatcher.CurrentDispatcher;
            Thread.Sleep(500);
            if (closeRequest)
                return;
            var splashFrm = new MainWindowSplashScreen();
            try
            {
                splashFrm.Show();
                try
                {
                    Dispatcher.Run();
                }
                finally
                {
                    context = null;
                }
            }
            finally
            {
                splashFrm.Close();
            }
        }

        #endregion
    }
}
