﻿using MindMate.Model;
using RibbonLib.Controls;
using RibbonLib.Controls.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RibbonLib.Interop;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using MindMate.MetaModel;
using MindMate.Plugins;
using MindMate.View.EditorTabs;
using MindMate.View.MapControls;
using MindMate.View.MapControls.Drawing;
using MindMate.Win7.Properties;
using RibbonLib;

namespace MindMate.View.Ribbon
{
    public partial class Ribbon
    {
        private readonly Controller.MainCtrl mainCtrl;
        private readonly RibbonLib.Ribbon ribbon;

        public Ribbon(RibbonLib.Ribbon ribbon, Controller.MainCtrl mainCtrl, EditorTabs.EditorTabs tabs)
        {
            this.ribbon = ribbon;
            this.mainCtrl = mainCtrl;

            InitializeComponents();

            HelpButton.ExecuteEvent += HelpButton_ExecuteEvent;

            //Application Menu
            ApplicationMenu.TooltipTitle = "Menu";
            ApplicationMenu.TooltipDescription = "Application main menu";

            ButtonNew.ExecuteEvent += _buttonNew_ExecuteEvent;
            ButtonExit.ExecuteEvent += _buttonExit_ExecuteEvent;
            ButtonOpen.ExecuteEvent += _buttonOpen_ExecuteEvent;
            ButtonSave.ExecuteEvent += _buttonSave_ExecuteEvent;
            SaveAs.ExecuteEvent += SaveAs_ExecuteEvent;
            SaveAll.ExecuteEvent += SaveAll_ExecuteEvent;
            ExportAsPNG.ExecuteEvent += ExportAsPNG_ExecuteEvent;
            ExportAsJPG.ExecuteEvent += ExportAsJPG_ExecuteEvent;
            Close.ExecuteEvent += Close_ExecuteEvent;

            RecentItems.RecentItems = CreateRecentItemsList();
            RecentItems.ExecuteEvent += RecentItems_ExecuteEvent;

            //Home Tab : New Node group
            NewChildNode.ExecuteEvent += NewChildNode_ExecuteEvent;
            NewLongNode.ExecuteEvent += NewLongNode_ExecuteEvent;
            NewNodeAbove.ExecuteEvent += NewNodeAbove_ExecuteEvent;
            NewNodeBelow.ExecuteEvent += NewNodeBelow_ExecuteEvent;
            NewNodeParent.ExecuteEvent += NewParent_ExecuteEvent;

            //Home Tab: Edit group
            EditText.ExecuteEvent += _btnEditText_ExecuteEvent;
            EditLong.ExecuteEvent += _btnEditLong_ExecuteEvent;
            DeleteNode.ExecuteEvent += _btnDeleteNode_ExecuteEvent;

            //Home Tab: Cipboard group
            Paste.ExecuteEvent += _btnPaste_ExecuteEvent;
            PasteAsText.ExecuteEvent += _btnPasteAsText_ExecuteEvent;
            Cut.ExecuteEvent += _btnCut_ExecuteEvent;
            Copy.ExecuteEvent += _btnCopy_ExecuteEvent;
            FormatPainter.ExecuteEvent += _btnFormatPainter_ExecuteEvent;

            //Home Tab: Font group
            RichFont.ExecuteEvent += _RichFont_ExecuteEvent;
            
            //Home Tab: Icons Group
            IconsGallery.ItemsSourceReady += _iconGallery_ItemsSourceReady;
            IconsGallery.ExecuteEvent += _iconGallery_ExecuteEvent;
            LaunchIconsDialog.ExecuteEvent += _launchIconsDialog_ExecuteEvent;
            RemoveLastIcon.ExecuteEvent += _removeLastIcon_ExecuteEvent;
            RemoveAllIcons.ExecuteEvent += _removeAllIcons_ExecuteEvent;

            //Edit Tab: Select Group
            SelectAll.ExecuteEvent += SelectAll_ExecuteEvent;
            SelectSiblings.ExecuteEvent += SelectSiblings_ExecuteEvent;
            SelectAncestors.ExecuteEvent += SelectAncestors_ExecuteEvent;
            SelectChildren.ExecuteEvent += SelectChildren_ExecuteEvent;
            SelectDescendents.ExecuteEvent += SelectDescendents_ExecuteEvent;
            SelectDescendentsUpto1.ExecuteEvent += SelectDescendentsUpto1_ExecuteEvent;
            SelectDescendentsUpto2.ExecuteEvent += SelectDescendentsUpto2_ExecuteEvent;
            SelectDescendentsUpto3.ExecuteEvent += SelectDescendentsUpto3_ExecuteEvent;
            SelectDescendentsUpto4.ExecuteEvent += SelectDescendentsUpto4_ExecuteEvent;
            SelectDescendentsUpto5.ExecuteEvent += SelectDescendentsUpto5_ExecuteEvent;
            SelectLevel1.ExecuteEvent += SelectLevel1_ExecuteEvent;
            SelectLevel2.ExecuteEvent += SelectLevel2_ExecuteEvent;
            SelectLevel3.ExecuteEvent += SelectLevel3_ExecuteEvent;
            SelectLevel4.ExecuteEvent += SelectLevel4_ExecuteEvent;
            SelectLevel5.ExecuteEvent += SelectLevel5_ExecuteEvent;
            SelectLevelCurrent.ExecuteEvent += SelectLevelCurrent_ExecuteEvent;

            //Edit Tab: Expand / Collapse Group
            ExpandAll.ExecuteEvent += ExpandAll_ExecuteEvent;
            CollapseAll.ExecuteEvent += CollapseAll_ExecuteEvent;
            ToggleCurrent.ExecuteEvent += ToggleCurrent_ExecuteEvent;
            ToggleBranch.ExecuteEvent += ToggleBranch_ExecuteEvent;
            ExpandMapToCurrentLevel.ExecuteEvent += ExpandMapToCurrentLevel_ExecuteEvent;
            ExpandMapToLevel1.ExecuteEvent += ExpandMapToLevel1_ExecuteEvent;
            ExpandMapToLevel2.ExecuteEvent += ExpandMapToLevel2_ExecuteEvent;
            ExpandMapToLevel3.ExecuteEvent += ExpandMapToLevel3_ExecuteEvent;
            ExpandMapToLevel4.ExecuteEvent += ExpandMapToLevel4_ExecuteEvent;
            ExpandMapToLevel5.ExecuteEvent += ExpandMapToLevel5_ExecuteEvent;

            //Edit Tab: Navigate Group
            NavigateToCenter.ExecuteEvent += NavigateToCenter_ExecuteEvent;
            NavigateToFirstSibling.ExecuteEvent += NavigateToFirstSibling_ExecuteEvent;
            NavigateToLastSibling.ExecuteEvent += NavigateToLastSibling_ExecuteEvent;

            //Edit Tab: Move
            MoveUp.ExecuteEvent += MoveUp_ExecuteEvent;
            MoveDown.ExecuteEvent += MoveDown_ExecuteEvent;

            //Edit Tab: Sort
            SortAlphabetic.ExecuteEvent += SortAlphabetic_ExecuteEvent;
            SortDueDate.ExecuteEvent += SortDueDate_ExecuteEvent;
            SortNodeCount.ExecuteEvent += SortNodeCount_ExecuteEvent;
            SortModifiedDate.ExecuteEvent += SortModifiedDate_ExecuteEvent;
            SortCreateDate.ExecuteEvent += SortCreateDate_ExecuteEvent;
            SortOrder.ExecuteEvent += SortOrder_ExecuteEvent;

            SortOrder.BooleanValue = true;

            //Edit Tab: Undo / Redo
            Undo.ExecuteEvent += Undo_ExecuteEvent;
            Redo.ExecuteEvent += Redo_ExecuteEvent;

            //Insert Tab: Hyperlink
            Hyperlink.ExecuteEvent += Hyperlink_ExecuteEvent;
            HyperlinkFile.ExecuteEvent += HyperlinkFile_ExecuteEvent;
            HyperlinkFolder.ExecuteEvent += HyperlinkFolder_ExecuteEvent;
            RemoveHyperlink.ExecuteEvent += RemoveHyperlink_ExecuteEvent;

            //Insert Tab: Note
            InsertNote.ExecuteEvent += InsertNote_ExecuteEvent;

            //Format Tab: Node Format
            NodeShape.ItemsSourceReady += NodeShape_ItemsSourceReady;
            NodeShape.ExecuteEvent += NodeShape_ExecuteEvent;
            ClearShapeFormat.ExecuteEvent += ClearShapeFormat_ExecuteEvent;
            LineColor.ExecuteEvent += LineColor_ExecuteEvent;
            LinePatternSolid.ExecuteEvent += LinePatternSolid_ExecuteEvent;
            LinePatternDashed.ExecuteEvent += LinePatternDashed_ExecuteEvent;
            LinePatternDotted.ExecuteEvent += LinePatternDotted_ExecuteEvent;
            LinePatternMixed.ExecuteEvent += LinePatternMixed_ExecuteEvent;
            LineThickness1.ExecuteEvent += LineThickness1_ExecuteEvent;
            LineThickness2.ExecuteEvent += LineThickness2_ExecuteEvent;
            LineThickness4.ExecuteEvent += LineThickness4_ExecuteEvent;
            NodeStyleGallery.ExecuteEvent += NodeStyleGallery_ExecuteEvent;

            //Format Tab: Node Style
            CreateNodeStyle.ExecuteEvent += CreateNodeStyle_ExecuteEvent;

            //register for change events
            mainCtrl.PersistenceManager.CurrentTreeChanged += PersistenceManager_CurrentTreeChanged;
            MindMate.Model.ClipboardManager.StatusChanged += ClipboardManager_StatusChanged;
            tabs.ControlAdded += Tabs_ControlAdded;
            tabs.ControlRemoved += Tabs_ControlRemoved;
            tabs.SelectedIndexChanged += Tabs_SelectedIndexChanged;

        }

