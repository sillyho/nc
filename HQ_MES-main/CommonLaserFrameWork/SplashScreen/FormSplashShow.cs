using System.Threading;
using System.Windows.Forms;

namespace CommonLaserFrameWork.SplashScreen
{
    public partial class FormSplashShow : Form
    {
        public FormSplashShow()
        {
            InitializeComponent();
        }

        private void FormSplashShow_FormClosing(object sender, FormClosingEventArgs e)
        {
            //关闭时渐变透明
            for (int i = 100; i > 1; i--)
            {
                this.Opacity = i / 100.0;
                Thread.Sleep(10);
            }
        }
    }
}
