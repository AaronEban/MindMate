﻿using MindMate.View.NoteEditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MindMate.View
{
    public sealed class SideTabControl : TabControl
    {
        private const string NoteTabTitle = "Note Editor";

        public SideTabControl()
        {
            Dock = DockStyle.Fill;
            //SideBarTabs.Alignment = TabAlignment.Bottom;

            ImageList imageList = new ImageList();
            imageList.Images.Add(MindMate.Properties.Resources.sticky_note_pin);
            ImageList = imageList;

            NoteTab = new TabPage(NoteTabTitle) { ImageIndex = 0 };
            NoteEditor = new NoteEditor { Dock = DockStyle.Fill };
            NoteTab.Controls.Add(NoteEditor);

            TabPages.Add(NoteTab);
        }

        public NoteEditor NoteEditor { get; set; }

        public TabPage NoteTab { get; set; }

    }
}
