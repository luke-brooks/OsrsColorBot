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

            engine.DropAllItems(ironOre);

            for (int i = 0; i < 10; i++)
            {
                IoSimulator.PauseThread(2300);
                engine.ClickOnGameField(rocks); 
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

            //#region Power Mining items
            //var ironOre = imgs.Where(x => x.ImageName == "iron ore.bmp").FirstOrDefault();
            //var drop = imgs.Where(x => x.ImageName == "drop.bmp").FirstOrDefault();
            //var ironRocks1 = imgs.Where(x => x.ImageName == "iron rocks 1.bmp").FirstOrDefault();
            //var ironRocks2 = imgs.Where(x => x.ImageName == "iron rocks 2.bmp").FirstOrDefault();
            //var packContents = imgs.Where(x => x.ImageName == "view pack contents.bmp").FirstOrDefault();
            //#endregion

            //var gemRocks = imgs.Where(x => x.ImageName.Contains("gem rocks")).ToList();

            //var gemRocksColors = gemRocks.SelectMany(x => imageProcessor.SaveColorData(x)).ToList();

            //var packContentsLocation = imageProcessor.SearchScreenForImage(packContents, getSingleOccurrence: true).MatchLocations.FirstOrDefault();
            //#region Color detect attempt
            ////var blah = imageProcessor.SearchScreenForColors(gemRocksColors, gemRocks.FirstOrDefault());

            ////if (blah.MatchLocations.Any())
            ////{
            ////    int myNum = blah.MatchLocations.Count % 2;
            ////    IoSimulator.ClickLocation(blah.MatchLocations.ToArray()[myNum], false);
            ////    //IoSimulator.PauseThread(1300);
            ////    //IoSimulator.ClickLocation(blah.MatchLocations.First());

            ////}
            //#endregion


            //DropAllItems(imageProcessor, itemsToDrop, drop, packContentsLocation);

            // DropAllOfItemFromInventory(imageProcessor, opal, drop, packContentsLocation);


        }
    }
}
