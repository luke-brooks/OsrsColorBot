using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace OsrsColorBot
{
    public class ActionEngine
    {
        public List<OsrsImage> AllImages { get; private set; }
        public UiImageData MenuControls { get; private set; } 

        private ImageProcessingService _imageProcessor = new ImageProcessingService();

        public ActionEngine(string resourceDirectory)
        {
            AllImages = _imageProcessor.LoadBitmapResources(resourceDirectory);
            MenuControls = LocateMenuControls();
        }

        public void UseToolOnResources(List<OsrsImage> resources, OsrsImage tool, int pauseTime = 2000)
        {
            resources.Add(tool);

            #region Determine Scan Boundaries

            var scanBounds = new ScanBoundaries();

            var invLoc = MenuControls.Inventory.MatchLocations.FirstOrDefault();

            scanBounds.MinX = invLoc.X;
            scanBounds.MinY = invLoc.Y;
            scanBounds.MaxX = invLoc.X + MenuControls.Inventory.ImageData.ImageBitmap.Width;
            scanBounds.MaxY = invLoc.Y + MenuControls.Inventory.ImageData.ImageBitmap.Height;

            #endregion

            IoSimulator.ClickLocation(MenuControls.PackContents.MatchLocations.FirstOrDefault());
            var resourceScanData = _imageProcessor.SearchScreenForImages(resources, scanBounds);

            var toolData = resourceScanData.Where(x => x.ImageData.ImageName == tool.ImageName).FirstOrDefault();

            if (toolData != null)
            {
                var toolLocation = toolData.MatchLocations.FirstOrDefault();

                foreach (var r in resourceScanData.Where(x => x.ImageData.ImageName != tool.ImageName))
                {
                    IoSimulator.ClickLocation(toolLocation);

                    IoSimulator.PauseThread(2000);

                    IoSimulator.ClickLocation(r.MatchLocations.FirstOrDefault());

                    IoSimulator.PauseThread(2000);

                    IoSimulator.ClickLocation(MenuControls.PerformActionOnAll);

                    IoSimulator.PauseThread(pauseTime * r.MatchLocations.Count);
                }
            }
            else
            {
                // tool not in inventory
            }
        }
        
        public void DropAllItems(List<OsrsImage> itemIcons, OsrsImage dropIcon)
        {
            IoSimulator.ClickLocation(MenuControls.PackContents.MatchLocations.FirstOrDefault());

            var itemIconScanData = _imageProcessor.SearchScreenForImages(itemIcons);

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

                var dropScanData = _imageProcessor.SearchScreenForImage(dropIcon, menuScanSection, getSingleOccurrence: true);

                if (dropScanData.MatchLocations.Any())
                {
                    var dsdLocation = dropScanData.MatchLocations.FirstOrDefault();

                    var paddedDropLocation = AddPoints(dsdLocation, dropIcon.CenterOfImage);

                    IoSimulator.ClickLocation(paddedDropLocation);
                }

                IoSimulator.PauseThread(750);
            }
        }

        private UiImageData LocateMenuControls()
        {
            var result = new UiImageData();

            var menuControls = AllImages.Where(x => x.ImageName == "view pack contents.bmp" 
                                                    || x.ImageName == "top left of chat-action window.bmp"
                                                    || x.ImageName == "combat menu.bmp"
                                                    ).ToList();

            var menuControlsScanData = _imageProcessor.SearchScreenForImages(menuControls).ToList();

            foreach (var mcsd in menuControlsScanData)
            {
                switch (mcsd.ImageData.ImageName)
                {
                    case "view pack contents.bmp":
                        result.PackContents = mcsd;
                        break;

                    case "top left of chat-action window.bmp":
                        var chatWindow = AllImages.Where(x => x.ImageName == "full chat-action window.bmp").FirstOrDefault();
                        result.PerformActionOnAll = AddPoints(mcsd.MatchLocations.FirstOrDefault(), chatWindow.CenterOfImage);
                        break;

                    case "combat menu.bmp":
                        var inventoryWindow = AllImages.Where(x => x.ImageName == "inventory list.bmp").FirstOrDefault();
                        mcsd.ImageData = inventoryWindow;
                        result.Inventory = mcsd;
                        break;

                    default:
                        break;
                }
            }

            return result;
        }

        private Point AddPoints(Point pointA, Point pointB)
        {
            var result = new Point(pointA.X + pointB.X, pointA.Y + pointB.Y);

            return result;
        }
    }
}
