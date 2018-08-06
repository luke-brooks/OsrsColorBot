using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace OsrsColorBot
{
    public class ActionLibrary
    {
        public List<OsrsImage> AllImages { get; private set; }
        public UiImageData MenuControls { get; private set; } 

        private ImageProcessingService _imageProcessor = new ImageProcessingService();

        public ActionLibrary(string resourceDirectory)
        {
            AllImages = _imageProcessor.LoadBitmapResources(resourceDirectory);
            MenuControls = LocateMenuControls();
        }

        public void ClickOnGameField(List<OsrsImage> gameObjects)
        {
            #region Determine Scan Boundaries

            var gameFieldCenterLocation = AddPoints(MenuControls.GameFieldView.MatchLocations.FirstOrDefault(), MenuControls.GameFieldView.ImageData.CenterOfImage);
            var gameFieldScanSection = new ScanBoundaries();
            var xBuffer = 200;
            var yBuffer = 100;

            gameFieldScanSection.MinX = gameFieldCenterLocation.X - xBuffer > 0 ? gameFieldCenterLocation.X - xBuffer : 0;
            gameFieldScanSection.MinY = gameFieldCenterLocation.Y - yBuffer > 0 ? gameFieldCenterLocation.Y - yBuffer : 0;
            gameFieldScanSection.MaxX = gameFieldCenterLocation.X + xBuffer;
            gameFieldScanSection.MaxY = gameFieldCenterLocation.Y + yBuffer;

            #endregion

            var gameObjectsScanData = _imageProcessor.SearchScreenForImages(gameObjects, gameFieldScanSection, getSingleOccurrence: true);

            if (gameObjectsScanData.Any())
            {
                var gObj = gameObjectsScanData.FirstOrDefault();
                var matchedLocation = gObj.MatchLocations.FirstOrDefault();

                var clickLocation = AddPoints(matchedLocation, gObj.ImageData.CenterOfImage);

                IoSimulator.ClickLocation(clickLocation);
                IoSimulator.PauseThread(600);
                IoSimulator.ClickLocation(clickLocation);
            }
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
        
        public void DropAllItems(List<OsrsImage> itemIcons)
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

                #region Determine Scan Boundaries
                var menuScanSection = new ScanBoundaries();

                var scanBuffer = 150;

                menuScanSection.MinX = paddedItemIconLocation.X - scanBuffer;
                menuScanSection.MinY = paddedItemIconLocation.Y - scanBuffer;
                menuScanSection.MaxX = paddedItemIconLocation.X + scanBuffer;
                menuScanSection.MaxY = paddedItemIconLocation.Y + scanBuffer;
                #endregion

                var dropScanData = _imageProcessor.SearchScreenForImages(MenuControls.RightClickMenu.Drop.ToList(), menuScanSection, getSingleOccurrence: true);

                if (dropScanData.Any())
                {
                    var dsdLocation = dropScanData.FirstOrDefault().MatchLocations.FirstOrDefault();

                    var paddedDropLocation = AddPoints(dsdLocation, MenuControls.RightClickMenu.Drop.CenterOfImage);

                    IoSimulator.ClickLocation(paddedDropLocation);
                }

                IoSimulator.PauseThread(750);
            }
        }

        private UiImageData LocateMenuControls()
        {
            var result = new UiImageData();

            result.RightClickMenu.Drop = AllImages.Where(x => x.ImageName == "drop.bmp").FirstOrDefault();

            var menuControls = AllImages.Where(x => x.ImageName == "view pack contents.bmp" 
                                                    || x.ImageName == "top left of chat-action window.bmp"
                                                    || x.ImageName == "combat menu.bmp"
                                                    || x.ImageName == "bottom left of full ui view.bmp"
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

                    case "bottom left of full ui view.bmp":
                        var fullUiWindow = AllImages.Where(x => x.ImageName == "full ui view.bmp").FirstOrDefault();
                        var gameField = AllImages.Where(x => x.ImageName == "gamefield view.bmp").FirstOrDefault();

                        var gameFieldData = new OsrsScanData { ImageData = gameField };
                        var gameFieldLocation = 
                            new Point(mcsd.MatchLocations.FirstOrDefault().X, 
                                mcsd.MatchLocations.FirstOrDefault().Y - fullUiWindow.ImageBitmap.Height > 0 ? 
                                    mcsd.MatchLocations.FirstOrDefault().Y - fullUiWindow.ImageBitmap.Height : 0);

                        gameFieldData.MatchLocations.Add(gameFieldLocation);

                        result.GameFieldView = gameFieldData;
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
