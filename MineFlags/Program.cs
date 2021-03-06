﻿using MineFlags.Logic;
using MineFlags.RulesEngine;
using MineFlags.Storage;
using System;
using System.Threading;
using System.Windows.Forms;

namespace MineFlags
{
    static class Program
    {
        // Constants used int he application
        private const string FilePath = "data.xml";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {
            Thread th = Thread.CurrentThread;
            th.Name = "GUI thread";

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MineField(FilePath));
        }
    }
}
