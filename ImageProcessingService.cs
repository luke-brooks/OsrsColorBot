﻿using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using WindowScrape.Static;

namespace OsrsColorBot
{
    public class ImageProcessingService
    {
        public static UserConfigOptionsModel UserConfigOptions { get; set; }

        public ImageProcessingService()
        {
        }

        #region Public Methods
        
        public List<OsrsScanData> SearchScreenForImages(List<OsrsImage> osrsImages, ScanBoundaries boundaries = null, bool getSingleOccurrence = false)
        {
            var response = new List<OsrsScanData>();
            var osrsWindow = HwndInterface.GetHwndFromTitle("Old School RuneScape");
            var windowSize = HwndInterface.GetHwndSize(osrsWindow);
            var windowLocation = HwndInterface.GetHwndPos(osrsWindow);

            HwndInterface.ActivateWindow(osrsWindow);

            var screenshot = TakeScreenshot();

            #region Scan Boundaries Calculation
            if (boundaries == null)
            {
                boundaries = new ScanBoundaries();

                var imgMaxX = osrsImages.Max(x => x.ImageBitmap.Width);
                var imgMaxY = osrsImages.Max(x => x.ImageBitmap.Height);

                boundaries.MinX = windowLocation.X < 0 ? 0 : windowLocation.X;
                boundaries.MinY = windowLocation.Y < 0 ? 0 : windowLocation.Y;
                boundaries.MaxX = (windowLocation.X + windowSize.Width) > screenshot.Width ? screenshot.Width - imgMaxX : (windowLocation.X + windowSize.Width) - imgMaxX;
                boundaries.MaxY = (windowLocation.Y + windowSize.Height) > screenshot.Height ? screenshot.Height - imgMaxY : (windowLocation.Y + windowSize.Height) - imgMaxY;
            }
            #endregion

            response = FindAllBitmapsInImage(osrsImages, screenshot, boundaries, getSingleOccurrence);

            return response;
        }

        public OsrsScanData SearchScreenForColors(List<Color> colors, OsrsImage image, ScanBoundaries boundaries = null)
        {
            LoggingUtility.WriteToAuditLog("Starting Color Search");
            OsrsScanData response = null;
            var osrsWindow = HwndInterface.GetHwndFromTitle("Old School RuneScape");
            var windowSize = HwndInterface.GetHwndSize(osrsWindow);
            var windowLocation = HwndInterface.GetHwndPos(osrsWindow);

            HwndInterface.ActivateWindow(osrsWindow);

            var screenshot = TakeScreenshot();

            #region Scan Boundaries Calculation
            if (boundaries == null)
            {
                boundaries = new ScanBoundaries();

                boundaries.MinX = windowLocation.X < 0 ? 0 : windowLocation.X;
                boundaries.MinY = windowLocation.Y < 0 ? 0 : windowLocation.Y;
                boundaries.MaxX = (windowLocation.X + windowSize.Width) > screenshot.Width ? screenshot.Width : (windowLocation.X + windowSize.Width);
                boundaries.MaxY = (windowLocation.Y + windowSize.Height) > screenshot.Height ? screenshot.Height : (windowLocation.Y + windowSize.Height);
            }
            #endregion

            response = FindColorsInImage(colors, screenshot, boundaries);
            //response = FindColorsInImage(colors, image.ImageBitmap, new ScanBoundaries { MinX = 0, MinY = 0, MaxX = image.ImageBitmap.Width, MaxY = image.ImageBitmap.Height });

            LoggingUtility.WriteToAuditLog("Color Search Complete");
            return response;
        }

        public List<Color> SaveColorData(OsrsImage osrsImage)
        {
            var result = new List<Color>();
            var colorList = new List<OsrsColor>();

            // The X and Y on the inner loops represent the coordinates on the bitmap that we are trying to find in the Screenshot
            for (int innerX = 0; innerX < osrsImage.ImageBitmap.Width; innerX++)
            {
                for (int innerY = 0; innerY < osrsImage.ImageBitmap.Height; innerY++)
                {
                    var imgColor = osrsImage.ImageBitmap.GetPixel(innerX, innerY);

                    result.Add(imgColor);

                    var cData = new OsrsColor()
                    {
                        ImageName = osrsImage.ImageName,
                        ColorName = imgColor.Name,
                        A = imgColor.A,
                        R = imgColor.R,
                        G = imgColor.G,
                        B = imgColor.B
                    };

                    if (!colorList.Where(x => x.ImageName == cData.ImageName
                        && x.ColorName == cData.ColorName
                        && x.A == cData.A
                        && x.R == cData.R
                        && x.G == cData.G
                        && x.B == cData.B).Any())
                    {
                        colorList.Add(cData);
                    }
                }
            }

            LoggingUtility.WriteToColorLog(colorList, osrsImage.ImageName);

            return result;
        }