        /// <summary>
        /// Setting certain properties (like image) doesn't work if Ribbon is not loaded. Use this method to set such properties.
        /// </summary>
        public void OnRibbonLoaded()
        {
            NodeShape.LargeImage = ribbon.ConvertToUIImage(Resources.Node_Format_Bubble);
        }

        public void SetupPluginCommands(MainMenuItem[] pluginItems)
        {
            MainMenuItem mTask = pluginItems[0];

            var handlerAddTask = mTask.DropDownItems[0].Click;
            AddTask.ExecuteEvent += (sender, args) => handlerAddTask(sender, args);

            var handlerAddTaskToday = mTask.DropDownItems[1].Click;
            AddTaskToday.ExecuteEvent += (sender, args) => handlerAddTaskToday(sender, args);

            var handlerAddTaskTomorrow = mTask.DropDownItems[2].Click;
            AddTaskTomorrow.ExecuteEvent += (sender, args) => handlerAddTaskTomorrow(sender, args);

            var handlerAddTaskNextWeek = mTask.DropDownItems[3].Click;
            AddTaskNextWeek.ExecuteEvent += (sender, args) => handlerAddTaskNextWeek(sender, args);

            var handlerAddTaskNextMonth = mTask.DropDownItems[4].Click;
            AddTaskNextMonth.ExecuteEvent += (sender, args) => handlerAddTaskNextMonth(sender, args);

            var handlerAddTaskNextQuarter = mTask.DropDownItems[5].Click;
            AddTaskNextQuarter.ExecuteEvent += (sender, args) => handlerAddTaskNextQuarter(sender, args);

            var handlerCompleteTask = mTask.DropDownItems[6].Click;
            CompleteTask.ExecuteEvent += (sender, args) => handlerCompleteTask(sender, args);

            var handlerRemoveTask = mTask.DropDownItems[7].Click;
            RemoveTask.ExecuteEvent += (sender, args) => handlerRemoveTask(sender, args);
        }

