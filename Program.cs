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
        //[STAThread]
        static void Main(string[] args)
        {
            var engine = new ActionLibrary(@"C:\Users\Luke\Documents\OsrsColorBot Resources");

            #region Power Mining Iron Ore

            var rocks = engine.AllImages.Where(x => x.ImageName.Contains("iron rocks")).ToList();
            var ironOre = engine.AllImages.Where(x => x.ImageName == "iron ore.bmp").ToList();

            for (int j = 0; j < 20; j++)
            {
                engine.DropAllItems(ironOre);

                for (int i = 0; i < 20; i++)
                {
                    IoSimulator.PauseThread(2300);
                    engine.ClickOnGameField(rocks);
                }

                IoSimulator.PauseThread(2000);
            }

            #endregion

            #region Gem Stone Mining and Crafting
            //// tool
            //var chisel = engine.AllImages.Where(x => x.ImageName == "chisel.bmp").FirstOrDefault();

            //// resources
            //var uncuts = engine.AllImages.Where(x => x.ImageName.Contains("uncut")).ToList();
            //var cuts = engine.AllImages.Where(x => x.ImageName == "cut jade.bmp" || 
            //                                        x.ImageName == "cut opal.bmp" || 
            //                                        x.ImageName == "cut topaz.bmp").ToList();
            //var itemsToDrop = engine.AllImages.Where(x => x.ImageName == "crushed gem.bmp").ToList();

            //var drop = engine.AllImages.Where(x => x.ImageName == "drop.bmp").FirstOrDefault();

            //engine.UseToolOnResources(uncuts, chisel);

            //IoSimulator.PauseThread(1000);

            //engine.UseToolOnResources(cuts, chisel, 3000);

            //engine.DropAllItems(itemsToDrop, drop);
            #endregion
            
        }
    }
}
