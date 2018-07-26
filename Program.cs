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

            var ironOre = imgs.Where(x => x.ImageName == "iron ore.bmp").FirstOrDefault();
            var drop = imgs.Where(x => x.ImageName == "drop.bmp").FirstOrDefault();
            var ironRocks1 = imgs.Where(x => x.ImageName == "iron rocks 1.bmp").FirstOrDefault();
            var ironRocks2 = imgs.Where(x => x.ImageName == "iron rocks 2.bmp").FirstOrDefault();
            var packContents = imgs.Where(x => x.ImageName == "view pack contents.bmp").FirstOrDefault();

            var gemRocks = imgs.Where(x => x.ImageName.Contains("gem rocks")).ToList();

            var gemRocksColors = gemRocks.SelectMany(x => imageProcessor.SaveColorData(x)).ToList();

            var packContentsLocation = imageProcessor.SearchScreenForImage(packContents, getSingleOccurrence: true).MatchLocations.FirstOrDefault();

            var blah = imageProcessor.SearchScreenForColors(gemRocksColors, gemRocks.FirstOrDefault());

            var numOfIterations = 10;
            for (int i = 0; i < numOfIterations * 4; i++)
            {
                for (int j = 0; j < numOfIterations; j++)
                {
                    MineRocks(imageProcessor, ironRocks1);

                    IoSimulator.PauseThread(3000);

                    MineRocks(imageProcessor, ironRocks2);

                    IoSimulator.PauseThread(3000);
                }

                DropIronOres(imageProcessor, ironOre, drop, packContentsLocation); 
            }
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

        private static void DropIronOres(ImageProcessingService imageProcessor, OsrsImage ironOre, OsrsImage dropIcon, Point packContentsLocation)
        {
            IoSimulator.ClickLocation(packContentsLocation);

            var ironOreScanData = imageProcessor.SearchScreenForImage(ironOre);

            foreach (var ironOreLocation in ironOreScanData.MatchLocations)
            {
                var paddedIronOreLocation = AddPoints(ironOreLocation, ironOre.CenterOfImage);

                IoSimulator.ClickLocation(paddedIronOreLocation, leftClick: false);

                IoSimulator.PauseThread(50);

                var menuScanSection = new ScanBoundaries();

                var scanBuffer = 100;

                menuScanSection.MinX = paddedIronOreLocation.X - scanBuffer;
                menuScanSection.MinY = paddedIronOreLocation.Y - scanBuffer;
                menuScanSection.MaxX = paddedIronOreLocation.X + scanBuffer;
                menuScanSection.MaxY = paddedIronOreLocation.Y + scanBuffer;

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
    }
}
