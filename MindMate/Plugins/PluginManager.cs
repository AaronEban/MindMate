﻿using MindMate.Controller;
using MindMate.Model;
using MindMate.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MindMate.Plugins
{
    public class PluginManager : IPluginManager
    {
        private readonly MainCtrl mainCtrl;

        public List<IPlugin> Plugins { get; private set; }

        public PluginManager(MainCtrl mainCtrl)
        {
            this.mainCtrl = mainCtrl;

            Plugins = new List<IPlugin>();

            LoadPlugins();

            mainCtrl.PersistenceManager.NewTreeCreating += PersistentManager_NewTreeCreating;
            mainCtrl.PersistenceManager.TreeOpening += PersistentManager_TreeOpening;
            mainCtrl.PersistenceManager.TreeClosing += PersistentManager_TreeClosing;

        }

        private void LoadPlugins()
        {
            Plugins.Add(new Tasks.TaskPlugin());
        }

        public void Initialize()
        {
            Plugins.ForEach(a => { 
                
                a.Initialize(this); 

                if(a is IPluginSystemIcon)
                {
                    var icons = ((IPluginSystemIcon)a).CreateSystemIcons();
                    MetaModel.MetaModel.Instance.SystemIconList.AddRange(icons);
                }
            });
        }

        public void InitializeContextMenu(ContextMenuCtrl contextMenuCtrl)
        {
            foreach(IPlugin p in Plugins)
            {
                IPluginMapNodeContextMenu plugin = p as IPluginMapNodeContextMenu;
                if (plugin != null)
                {
                    var menu = plugin.CreateContextMenuItemsForNode();
                    if (menu != null)
                        contextMenuCtrl.InsertMenuItems(menu);
                }
            }
        }

        internal void InitializeMainMenu(View.IMainForm mainManuCtrl)
        {
            foreach(IPlugin p in Plugins)
            {
                IPluginMainMenu plugin = p as IPluginMainMenu;
                if(plugin != null)
                {
                    var menu = plugin.CreateMainMenuItems();
                    if (menu != null)
                        mainManuCtrl.InsertMenuItems(menu);
                }
            }
        }

        public void InitializeSideBarWindow(TabControl sidebar)
        {
            foreach(IPlugin plugin in Plugins)
            {
                var controls = plugin.CreateSideBarWindows();
                
                foreach(Control ctrl in controls)
                {
                    TabPage tPage = new TabPage(ctrl.Text);
                    ctrl.Dock = DockStyle.Fill;
                    tPage.Controls.Add(ctrl);
                    sidebar.TabPages.Add(tPage);
                }
            }
        }

        internal void OnMapNodeContextMenuOpening(SelectedNodes selectedNodes)
        {
            for(int i = 0; i < Plugins.Count; i++)
            {
                IPluginMapNodeContextMenu plugin = Plugins[i] as IPluginMapNodeContextMenu;
                if(plugin != null)
                {
                    plugin.OnContextMenuOpening(selectedNodes);
                }
            }
        }

        internal void OnMainMenuOpening(Plugins.MainMenuLocation menu, SelectedNodes selectedNodes)
        {

        }

        internal void OnApplicationReady()
        {
            Plugins.ForEach(p => p.OnApplicationReady());
        }

        #region New/Open/Close Tree

        private void OnTreeCreating(MapTree tree)
        {
            foreach (IPlugin plugin in Plugins)
            {
                plugin.OnCreatingTree(tree);
            } 
        }

        private void OnTreeDeleting(MapTree tree)
        {
            foreach (IPlugin plugin in Plugins)
            {
                plugin.OnDeletingTree(tree);
            }
        }

        private void PersistentManager_NewTreeCreating(Serialization.PersistenceManager manager, Serialization.PersistentTree tree)
        {
            OnTreeCreating(tree.Tree);         
        }


        private void PersistentManager_TreeOpening(Serialization.PersistenceManager manager, Serialization.PersistentTree tree)
        {
            OnTreeCreating(tree.Tree);
        }

        private void PersistentManager_TreeClosing(Serialization.PersistenceManager manager, Serialization.PersistentTree tree)
        {
            OnTreeDeleting(tree.Tree);
        }

        #endregion New/Open/Close Tree

        #region IPluginManager Interface

        public void FocusMapEditor()
        {
            mainCtrl.ReturnFocusToMapView();
        }

        public void ScheduleTask(TaskSchedular.ITask task)
        {
            mainCtrl.ScheduleTask(task);
        }        

        public void RescheduleTask(TaskSchedular.ITask task, DateTime startTime)
        {
            mainCtrl.RescheduleTask(task, startTime);
        }

        /// <summary>
        /// Selected Nodes of the currently active MapTree
        /// </summary>
        public SelectedNodes ActiveNodes
        {
            get { return mainCtrl.ActiveNodes; }
        }

        #endregion IPluginManager Interface
    }
}