        #region Application Menu

        private void _buttonOpen_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.OpenMap();
        }

        private void _buttonExit_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        void _buttonNew_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.NewMap();
        }

        private void _buttonSave_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.SaveCurrentMap();
        }

        private void SaveAs_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.SaveCurrentMapAs();
        }

        private void SaveAll_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.SaveAll();
        }

        private void ExportAsPNG_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.ExportAsPng();
        }

        private void ExportAsJPG_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.ExportAsJpg();
        }

        private void Close_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CloseCurrentMap();
        }

        private List<RecentItemsPropertySet> CreateRecentItemsList()
        {
            var recentItems = new List<RecentItemsPropertySet>(MetaModel.MetaModel.RecentFilesCount);
            for (int i = 0; i < MetaModel.MetaModel.Instance.RecentFiles.Count; i++)
            {
                string recentFile = MetaModel.MetaModel.Instance.RecentFiles[i];
                recentItems.Add(new RecentItemsPropertySet()
                {
                    Label = System.IO.Path.GetFileName(recentFile),
                    LabelDescription = recentFile,
                    Pinned = false
                });
            }
            return recentItems;
        }

        public void RefreshRecentItemsList()
        {
            var ribbonRecentItems = RecentItems.RecentItems;
            var recentFiles = MetaModel.MetaModel.Instance.RecentFiles;

            // start: make two list same in size
            int countDiff = recentFiles.Count - ribbonRecentItems.Count;
            if (countDiff != 0)
            {
                if (countDiff > 0)
                {
                    do
                    {
                        ribbonRecentItems.Add(new RecentItemsPropertySet());
                        countDiff--;
                    } while (countDiff != 0);
                }
                else
                {
                    do
                    {
                        ribbonRecentItems.RemoveAt(ribbonRecentItems.Count - 1);
                        countDiff++;
                    } while (countDiff != 0);
                }
            }
            // end: make two list same in size

            // refresh ribbon recent items list
            for (int i = 0; i < recentFiles.Count; i++)
            {
                string fileName = recentFiles[i];
                ribbonRecentItems[i].Label = System.IO.Path.GetFileName(fileName);
                ribbonRecentItems[i].LabelDescription = fileName;
                ribbonRecentItems[i].Pinned = false;
            }
        }

        private void RecentItems_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            if (e.Key.PropertyKey == RibbonProperties.SelectedItem)
            {
                // get selected item label description
                PropVariant propLabelDescription;
                e.CommandExecutionProperties.GetValue(ref RibbonProperties.LabelDescription,
                                                    out propLabelDescription);
                string labelDescription = (string)propLabelDescription.Value;
                
                // open file
                mainCtrl.OpenMap(labelDescription);
            }
        }

        private void HelpButton_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.ShowAboutBox();
        }

        #endregion

        #region Home Tab

        private void NewChildNode_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.AppendChildNodeAndEdit();
        }

        private void NewLongNode_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.AppendMultiLineNodeAndEdit();
        }

        private void NewNodeAbove_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.AppendSiblingAboveAndEdit();
        }

        private void NewNodeBelow_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.AppendSiblingNodeAndEdit();
        }

        private void NewParent_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.InsertParentAndEdit();
        }

        private void _btnEditText_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.BeginCurrentNodeEdit(MapControls.TextCursorPosition.Undefined);
        }

        private void _btnEditLong_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.MultiLineNodeEdit();
        }

        private void _btnDeleteNode_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.DeleteSelectedNodes();
        }

        private void _btnPaste_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.Paste();
        }

        private void _btnPasteAsText_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.Paste(true);
        }

        private void _btnCut_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.Cut();
        }

        private void _btnCopy_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.Copy();
        }

        private void _btnFormatPainter_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            if (FormatPainter.BooleanValue)
            {
                bool ctrlKeyDown = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                mainCtrl.CurrentMapCtrl.CopyFormat(ctrlKeyDown);
            }
            else
            {
                mainCtrl.CurrentMapCtrl.ClearFormatPainter();
            }
        }

        private void _RichFont_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            
            PropVariant propChangesProperties;
            e.CommandExecutionProperties.GetValue(ref RibbonProperties.FontProperties_ChangedProperties, out propChangesProperties);
            IPropertyStore changedProperties = (IPropertyStore)propChangesProperties.Value;
            uint changedPropertiesNumber;
            changedProperties.GetCount(out changedPropertiesNumber);

            for (uint i = 0; i < changedPropertiesNumber; ++i)
            {
                PropertyKey propertyKey;
                changedProperties.GetAt(i, out propertyKey);
                
                //get property name
                //Debug.WriteLine(RibbonProperties.GetPropertyKeyName(ref propertyKey));

                if (propertyKey == RibbonProperties.FontProperties_Bold)
                {
                    mainCtrl.CurrentMapCtrl.ToggleSelectedNodeBold();
                }
                else if (propertyKey == RibbonProperties.FontProperties_Italic)
                {
                    mainCtrl.CurrentMapCtrl.ToggleSelectedNodeItalic();
                }
                else if (propertyKey == RibbonProperties.FontProperties_Strikethrough)
                {
                    mainCtrl.CurrentMapCtrl.ToggleSelectedNodeStrikeout();
                }
                else if (propertyKey == RibbonProperties.FontProperties_Family)
                {
                    mainCtrl.CurrentMapCtrl.SetFontFamily(RichFont.Family);
                }
                else if (propertyKey == RibbonProperties.FontProperties_Size)
                {
                    mainCtrl.CurrentMapCtrl.SetFontSize((float)RichFont.Size);
                }
                else if (propertyKey == RibbonProperties.FontProperties_BackgroundColor)
                {
                    mainCtrl.CurrentMapCtrl.ChangeBackColor(RichFont.BackgroundColor);
                }
                else if (propertyKey == RibbonProperties.FontProperties_BackgroundColorType)
                {
                    mainCtrl.CurrentMapCtrl.ChangeBackColor(Color.Empty);
                }
                else if (propertyKey == RibbonProperties.FontProperties_ForegroundColor)
                {
                    mainCtrl.CurrentMapCtrl.ChangeTextColor(RichFont.ForegroundColor);
                }
                else if (propertyKey == RibbonProperties.FontProperties_ForegroundColorType)
                {
                    mainCtrl.CurrentMapCtrl.ChangeTextColor(Color.Empty);
                }
            }
        }

        private void _iconGallery_ItemsSourceReady(object sender, EventArgs e)
        {
            IUICollection itemsSource = IconsGallery.ItemsSource;
            itemsSource.Clear();

            foreach (ModelIcon icon in MetaModel.MetaModel.Instance.IconsList)
            {
                itemsSource.Add(new GalleryIconPropertySet(icon, ribbon));
            }

            RemoveLastIcon.SmallImage = ribbon.ConvertToUIImage(MindMate.Properties.Resources.minus);
            RemoveAllIcons.SmallImage = ribbon.ConvertToUIImage(MindMate.Properties.Resources.cross_script);
            LaunchIconsDialog.SmallImage = ribbon.ConvertToUIImage(MindMate.Properties.Resources.smartart_change_color_gallery_16);
        }

        private void _iconGallery_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            GalleryIconPropertySet iconPropertySet = (GalleryIconPropertySet) e.CommandExecutionProperties;
            if (iconPropertySet != null)
            {
                mainCtrl.CurrentMapCtrl.AppendIcon(iconPropertySet.Name);
            }
        }

        private void _launchIconsDialog_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.AppendIconFromIconSelectorExt();
        }

        private void _removeLastIcon_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.RemoveLastIcon();
        }

        private void _removeAllIcons_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.RemoveAllIcon();
        }

        #endregion Home Tab

        #region Edit Tab

        private void SelectAll_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectAllNodes(ExpandOnSelect.BooleanValue);
        }

        private void SelectSiblings_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectSiblings();
        }

        private void SelectAncestors_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectAncestors();
        }

        private void SelectChildren_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectChildren(true);
        }

        private void SelectDescendents_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectDescendents(ExpandOnSelect.BooleanValue);
        }

        private void SelectDescendentsUpto1_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectDescendents(1, ExpandOnSelect.BooleanValue);
        }

        private void SelectDescendentsUpto2_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectDescendents(2, ExpandOnSelect.BooleanValue);
        }

        private void SelectDescendentsUpto3_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectDescendents(3, ExpandOnSelect.BooleanValue);
        }

        private void SelectDescendentsUpto4_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectDescendents(4, ExpandOnSelect.BooleanValue);
        }

        private void SelectDescendentsUpto5_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectDescendents(5, ExpandOnSelect.BooleanValue);
        }

        private void SelectLevel1_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectLevel(1, false, ExpandOnSelect.BooleanValue);
        }

        private void SelectLevel2_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectLevel(2, false, ExpandOnSelect.BooleanValue);
        }

        private void SelectLevel3_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectLevel(3, false, ExpandOnSelect.BooleanValue);
        }

        private void SelectLevel4_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectLevel(4, false, ExpandOnSelect.BooleanValue);
        }

        private void SelectLevel5_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectLevel(5, false, ExpandOnSelect.BooleanValue);
        }

        private void SelectLevelCurrent_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectCurrentLevel(ExpandOnSelect.BooleanValue);
        }

        private void ExpandAll_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.UnfoldAll();
        }

        private void CollapseAll_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.FoldAll();
        }

        private void ToggleCurrent_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.ToggleFolded();
        }

        private void ToggleBranch_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.ToggleBranchFolding();
        }

        private void ExpandMapToCurrentLevel_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.UnfoldMapToCurrentLevel();
        }

        private void ExpandMapToLevel1_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.UnfoldMapToLevel(1);
        }

        private void ExpandMapToLevel2_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.UnfoldMapToLevel(2);
        }

        private void ExpandMapToLevel3_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.UnfoldMapToLevel(3);
        }

        private void ExpandMapToLevel4_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.UnfoldMapToLevel(4);
        }

        private void ExpandMapToLevel5_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.UnfoldMapToLevel(5);
        }

        private void NavigateToCenter_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectRootNode();
        }

        private void NavigateToFirstSibling_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectTopSibling();
        }

        private void NavigateToLastSibling_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.SelectBottomSibling();
        }

        private void MoveUp_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.MoveNodeUp();
        }

        private void MoveDown_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.MoveNodeDown();
        }

        private const string AscendingOrderString = "Ascending Order";
        private const string DescendingOrderString = "Descending Order";

        private bool IsAscendingSortOrder { get { return SortOrder.Label == AscendingOrderString || SortOrder.Label == null; }}

        private void SortAlphabetic_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            if (IsAscendingSortOrder)
            {
                mainCtrl.CurrentMapCtrl.SortAlphabeticallyAsc();
            }
            else
            {
                mainCtrl.CurrentMapCtrl.SortAlphabeticallyDesc();
            }
        }

        private void SortDueDate_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            if (IsAscendingSortOrder)
            {
                mainCtrl.CurrentMapCtrl.SortByTaskAsc();
            }
            else
            {
                mainCtrl.CurrentMapCtrl.SortByTaskDesc();
            }

        }

        private void SortNodeCount_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            if (IsAscendingSortOrder)
            {
                mainCtrl.CurrentMapCtrl.SortByDescendentsCountAsc();
            }
            else
            {
                mainCtrl.CurrentMapCtrl.SortByDescendentsCountDesc();
            }
        }

        private void SortModifiedDate_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            if (IsAscendingSortOrder)
            {
                mainCtrl.CurrentMapCtrl.SortByModifiedDateAsc();
            }
            else
            {
                mainCtrl.CurrentMapCtrl.SortByModifiedDateDesc();
            }
        }

        private void SortCreateDate_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            if (IsAscendingSortOrder)
            {
                mainCtrl.CurrentMapCtrl.SortByCreateDateAsc();
            }
            else
            {
                mainCtrl.CurrentMapCtrl.SortByCreateDateDesc();
            }
        }

        private void SortOrder_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            if (SortOrder.Label == null || SortOrder.Label.Equals(AscendingOrderString)) //ascending, swtich to descending
            {
                SortOrder.BooleanValue = true;
                SortOrder.Label = DescendingOrderString;
                SortOrder.TooltipTitle = "Sort Order: Descending";
                SortOrder.SmallImage = ribbon.ConvertToUIImage(Resources.Descending_Sorting_32);
            }
            else //switch to ascending
            {
                SortOrder.BooleanValue = true;
                SortOrder.Label = AscendingOrderString;
                SortOrder.TooltipTitle = "Sort Order: Ascending";
                SortOrder.SmallImage = ribbon.ConvertToUIImage(Resources.Ascending_Sorting_32);
            }
            
        }

        private void Undo_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.Undo();
        }

        private void Redo_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.Redo();
        }

        #endregion

        #region Insert Tab

        private void Hyperlink_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.AddHyperlinkUsingTextbox();
        }

        private void HyperlinkFile_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.AddHyperlinkUsingFileDialog();
        }

        private void HyperlinkFolder_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.AddHyperlinkUsingFolderDialog();
        }

        private void RemoveHyperlink_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.RemoveHyperlink();
        }

        private void InsertNote_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.StartNoteEditing();
        }

        #endregion

        #region Format Tab

        private void NodeShape_ItemsSourceReady(object sender, EventArgs e)
        {
            var itemSource = NodeShape.ItemsSource;
            itemSource.Clear();

            itemSource.Add(new GalleryItemPropertySet()
            {
                Label = "Fork",
                ItemImage = ribbon.ConvertToUIImage(Resources.Node_Format_Fork)
            });
            itemSource.Add(new GalleryItemPropertySet()
            {
                Label = "Bubble",
                ItemImage = ribbon.ConvertToUIImage(Resources.Node_Format_Bubble)
            });
            itemSource.Add(new GalleryItemPropertySet()
            {
                Label = "Box",
                ItemImage = ribbon.ConvertToUIImage(Resources.Node_Format_Box)
            });
            itemSource.Add(new GalleryItemPropertySet()
            {
                Label = "Bullet",
                ItemImage = ribbon.ConvertToUIImage(Resources.Node_Format_Bullet)
            });
        }

        private void NodeShape_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            var item = (GalleryItemPropertySet)e.CommandExecutionProperties;
            if (item != null)
            {
                mainCtrl.CurrentMapCtrl.ChangeNodeShape(item.Label);
            }
        }

        private void ClearShapeFormat_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.ClearNodeShape();
        }

        private void LineColor_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.ChangeLineColor(LineColor.Color);
        }

        private void LinePatternSolid_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.ChangeLinePattern(DashStyle.Solid);
        }

        private void LinePatternDashed_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.ChangeLinePattern(DashStyle.Dash);
        }

        private void LinePatternDotted_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.ChangeLinePattern(DashStyle.Dot);
        }

        private void LinePatternMixed_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.ChangeLinePattern(DashStyle.DashDotDot);
        }

        private void LineThickness1_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.ChangeLineWidth(1);
        }

        private void LineThickness2_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.ChangeLineWidth(2);
        }

        private void LineThickness4_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            mainCtrl.CurrentMapCtrl.ChangeLineWidth(4);
        }

        private void CreateNodeStyle_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            var style = mainCtrl.CurrentMapCtrl.CreateNodeStyle();
            if (style != null)
            {
                NodeStyleGallery.ItemsSource.Add(new GalleryNodeStylePropertySet(style, ribbon));
            }
        }

        private void NodeStyleGallery_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            var galleryStyle = (GalleryNodeStylePropertySet)e.CommandExecutionProperties;
            if (galleryStyle != null)
            {
                mainCtrl.CurrentMapCtrl.ApplyNodeStyle(galleryStyle.NodeStyle);
            }
        }

        #endregion

        #region Events to Refresh Command State

        private void PersistenceManager_CurrentTreeChanged(Serialization.PersistenceManager manager, Serialization.PersistentTree oldTree, Serialization.PersistentTree newTree)
        {
            if (oldTree != null)
            {
                oldTree.Tree.SelectedNodes.NodeSelected -= SelectedNodes_NodeSelected;
                oldTree.Tree.SelectedNodes.NodeDeselected -= SelectedNodes_NodeDeselected;
                oldTree.Tree.NodePropertyChanged -= Tree_NodePropertyChanged;
                oldTree.Tree.IconChanged -= Tree_IconChanged;
                oldTree.Tree.TreeStructureChanged -= Tree_TreeStructureChanged;
                oldTree.Tree.AttributeChanged -= Tree_AttributeChanged;
                oldTree.Tree.AttributeSpecChangeEvent -= Tree_AttributeSpecChangeEvent;
            }

            if (newTree != null)
            {
                newTree.Tree.SelectedNodes.NodeSelected += SelectedNodes_NodeSelected;
                newTree.Tree.SelectedNodes.NodeDeselected += SelectedNodes_NodeDeselected;
                newTree.Tree.NodePropertyChanged += Tree_NodePropertyChanged;
                newTree.Tree.IconChanged += Tree_IconChanged;
                newTree.Tree.TreeStructureChanged += Tree_TreeStructureChanged;
                newTree.Tree.AttributeChanged += Tree_AttributeChanged;
                newTree.Tree.AttributeSpecChangeEvent += Tree_AttributeSpecChangeEvent;

                UpdateFontControl(newTree.Tree.SelectedNodes);
                UpdateUndoGroup(newTree.Tree);
            }
            else
            {
                ClearFontControl();
            }
            
        }

        private void Tree_NodePropertyChanged(MapNode node, NodePropertyChangedEventArgs e)
        {
            if (node.Tree == mainCtrl.PersistenceManager.CurrentTree.Tree)
            {
                if (e.ChangedProperty == NodeProperties.Bold || e.ChangedProperty == NodeProperties.Italic
                    || e.ChangedProperty == NodeProperties.Strikeout || e.ChangedProperty == NodeProperties.FontName
                    || e.ChangedProperty == NodeProperties.FontSize)
                {
                    UpdateFontControl(node.Tree.SelectedNodes);
                }
                UpdateUndoGroup(node.Tree);
            }
        }

        private void Tree_IconChanged(MapNode node, IconChangedEventArgs arg2)
        {
            UpdateUndoGroup(node.Tree);
        }

        private void Tree_TreeStructureChanged(MapNode node, TreeStructureChangedEventArgs arg2)
        {
            UpdateUndoGroup(node.Tree);
        }

        private void Tree_AttributeChanged(MapNode node, AttributeChangeEventArgs arg2)
        {
            UpdateUndoGroup(node.Tree);
        }

        private void Tree_AttributeSpecChangeEvent(MapTree.AttributeSpec aSpec, MapTree.AttributeSpecEventArgs e)
        {
            UpdateUndoGroup(aSpec.Tree);
        }

        private void SelectedNodes_NodeSelected(MapNode node, SelectedNodes selectedNodes)
        {
            UpdateFontControl(selectedNodes);
        }

        private void SelectedNodes_NodeDeselected(MapNode node, SelectedNodes selectedNodes)
        {
            UpdateFontControl(selectedNodes);
        }

        private void UpdateFontControl(SelectedNodes nodes)
        {
            if (nodes.Count == 1)
            {
                MapNode n = nodes.First;
                RichFont.Bold = n.Bold ? FontProperties.Set : FontProperties.NotSet;
                RichFont.Italic = n.Italic ? FontProperties.Set : FontProperties.NotSet;
                RichFont.Strikethrough = n.Strikeout ? FontProperties.Set : FontProperties.NotSet;
                RichFont.Family = n.NodeView.Font.Name;
                RichFont.Size = (decimal)n.NodeView.Font.Size;
            }
            else
            {
                ClearFontControl();
            }
        }

        private void ClearFontControl()
        {
            RichFont.Bold = FontProperties.NotSet;
            RichFont.Italic = FontProperties.NotSet;
            RichFont.Strikethrough = FontProperties.NotSet;
            RichFont.Family = null;
            RichFont.Size = 0;
        }

        private void UpdateUndoGroup(MapTree tree)
        {
            Undo.Enabled = tree.ChangeManager.CanUndo;
            Redo.Enabled = tree.ChangeManager.CanRedo;
        }

        private void ClipboardManager_StatusChanged()
        {
            if (ClipboardManager.HasCutNode)
            {
                Cut.SmallImage = ribbon.ConvertToUIImage(Win7.Properties.Resources.cut_red_small);
            }
            else
            {
                Cut.SmallImage = ribbon.ConvertToUIImage(Win7.Properties.Resources.cut_small);
            }
        }

        private void Tabs_ControlAdded(object sender, ControlEventArgs e)
        {
            Tab tab = e.Control as Tab;
            if (tab != null)
            {
                tab.MapView.FormatPainter.StateChanged += FormatPainter_StateChanged;
            }
        }

        private void Tabs_ControlRemoved(object sender, ControlEventArgs e)
        {
            Tab tab = e.Control as Tab;
            if (tab != null)
            {
                tab.MapView.FormatPainter.StateChanged -= FormatPainter_StateChanged;
            }
        }

        private void Tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            Tab tab = ((EditorTabs.EditorTabs)sender).SelectedTab as Tab;
            if (tab != null)
            {
                UpdateFormatPainter(tab.MapView.FormatPainter);
            }
            else
            {
                FormatPainter.BooleanValue = false;
            }
        }

        private void FormatPainter_StateChanged(MapControls.MapViewFormatPainter painter)
        {
            UpdateFormatPainter(painter);
        }

        private void UpdateFormatPainter(MapControls.MapViewFormatPainter painter)
        {
            switch (painter.Status)
            {
                case MapControls.FormatPainterStatus.Empty:
                    FormatPainter.BooleanValue = false;
                    break;
                case MapControls.FormatPainterStatus.SingleApply:
                case MapControls.FormatPainterStatus.MultiApply:
                    FormatPainter.BooleanValue = true;
                    break;
            }
        }

        #endregion

    }
}
