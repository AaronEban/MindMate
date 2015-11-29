﻿using MindMate.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MindMate.Win7
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var traceLog = new System.Diagnostics.TextWriterTraceListener("MindMate_Trace.log");
            System.Diagnostics.Trace.Listeners.Add(traceLog);
            System.Diagnostics.Trace.AutoFlush = true;
            //MyWebMind.Debug.IconListCreator.GenerateIconXML();
            MainForm form = new MainForm();
            MainCtrl mainCtrl = new MainCtrl();
            mainCtrl.LaunchMindMate(form);
            var ribbonHandler = new View.Ribbon.Ribbon(form.Ribbon, mainCtrl);
            form.Load += (n, e) => ribbonHandler.Initialize();            
            Application.Run(form);
            traceLog.Close();
        }
    }
}
