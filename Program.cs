using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace OsrsColorBot
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var engine = new OsrsBotEngine();

            Application.Run(new OsrsGui(engine));
        }
    }
}
