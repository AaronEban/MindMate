﻿/* Author: Syed Umar Anis (mail@umaranis.com)                    
 * Copyright (c) 2014 Syed Umar Anis                             
 * This software is licensed under MIT (see LICENSE.txt)    
 */

using System;
using System.Windows.Forms;
using MindMate.Plugins;
using MindMate.View.EditorTabs;
using MindMate.View.NoteEditing;

namespace MindMate.WinXP
{
    //TODO: Compare with Win& and bring multiple tab functionality here
    public partial class MainForm : Form, View.IMainForm
    {
        private View.MapControls.MapViewPanel mapViewPanel;
                
        public MainForm()
        {
            InitializeComponent();
            toolStrip1.MainMenu = MainMenu;
            SetupSideBar();

            notesEditor.GotFocus += (a, b) => this.focusedControl = notesEditor; 

            // moving splitter makes it the focused control, below event focuses the last control again
            splitContainer1.GotFocus += (a, b) => FocusLastControl();

            // changing side bar tab gives focus away to tab control header, below event focuses relevant control again
            SideBarTabs.SelectedIndexChanged += SideBarTabs_SelectedIndexChanged;
        }

        public MainMenu MainMenu { get { return menuStrip; } }

        private void SideBarTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SideBarTabs.SelectedTab.Text == "Note Editor")
                SideBarTabs.SelectedTab.Controls[0].Focus();
            else
                FocusMapView();
        }

        private NoteEditor notesEditor;
        public NoteEditor NoteEditor
        {
            get { return this.notesEditor; }
        }

        private TabControl sideBarTabs;
        public TabControl SideBarTabs
        {
            get { return sideBarTabs; }
        }

        public View.StatusBar StatusBar { get { return this.statusStrip1; } }

        private Control focusedControl;

        private void FocusLastControl()
        {
            focusedControl?.Focus();
        }

        public bool IsNoteEditorActive
        {
            get { return ActiveControl == splitContainer1 && splitContainer1.ActiveControl == notesEditor; }
        }        

        public void FocusMapView()
        {
            mapViewPanel.Focus();
        }

        private void SetupSideBar()
        {
            sideBarTabs = new TabControl();
            sideBarTabs.Dock = DockStyle.Fill;

            ImageList imageList = new ImageList();
            imageList.Images.Add(MindMate.Properties.Resources.knotes);
            sideBarTabs.ImageList = imageList;

            TabPage tPage = new TabPage("Note Editor");
            tPage.ImageIndex = 0;
            notesEditor = new NoteEditor();
            notesEditor.Dock = DockStyle.Fill;
            tPage.Controls.Add(notesEditor);


            sideBarTabs.TabPages.Add(tPage);
            this.splitContainer1.Panel2.Controls.Add(sideBarTabs);
        }

        //TODO: Update this to support editor tabs
        public void AddMainView(View.MapControls.MapViewPanel mapViewPanel)
        {
            this.mapViewPanel = mapViewPanel;
            splitContainer1.Panel1.Controls.Add(mapViewPanel);
            mapViewPanel.MapView.CenterOnForm();
            mapViewPanel.GotFocus += (sender, e) => focusedControl = this.mapViewPanel;
        }

        public MainMenuCtrl MainMenuCtrl { get; set; }

        public EditorTabs EditorTabs
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void InsertMenuItems(MainMenuItem[] menuItems)
        {
            MainMenuCtrl.InsertMenuItems(menuItems);
        }

        public void RefreshRecentFilesMenuItems()
        {
            MainMenuCtrl.RefreshRecentFilesMenuItems();
        }
    }
}
