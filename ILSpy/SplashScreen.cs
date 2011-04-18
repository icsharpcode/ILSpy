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

namespace ICSharpCode.ILSpy
{
    public partial class SplashScreen : Form
    {
        static SynchronizationContext context = null;

        #region static methods
        internal static void Start()
        {
            //new thread for splash screen in 'windows forms'
            new System.Threading.Thread(splashThread).Start();
        }

        internal static void Stop()
        {
            try
            {
                if (context != null)
                {
                    context.Post(e =>
                        {
                            if (System.Windows.Forms.Application.OpenForms.Count > 0)
                            {
                                System.Windows.Forms.Application.OpenForms[0].Close();
                            }
                        }, null);
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
            using (var splashFrm = new SplashScreen())
            {
                context = SynchronizationContext.Current;
                splashFrm.Show();
                try
                {
                    Application.Run(splashFrm);
                }
                finally
                {
                    context = null;
                    //System.Diagnostics.Debug.WriteLine("end of thread");
                }
            }
        }

        #endregion

        public SplashScreen()
        {
            InitializeComponent();
        }

        int index = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            Image current = null;
            index = (index+1) % 6;
            switch(index)
            {
                case 1 :
                    current = Properties.Resources.loading_12;
                    break;
                case 2 :
                    current = Properties.Resources.loading_13;
                    break;
                case 3 :
                    current = Properties.Resources.loading_14;
                    break;
                case 4:
                    current = Properties.Resources.loading_13;
                    break;
                case 5:
                    current = Properties.Resources.loading_12;
                    break;
                default:
                    current = Properties.Resources.loading_11;
                    break;
            }
            this.pictureBox1.Image = current;
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {
            this.pictureBox1.Image = Properties.Resources.loading_11;
            timer1.Enabled = true;
        }

    }
}
