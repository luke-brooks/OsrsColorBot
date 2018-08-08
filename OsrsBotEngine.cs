using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Configuration;
using System.Windows.Forms;

namespace OsrsColorBot
{
    public class OsrsBotEngine
    {
        private ActionLibrary _actionLib;

        public OsrsBotEngine()
        {
            _actionLib = new ActionLibrary(@"C:\Users\Luke\Documents\OsrsColorBot Resources");
        }

        public void PowerMine(int numOfIterations = 3)
        {
            var rocks = _actionLib.AllImages.Where(x => x.ImageName.Contains("iron rocks")).ToList();
            var ironOre = _actionLib.AllImages.Where(x => x.ImageName == "iron ore.bmp").ToList();

            for (int j = 0; j < numOfIterations; j++)
            {
                _actionLib.DropAllItems(ironOre);

                for (int i = 0; i < 20; i++)
                {
                    IoSimulator.PauseThread(2300);
                    _actionLib.ClickOnGameField(rocks);
                }

                IoSimulator.PauseThread(2000);
            }
        }

        public void CutGemStones()
        {
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
        }
    }
}
