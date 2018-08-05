using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using WindowScrape.Static;
using OsrsColorBot.LowLevelIO;

namespace OsrsColorBot
{
    public static class IoSimulator
    {
        public static void PauseThread(int milliseconds, double fraction = .1)
        {
            int buffer = Convert.ToInt32(milliseconds * fraction);
            Thread.Sleep(new Random().Next(milliseconds - buffer, milliseconds + buffer));
        }

        public static bool CheckUserIdleStatus()
        {
            var userIsIdle = false;

            var idleSeconds = TimeSpan.FromTicks(GetLastUserInput.GetIdleTickCount()).TotalSeconds * 10000; // multiply by 10,000 to adjust the low level data to C# data type

            if (idleSeconds > 10)//UserConfigOptions.IdleThresholdSec)
            {
                userIsIdle = true;
            }

            return userIsIdle;
        }

        public static void WiggleMouse()
        {
            Cursor.Position = new Point(Cursor.Position.X + 10, Cursor.Position.Y);
            PauseThread(500);
            Cursor.Position = new Point(Cursor.Position.X - 10, Cursor.Position.Y);
        }

        public static void ClickLocation(Point location, bool leftClick = true)
        {
            var osrsWindow = HwndInterface.GetHwndFromTitle("Old School RuneScape");

            HwndInterface.ActivateWindow(osrsWindow);

            Cursor.Position = location;

            if (leftClick)
            {
                MouseEvent.MouseLeftClick();
            }
            else
            {
                MouseEvent.MouseRightClick();
            }
        }
    }
}
