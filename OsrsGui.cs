using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsrsColorBot
{
    public partial class OsrsGui : Form
    {
        private OsrsBotEngine _engine;

        public OsrsGui(OsrsBotEngine engine)
        {
            _engine = engine;

            InitializeComponent();
        }

        private void btnPowerMine_Click(object sender, EventArgs e)
        {
            var its = Decimal.ToInt32(numIterations.Value);

            var start = DateTime.Now;

            _engine.PowerMine(its);

            var end = DateTime.Now;

            var message = String.Format("All Done!\nStart Time: {0}\nEnd Time: {1}", start, end);

            MessageBox.Show(message);
        }
    }
}
