using System;
using System.Collections.Generic;
using System.Text;
using CosmosTTF;
using TechOS.System;
using IL2CPU.API.Attribs;
using Sys = Cosmos.System;
using MIV;
using System.IO;
using TechOS.GUI;
using Cosmos.System.Graphics;
using System.Drawing;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.FileSystem;
using Cosmos.Core;
using Cosmos.Core.Memory;

namespace TechOS
{
    public class Kernel : Sys.Kernel
    {
        [ManifestResourceStream(ResourceName = "TechOS.Resources.Wallpaper.bmp")]
        public static byte[] Wallpaper;
        public static Bitmap wallpaper = new Bitmap(Wallpaper);
        public static Canvas canvas;
        public static string file;
        public static Sys.FileSystem.CosmosVFS fs;
        public static string current_directory = "0:\\";
        protected override void BeforeRun()
        {
            Console.Clear();
            Console.WriteLine("[INFO] File system loading");
            fs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            Console.WriteLine("[INFO] File system loaded\n");
            Console.WriteLine("Welcome to TechOS");
            Console.Write("Read config file? (y/n)");
            string configyn = Console.ReadLine();
            if (configyn == "y")
            {
                ConfigFile.LoadSystemConfig("0:\\Config\\system.cfg");
                Console.Clear();
                Console.WriteLine("[INFO] Config file loaded!");
                Console.WriteLine();
            }
            if (configyn == "n")
            {
                Console.WriteLine();
            }
        }

        protected override void Run()
        {
            Console.Write(current_directory + ">");
            string cmd = Console.ReadLine();
            if (cmd == "miv")
            {
                MIV.MIV.StartMIV();
            }
            else if (cmd == "dir")
            {
                string[] dirs = GetDirFadr(current_directory);
                string[] files = GetFilesFadr(current_directory);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                foreach (var item in dirs)
                {
                    Console.WriteLine(item);
                }
                Console.ForegroundColor = ConsoleColor.White;
                foreach (var item in files)
                {
                    Console.WriteLine(item);
                }
            }
            else if (cmd == "cdroot")
            {
                current_directory = "0:\\";
            }
            else if (cmd == "gui")
            {
                // backup canvases
                //
                // canvas = new VBECanvas(new Mode(1024, 768, ColorDepth.ColorDepth32));
                // canvas = new SVGAIICanvas(new Mode(1024, 768, ColorDepth.ColorDepth32));
                canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(1024, 768, ColorDepth.ColorDepth32));
                Sys.MouseManager.ScreenWidth = 1012;
                Sys.MouseManager.ScreenHeight = 768;
                while (true)
                {
                    Heap.Collect();
                    canvas.Clear(Color.White);
                    canvas.DrawImage(wallpaper, 0, 0);
                    Cursor.DrawCursor(canvas, Sys.MouseManager.X, Sys.MouseManager.Y);
                    canvas.Display();
                }
            }
            else if (cmd.StartsWith("cd "))
            {
                if (current_directory == "0:\\")
                {
                    if (Directory.Exists(current_directory + cmd.Remove(0, 3))) 
                    {
                        current_directory = current_directory + cmd.Remove(0, 3);
                    }
                }
                else
                {
                    if (Directory.Exists(current_directory + "\\" + cmd.Remove(0, 3)))
                    {
                        current_directory = current_directory + "\\" + cmd.Remove(0, 3);
                    }
                }
            }
            else if (cmd.StartsWith("mkdir "))
            {
                if (current_directory == "0:\\")
                {
                    Directory.CreateDirectory(current_directory + cmd.Remove(0, 6));
                }
                else
                {
                    Directory.CreateDirectory(current_directory + "\\" + cmd.Remove(0, 6));
                }
            }
            else
            {
                Console.WriteLine("[ERROR] Command not found.");
            }
        }
        private string[] GetDirFadr(string adr)
        {
            var dirs = Directory.GetDirectories(adr);
            return dirs;
        }
        private string[] GetFilesFadr(string adr)
        {
            var files = Directory.GetFiles(adr);
            return files;
        }
    }
}
