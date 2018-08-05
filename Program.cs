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
            var imageProcessor = new ImageProcessingService();
            var imgs = imageProcessor.LoadBitmapResources(@"C:\Users\Luke\Documents\OsrsColorBot Resources");

            #region Power Mining items
            var ironOre = imgs.Where(x => x.ImageName == "iron ore.bmp").FirstOrDefault();
            var drop = imgs.Where(x => x.ImageName == "drop.bmp").FirstOrDefault();
            var ironRocks1 = imgs.Where(x => x.ImageName == "iron rocks 1.bmp").FirstOrDefault();
            var ironRocks2 = imgs.Where(x => x.ImageName == "iron rocks 2.bmp").FirstOrDefault();
            var packContents = imgs.Where(x => x.ImageName == "view pack contents.bmp").FirstOrDefault();
            var opal = imgs.Where(x => x.ImageName.Contains("opal")).FirstOrDefault();
            #endregion

            var gemRocks = imgs.Where(x => x.ImageName.Contains("gem rocks")).ToList();

            var itemsToDrop = imgs.Where(x => x.ImageName == "crushed gem.bmp" || x.ImageName == "cut opal.bmp").ToList();
            var chisel = imgs.Where(x => x.ImageName == "chisel.bmp").FirstOrDefault();
            var uncuts = imgs.Where(x => x.ImageName.Contains("uncut")).ToList();
            var cutAllJade = imgs.Where(x => x.ImageName == "cut all jade.bmp").FirstOrDefault();

            var uncutJade = uncuts.Where(x => x.ImageName.Contains("jade")).FirstOrDefault();

            var gemRocksColors = gemRocks.SelectMany(x => imageProcessor.SaveColorData(x)).ToList();

            var packContentsLocation = imageProcessor.SearchScreenForImage(packContents, getSingleOccurrence: true).MatchLocations.FirstOrDefault();
            #region Color detect attempt
            //var blah = imageProcessor.SearchScreenForColors(gemRocksColors, gemRocks.FirstOrDefault());

            //if (blah.MatchLocations.Any())
            //{
            //    int myNum = blah.MatchLocations.Count % 2;
            //    IoSimulator.ClickLocation(blah.MatchLocations.ToArray()[myNum], false);
            //    //IoSimulator.PauseThread(1300);
            //    //IoSimulator.ClickLocation(blah.MatchLocations.First());

            //}
            #endregion

            var chiselLocation = imageProcessor.SearchScreenForImage(chisel, getSingleOccurrence: true).MatchLocations.FirstOrDefault();
            var uncutLocation = imageProcessor.SearchScreenForImage(uncutJade, getSingleOccurrence: true).MatchLocations.FirstOrDefault();

            IoSimulator.ClickLocation(chiselLocation);

            IoSimulator.PauseThread(2000);

            IoSimulator.ClickLocation(uncutLocation);

            IoSimulator.PauseThread(1500);

            var cutAllJadeLocation = imageProcessor.SearchScreenForImage(cutAllJade, getSingleOccurrence: true).MatchLocations.FirstOrDefault();


            IoSimulator.ClickLocation(cutAllJadeLocation);

            IoSimulator.PauseThread(2000);
            //DropAllItems(imageProcessor, itemsToDrop, drop, packContentsLocation);

            // DropAllOfItemFromInventory(imageProcessor, opal, drop, packContentsLocation);


        }

        private static void MineRocks(ImageProcessingService imageProcessor, OsrsImage rocks)
        {
            var rocksScanData = imageProcessor.SearchScreenForImage(rocks, getSingleOccurrence: true);

            if (rocksScanData.MatchLocations.Any())
            {
                var matchedLocation = rocksScanData.MatchLocations.FirstOrDefault();
                var paddedRockLocation = AddPoints(matchedLocation, rocks.CenterOfImage);

                var farthestReach = new Point(matchedLocation.X + (rocks.ImageBitmap.Width), matchedLocation.Y + rocks.ImageBitmap.Height);

                IoSimulator.ClickLocation(matchedLocation);
                IoSimulator.PauseThread(1300);
                IoSimulator.ClickLocation(matchedLocation);
            }
        }

        private static void DropAllOfItemFromInventory(ImageProcessingService imageProcessor, OsrsImage itemIcon, OsrsImage dropIcon, Point packContentsLocation)
        {
            IoSimulator.ClickLocation(packContentsLocation);

            var itemIconScanData = imageProcessor.SearchScreenForImage(itemIcon);

            foreach (var itemIconLocation in itemIconScanData.MatchLocations)
            {
                var paddedItemIconLocation = AddPoints(itemIconLocation, itemIcon.CenterOfImage);

                IoSimulator.ClickLocation(paddedItemIconLocation, leftClick: false);

                IoSimulator.PauseThread(50);

                var menuScanSection = new ScanBoundaries();

                var scanBuffer = 100;

                menuScanSection.MinX = paddedItemIconLocation.X - scanBuffer;
                menuScanSection.MinY = paddedItemIconLocation.Y - scanBuffer;
                menuScanSection.MaxX = paddedItemIconLocation.X + scanBuffer;
                menuScanSection.MaxY = paddedItemIconLocation.Y + scanBuffer;

                var dropScanData = imageProcessor.SearchScreenForImage(dropIcon, menuScanSection, getSingleOccurrence: true);

                if (dropScanData.MatchLocations.Any())
                {
                    var dsdLocation = dropScanData.MatchLocations.FirstOrDefault();

                    var paddedDropLocation = AddPoints(dsdLocation, dropIcon.CenterOfImage);

                    IoSimulator.ClickLocation(paddedDropLocation);
                }

                IoSimulator.PauseThread(750);
            }
        }

        private static void DropAllItems(ImageProcessingService imageProcessor, List<OsrsImage> itemIcons, OsrsImage dropIcon, Point packContentsLocation)
        {
            IoSimulator.ClickLocation(packContentsLocation);

            var itemIconScanData = imageProcessor.SearchScreenForImages(itemIcons);

            var allMatchedLocations = itemIconScanData.SelectMany(x => x.MatchLocations).ToList();
            var centerOfImage = itemIconScanData.Select(x => x.ImageData.CenterOfImage).FirstOrDefault();
            
            foreach (var itemIconLocation in allMatchedLocations)
            {
                var paddedItemIconLocation = AddPoints(itemIconLocation, centerOfImage);

                IoSimulator.ClickLocation(paddedItemIconLocation, leftClick: false);

                IoSimulator.PauseThread(100);

                var menuScanSection = new ScanBoundaries();

                var scanBuffer = 150;

                menuScanSection.MinX = paddedItemIconLocation.X - scanBuffer;
                menuScanSection.MinY = paddedItemIconLocation.Y - scanBuffer;
                menuScanSection.MaxX = paddedItemIconLocation.X + scanBuffer;
                menuScanSection.MaxY = paddedItemIconLocation.Y + scanBuffer;

                var dropScanData = imageProcessor.SearchScreenForImage(dropIcon, menuScanSection, getSingleOccurrence: true);

                if (dropScanData.MatchLocations.Any())
                {
                    var dsdLocation = dropScanData.MatchLocations.FirstOrDefault();

                    var paddedDropLocation = AddPoints(dsdLocation, dropIcon.CenterOfImage);

                    IoSimulator.ClickLocation(paddedDropLocation);
                }

                IoSimulator.PauseThread(750);
            } 
        }

        private static Point AddPoints(Point pointA, Point pointB)
        {
            var result = new Point(pointA.X + pointB.X, pointA.Y + pointB.Y);

            return result;
        }

        private static void PowerMine(ImageProcessingService imageProcessor, List<OsrsImage> gemRocks, OsrsImage ironOre, OsrsImage drop, Point packContentsLocation)
        {
            var numOfIterations = 10;
            for (int i = 0; i < numOfIterations * 4; i++)
            {
                for (int j = 0; j < numOfIterations; j++)
                {
                    MineRocks(imageProcessor, gemRocks.First());

                    IoSimulator.PauseThread(3000);

                    MineRocks(imageProcessor, gemRocks.Last());

                    IoSimulator.PauseThread(3000);
                }

                DropAllOfItemFromInventory(imageProcessor, ironOre, drop, packContentsLocation);
            }
        }
    }
}