        public List<OsrsImage> LoadBitmapResources(string imgLocation)
        {
            var osrsImgs = new List<OsrsImage>();

            ConfigService.CheckResourceDirectoryExists(imgLocation);

            var directories = new DirectoryInfo(imgLocation).GetDirectories();

            foreach (var d in directories)
            {
                var files = d.GetFiles();

                foreach (var f in files)
                {
                    osrsImgs.Add(new OsrsImage(new Bitmap(f.FullName), f.Name));
                }
            }

            return osrsImgs;
        }

        #endregion

        #region Private Methods

        private Bitmap TakeScreenshot()
        {
            var screenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            var g = Graphics.FromImage(screenshot);

            g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);

            LoggingUtility.SaveScreenshot(screenshot);

            return screenshot;
        }
        
        // this should be able to scan for a list of OsrsImages to find multiple items on one scan
        private List<OsrsScanData> FindAllBitmapsInImage(List<OsrsImage> needles, Bitmap haystack, ScanBoundaries boundaries, bool getSingleOccurrence)
        {
            var result = new List<OsrsScanData>();

            // The X and Y of the outer loops represent the coordinates on the Screenshot object
            for (int outerX = boundaries.MinX; outerX < boundaries.MaxX; outerX++)
            {
                for (int outerY = boundaries.MinY; outerY < boundaries.MaxY; outerY++)
                {
                    foreach (var n in needles)
                    {
                        var temp = new OsrsScanData() { ImageData = n };

                        // The X and Y on the inner loops represent the coordinates on the bitmap that we are trying to find in the Screenshot
                        for (int innerX = 0; innerX < n.ImageBitmap.Width; innerX++)
                        {
                            for (int innerY = 0; innerY < n.ImageBitmap.Height; innerY++)
                            {
                                Color cNeedle = n.ImageBitmap.GetPixel(innerX, innerY);
                                Color cHaystack = haystack.GetPixel(innerX + outerX, innerY + outerY);

                                // We compare the color of the pixel in the Screenshot with the pixel in the bitmap we are searching for
                                if (cNeedle.R != cHaystack.R || cNeedle.G != cHaystack.G || cNeedle.B != cHaystack.B)
                                {
                                    // Stop examining the current bitmap once a single pixel doesn't match
                                    goto notFound;
                                }
                            }
                        }

                        var foundPoint = new Point(outerX, outerY);
                        var existingEntry = result.Where(x => x.ImageData.ImageName == temp.ImageData.ImageName).FirstOrDefault();

                        if (existingEntry != null)
                        {
                            existingEntry.MatchLocations.Add(foundPoint);
                        }
                        else
                        {
                            temp.MatchLocations.Add(foundPoint);
                            result.Add(temp);
                        }

                        if (getSingleOccurrence)
                        {
                            return result;
                        }

                        notFound:
                        continue;
                    }
                }
            }

            return result;
        }

        // tis doesnt fucking work right now
        private OsrsScanData FindColorsInImage(List<Color> colors, Bitmap haystack, ScanBoundaries boundaries)
        {
            LoggingUtility.WriteToAuditLog("Beginning Screenshot Examination for Colors");
            var result = new OsrsScanData();

            // The X and Y of the outer loops represent the coordinates on the Screenshot object
            for (int outerX = boundaries.MinX; outerX < boundaries.MaxX; outerX++)
            {
                for (int outerY = boundaries.MinY; outerY < boundaries.MaxY; outerY++)
                {
                    foreach (var c in colors)
                    {
                        Color cHaystack = haystack.GetPixel(outerX, outerY);

                        if (result.MatchLocations.Count > 20)
                        {
                            return result;
                        }

                        // We compare the color of the pixel in the Screenshot with the Color we are searching for
                        if (c.R == cHaystack.R || c.G == cHaystack.G || c.B == cHaystack.B)
                        {
                            LoggingUtility.WriteToAuditLog(String.Format("Color Found: X = {0}  Y = {1}", outerX, outerY));

                            if (!result.MatchLocations.Where(x => x.X == outerX && x.Y == outerY).Any())
                            {
                                result.MatchLocations.Add(new Point(outerX, outerY));
                            }
                        }
                    }
                }
            }

            return result;
        }
        #endregion
    }
}
