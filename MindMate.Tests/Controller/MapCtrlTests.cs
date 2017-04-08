﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MindMate.Controller;
using MindMate.Model;
using MindMate.Serialization;
using MindMate.View.MapControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using MindMate.Plugins.Tasks.Model;
using MindMate.Tests.TestDouble;

namespace MindMate.Tests.Controller
{
    /// <summary>
    /// Testing Guidelines:
    /// 1- Test for root node
    /// 2- Test for scenario with no selected node
    /// </summary>
    [TestClass()]
    public class MapCtrlTests
    {
        private MapCtrl SetupMapCtrlWithFeaureDisplay()
        {
            string xmlString = System.IO.File.ReadAllText(@"Resources\Feature Display.mm");

            MapTree tree = new MapTree();
            new MindMapSerializer().Deserialize(xmlString, tree);

            tree.SelectedNodes.Add(tree.RootNode, false);

            var form = new System.Windows.Forms.Form();
            MetaModel.MetaModel.Initialize();
            MetaModel.MetaModel.Instance.MapEditorBackColor = Color.White;
            MetaModel.MetaModel.Instance.NoteEditorBackColor = Color.White;
            MapCtrl mapCtrl = new MapCtrl(new MapView(tree), new MainCtrlStub(form));
            form.Controls.Add(mapCtrl.MapView.Canvas);

            tree.TurnOnChangeManager();

            return mapCtrl;
        }

        private MapCtrl SetupMapCtrlWithEmptyTree()
        {
            MapTree tree = new MapTree();
            MapNode root = new MapNode(tree, "r");
            var form = new System.Windows.Forms.Form();
            MetaModel.MetaModel.Initialize();
            MetaModel.MetaModel.Instance.MapEditorBackColor = Color.White;
            MetaModel.MetaModel.Instance.NoteEditorBackColor = Color.White;
            MapCtrl mapCtrl = new MapCtrl(new MapView(tree), new MainCtrlStub(form));
            form.Controls.Add(mapCtrl.MapView.Canvas);

            tree.TurnOnChangeManager();

            return mapCtrl;
        }

        [TestMethod()]
        public void MapCtrl_WithFeatureDisplay()
        {
            Assert.IsNotNull(SetupMapCtrlWithFeaureDisplay());
        }

        [TestMethod()]
        public void MapCtrl_WithEmptyTree()
        {
            Assert.IsNotNull(SetupMapCtrlWithEmptyTree());
        }

        [TestMethod()]
        public void SelectAllNodes()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            c3.Folded = true;
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");

            mapCtrl.SelectAllNodes(false);

            Assert.AreEqual(7, t.SelectedNodes.Count);
        }

        [TestMethod()]
        public void SelectAllNodes_Unfold()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            c3.Folded = true;
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");

            mapCtrl.SelectAllNodes(true);

            Assert.AreEqual(10, t.SelectedNodes.Count);
        }

        [TestMethod]
        public void SelectLevel_1UndoBatch()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            c1.Folded = true;
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            c3.Folded = true;
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");
            var undoStackCount = t.ChangeManager.UndoStackCount;

            mapCtrl.SelectLevel(2, false, true);

            Assert.AreEqual(undoStackCount + 1, t.ChangeManager.UndoStackCount);
        }

        [TestMethod]
        public void SelectLevel_CanvasLocationNoChange()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            c3.Folded = true;
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");
            var location = mapCtrl.MapView.Canvas.Location;

            mapCtrl.SelectLevel(2, false, true);

            Assert.AreEqual(location, mapCtrl.MapView.Canvas.Location);
        }


        [TestMethod()]
        public void SelectCurrentLevel_Level013()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");

            t.SelectedNodes.Add(r, true);
            t.SelectedNodes.Add(c2, true);
            t.SelectedNodes.Add(c311, true);

            mapCtrl.SelectCurrentLevel(false);

            Assert.AreEqual(5, t.SelectedNodes.Count);
        }

        [TestMethod()]
        public void SelectCurrentLevel_Level013_Unfold()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");
            var c321 = new MapNode(c32, "c32");

            c32.Folded = true;

            t.SelectedNodes.Add(r, true);
            t.SelectedNodes.Add(c2, true);
            t.SelectedNodes.Add(c311, true);

            mapCtrl.SelectCurrentLevel(true);

            Assert.AreEqual(6, t.SelectedNodes.Count);
        }

        [TestMethod()]
        public void SelectCurrentLevel_Level013_SkipFolded()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");
            var c321 = new MapNode(c32, "c32");

            c32.Folded = true;

            t.SelectedNodes.Add(r, true);
            t.SelectedNodes.Add(c2, true);
            t.SelectedNodes.Add(c311, true);

            mapCtrl.SelectCurrentLevel(false);

            Assert.AreEqual(5, t.SelectedNodes.Count);
        }

        [TestMethod()]
        public void SelectCurrentLevel_Level023()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");

            t.SelectedNodes.Add(r, true);
            t.SelectedNodes.Add(c31, true);
            t.SelectedNodes.Add(c311, true);

            mapCtrl.SelectCurrentLevel(false);

            Assert.AreEqual(7, t.SelectedNodes.Count);
        }

        [TestMethod()]
        public void SelectCurrentLevel_Level023WithFolded()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            c1.Folded = true;
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");

            t.SelectedNodes.Add(r, true);
            t.SelectedNodes.Add(c31, true);
            t.SelectedNodes.Add(c311, true);

            mapCtrl.SelectCurrentLevel(false);

            Assert.AreEqual(4, t.SelectedNodes.Count);
        }

        [TestMethod()]
        public void SelectCurrentLevel_Level023WithFolded_Unfold()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            c1.Folded = true;
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");

            t.SelectedNodes.Add(r, true);
            t.SelectedNodes.Add(c31, true);
            t.SelectedNodes.Add(c311, true);

            mapCtrl.SelectCurrentLevel(true);

            Assert.AreEqual(7, t.SelectedNodes.Count);
        }

        [TestMethod()]
        public void SelectSiblings()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");

            c12.Selected = true;

            mapCtrl.SelectSiblings();

            Assert.AreEqual(3, t.SelectedNodes.Count);
        }

        [TestMethod()]
        public void SelectAncestors()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");

            c13.Selected = true;
            t.SelectedNodes.Add(c311, true);

            mapCtrl.SelectAncestors();

            Assert.AreEqual(6, t.SelectedNodes.Count);
        }

        [TestMethod()]
        public void SelectChildren()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");

            c1.Selected = true;

            mapCtrl.SelectChildren(false);

            Assert.AreEqual(3, t.SelectedNodes.Count);
        }

        [TestMethod()]
        public void SelectChildren_WithExpandSelection()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");

            c1.Selected = true;

            mapCtrl.SelectChildren(true);

            Assert.AreEqual(4, t.SelectedNodes.Count);
        }

        [TestMethod()]
        public void SelectDescendents()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");

            c3.Selected = true;

            mapCtrl.SelectDescendents(false);

            Assert.AreEqual(4, t.SelectedNodes.Count);
        }

        [TestMethod()]
        public void SelectDescendents_WithFolded_DonotSelectFolded()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");

            c3.Selected = true;
            c31.Folded = true;

            mapCtrl.SelectDescendents(false);

            Assert.AreEqual(3, t.SelectedNodes.Count);
        }

        [TestMethod()]
        public void SelectDescendents_WithFolded_Unfold()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");

            c3.Selected = true;
            c31.Folded = true;

            mapCtrl.SelectDescendents(true);

            Assert.AreEqual(4, t.SelectedNodes.Count);
        }

        [TestMethod()]
        public void SelectDescendents_Depth1()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");

            c3.Selected = true;

            mapCtrl.SelectDescendents(1, false);

            Assert.AreEqual(3, t.SelectedNodes.Count);
        }

        [TestMethod()]
        public void SelectDescendents_Depth2WithFolded()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");

            c3.Selected = true;
            c31.Folded = true;

            mapCtrl.SelectDescendents(2, false);

            Assert.AreEqual(3, t.SelectedNodes.Count);
        }

        [TestMethod()]
        public void SelectDescendents_Depth2WithFolded_Unfold()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");

            c3.Selected = true;
            c31.Folded = true;

            mapCtrl.SelectDescendents(2, true);

            Assert.AreEqual(4, t.SelectedNodes.Count);
        }

        [TestMethod()]
        public void FoldAll()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");

            mapCtrl.FoldAll();

            Assert.IsTrue(c3.Folded);
        }

        [TestMethod()]
        public void UnfoldAll()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");
            mapCtrl.FoldAll();

            mapCtrl.UnfoldAll();

            Assert.IsFalse(c3.Folded);
        }

        [TestMethod()]
        public void ToggleFolded()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");
            c3.Folded = true;
            c3.Selected = true;
            t.SelectedNodes.Add(c1, true);

            mapCtrl.ToggleFolded();

            Assert.IsFalse(c3.Folded);
            Assert.IsTrue(c1.Folded);
        }

        [TestMethod()]
        public void ToggleBranchFolding()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");
            c3.Folded = true;
            c31.Folded = true;
            c3.Selected = true;

            mapCtrl.ToggleBranchFolding();

            Assert.IsFalse(c3.Folded);
            Assert.IsFalse(c31.Folded);
        }

        [TestMethod()]
        public void UnfoldMapToCurrentLevel()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");
            c1.Folded = true;
            c31.Folded = true;
            c31.Selected = true;

            mapCtrl.UnfoldMapToCurrentLevel();

            Assert.IsTrue(c31.Folded);
            Assert.IsFalse(c1.Folded);
            Assert.IsTrue(c13.Folded);
        }

        [TestMethod()]
        public void UnfoldMapToCurrentLevel_Root()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");
            c1.Folded = true;
            c31.Folded = true;
            r.Selected = true;

            mapCtrl.UnfoldMapToCurrentLevel();

            Assert.IsFalse(r.Folded);
        }

        [TestMethod()]
        public void UnfoldMapToCurrentLevel_NoNodeSelected()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");
            c1.Folded = true;
            c31.Folded = true;

            mapCtrl.UnfoldMapToCurrentLevel();

            Assert.IsFalse(r.Folded);
        }

        [TestMethod()]
        public void UnfoldMapToLevel_Level1()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c32 = new MapNode(c3, "c32");
            c1.Folded = true;
            c31.Folded = true;
            c31.Selected = true;

            mapCtrl.UnfoldMapToLevel(1);

            Assert.IsTrue(c3.Folded);
            Assert.IsTrue(c1.Folded);
        }

        [TestMethod()]
        public void UnfoldMapToLevel_Level3()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            c1.Folded = true;
            c31.Folded = true;
            c31.Selected = true;

            mapCtrl.UnfoldMapToLevel(3);

            Assert.IsFalse(c3.Folded);
            Assert.IsFalse(c1.Folded);
            Assert.IsTrue(c311.Folded);
        }

        [TestMethod()]
        public void UnfoldMapToLevel_Level3_NoSelectedNode()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            c1.Folded = true;
            c31.Folded = true;

            mapCtrl.UnfoldMapToLevel(3);

            Assert.IsFalse(c3.Folded);
            Assert.IsFalse(c1.Folded);
            Assert.IsTrue(c311.Folded);
        }

        [TestMethod()]
        public void NavigateToCenter_EndNodeEditing()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            c1.Folded = true;
            c31.Folded = true;
            c3111.Selected = true;
            mapCtrl.BeginCurrentNodeEdit();

            mapCtrl.SelectRootNode();

            Assert.IsFalse(mapCtrl.MapView.NodeTextEditor.IsTextEditing);
        }

        [TestMethod()]
        public void NavigateToCenter_SelectRoot()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            c1.Folded = true;
            c31.Folded = true;

            mapCtrl.SelectRootNode();

            Assert.IsTrue(r.Selected);
        }

        [TestMethod()]
        public void SelectTopSibling()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            c1.Folded = true;
            c31.Folded = true;
            c3.Selected = true;

            mapCtrl.SelectTopSibling();

            Assert.IsTrue(c1.Selected);
            Assert.IsFalse(c3.Selected);
        }

        [TestMethod()]
        public void SelectBottomSibling()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            c1.Folded = true;
            c31.Folded = true;
            c2.Selected = true;

            mapCtrl.SelectBottomSibling();

            Assert.AreEqual(1, t.SelectedNodes.Count);
            Assert.IsTrue(c3.Selected);
        }

        [TestMethod()]
        public void SelectBottomSibling_WithRootSelected_NoChange()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            c1.Folded = true;
            c31.Folded = true;
            r.Selected = true;

            mapCtrl.SelectBottomSibling();

            Assert.AreEqual(1, t.SelectedNodes.Count);
            Assert.IsTrue(r.Selected);
        }

        [TestMethod()]
        public void MoveNodeUp_NoSelection_NoChange()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");

            mapCtrl.MoveNodeUp();
        }

        [TestMethod()]
        public void MoveNodeUp()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            c3.Selected = true;

            mapCtrl.MoveNodeUp();

            Assert.AreEqual(c3, c2.Previous);
        }

        [TestMethod()]
        public void MoveNodeDown()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            c2.Selected = true;

            mapCtrl.MoveNodeDown();

            Assert.AreEqual(c2, c3.Next);
        }

        [TestMethod()]
        public void SortAlphabeticallyAsc()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "C");
            var c11 = new MapNode(c1, "6");
            var c12 = new MapNode(c1, "2");
            var c13 = new MapNode(c1, "4");
            var c14 = new MapNode(c1, "7");
            var c15 = new MapNode(c1, "1");
            var c16 = new MapNode(c1, "5");
            var c17 = new MapNode(c1, "3");
            var c121 = new MapNode(c12, "c121");
            var c2 = new MapNode(r, "B");
            var c3 = new MapNode(r, "C", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c32 = new MapNode(c3, "c32");
            c1.Selected = true;

            mapCtrl.SortAlphabeticallyAsc();

            Assert.AreEqual(c15, c1.FirstChild);
            Assert.AreEqual(c12, c1.FirstChild.Next);
            Assert.AreEqual(c11, c1.LastChild.Previous);
            Assert.AreEqual(c14, c1.LastChild);
        }

        [TestMethod()]
        public void SortAlphabeticallyDesc()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "C");
            var c11 = new MapNode(c1, "6");
            var c12 = new MapNode(c1, "2");
            var c13 = new MapNode(c1, "4");
            var c14 = new MapNode(c1, "7");
            var c15 = new MapNode(c1, "1");
            var c16 = new MapNode(c1, "5");
            var c17 = new MapNode(c1, "3");
            var c121 = new MapNode(c12, "c121");
            var c2 = new MapNode(r, "B");
            var c3 = new MapNode(r, "C", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c32 = new MapNode(c3, "c32");
            c1.Selected = true;

            mapCtrl.SortAlphabeticallyDesc();

            Assert.AreEqual(c14, c1.FirstChild);
            Assert.AreEqual(c11, c1.FirstChild.Next);
            Assert.AreEqual(c12, c1.LastChild.Previous);
            Assert.AreEqual(c15, c1.LastChild);
        }

        [TestMethod()]
        public void SortByTaskAsc()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "C");
            var c11 = new MapNode(c1, "6"); c11.AddTask(DateTime.Now);
            var c12 = new MapNode(c1, "2");
            var c13 = new MapNode(c1, "4");
            var c14 = new MapNode(c1, "7"); c14.AddTask(DateTime.Now.AddSeconds(5));
            var c15 = new MapNode(c1, "1");
            var c16 = new MapNode(c1, "5");
            var c17 = new MapNode(c1, "3");
            c1.Selected = true;

            mapCtrl.SortByTaskAsc();

            Assert.AreEqual(c12, c1.FirstChild);
            Assert.AreEqual(c11, c1.LastChild.Previous);
            Assert.AreEqual(c14, c1.LastChild);
        }

        [TestMethod()]
        public void SortByTaskDesc()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "C");
            var c11 = new MapNode(c1, "6"); c11.AddTask(DateTime.Now);
            var c12 = new MapNode(c1, "2");
            var c13 = new MapNode(c1, "4");
            var c14 = new MapNode(c1, "7"); c14.AddTask(DateTime.Now.AddSeconds(5));
            var c15 = new MapNode(c1, "1");
            var c16 = new MapNode(c1, "5");
            var c17 = new MapNode(c1, "3");
            c1.Selected = true;

            mapCtrl.SortByTaskDesc();

            Assert.AreEqual(c17, c1.LastChild);
            Assert.AreEqual(c11, c1.FirstChild.Next);
            Assert.AreEqual(c14, c1.FirstChild);
        }

        [TestMethod()]
        public void SortByDescendentsCountAsc()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "C");
            var c11 = new MapNode(c1, "6");
            var c12 = new MapNode(c1, "2");
            var c13 = new MapNode(c1, "4");
            var c14 = new MapNode(c1, "7");
            var c15 = new MapNode(c1, "1");
            var c16 = new MapNode(c1, "5");
            var c17 = new MapNode(c1, "3");
            var c121 = new MapNode(c12, "c121");
            var c2 = new MapNode(r, "B");
            var c3 = new MapNode(r, "C", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c32 = new MapNode(c3, "c32");
            r.Selected = true;

            mapCtrl.SortByDescendentsCountAsc();

            Assert.AreEqual(c2, r.FirstChild);
            Assert.AreEqual(c1, r.LastChild);
        }

        [TestMethod()]
        public void SortByDescendentsCountDesc()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "C");
            var c11 = new MapNode(c1, "6");
            var c12 = new MapNode(c1, "2");
            var c13 = new MapNode(c1, "4");
            var c14 = new MapNode(c1, "7");
            var c15 = new MapNode(c1, "1");
            var c16 = new MapNode(c1, "5");
            var c17 = new MapNode(c1, "3");
            var c121 = new MapNode(c12, "c121");
            var c2 = new MapNode(r, "B");
            var c3 = new MapNode(r, "C", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c32 = new MapNode(c3, "c32");
            r.Selected = true;

            mapCtrl.SortByDescendentsCountDesc();

            Assert.AreEqual(c2, r.LastChild);
            Assert.AreEqual(c1, r.FirstChild);
        }

        [TestMethod()]
        public void SortByCreateDateCountAsc()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "C");
            var c11 = new MapNode(c1, "6");
            var c12 = new MapNode(c1, "2");
            var c13 = new MapNode(c1, "4");
            var c14 = new MapNode(c1, "7");
            var c15 = new MapNode(c1, "1");
            var c16 = new MapNode(c1, "5");
            var c17 = new MapNode(c1, "3");
            var c121 = new MapNode(c12, "c121");
            var c2 = new MapNode(r, "B");
            c2.Created = DateTime.Now.AddSeconds(5);
            var c3 = new MapNode(r, "C", NodePosition.Left);
            c3.Created = DateTime.Now.AddSeconds(10);
            var c31 = new MapNode(c3, "c31");
            var c32 = new MapNode(c3, "c32");
            r.Selected = true;

            mapCtrl.SortByCreateDateAsc();

            Assert.AreEqual(c3, r.LastChild);
            Assert.AreEqual(c1, r.FirstChild);
        }

        [TestMethod()]
        public void SortByCreateDateCountDesc()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "C");
            c1.Created = DateTime.Now;
            var c11 = new MapNode(c1, "6");
            var c12 = new MapNode(c1, "2");
            var c13 = new MapNode(c1, "4");
            var c14 = new MapNode(c1, "7");
            var c15 = new MapNode(c1, "1");
            var c16 = new MapNode(c1, "5");
            var c17 = new MapNode(c1, "3");
            var c121 = new MapNode(c12, "c121");
            var c2 = new MapNode(r, "B");
            c2.Created = DateTime.Now.AddSeconds(5);
            var c3 = new MapNode(r, "C", NodePosition.Left);
            c3.Created = DateTime.Now.AddSeconds(10);
            var c31 = new MapNode(c3, "c31");
            var c32 = new MapNode(c3, "c32");
            r.Selected = true;

            mapCtrl.SortByCreateDateDesc();

            Assert.AreEqual(c1, r.LastChild);
            Assert.AreEqual(c3, r.FirstChild);
        }

        [TestMethod()]
        public void SortByModifiedDateCountAsc()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "6");
            var c12 = new MapNode(c1, "2");
            var c13 = new MapNode(c1, "4");
            var c14 = new MapNode(c1, "7");
            var c15 = new MapNode(c1, "1");
            var c16 = new MapNode(c1, "5");
            var c17 = new MapNode(c1, "3");
            var c121 = new MapNode(c12, "c121");
            var c2 = new MapNode(r, "c2");
            c2.Modified = DateTime.Now.AddSeconds(5);
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c32 = new MapNode(c3, "c32");
            c3.Modified = DateTime.Now.AddSeconds(10);
            r.Selected = true;

            mapCtrl.SortByModifiedDateAsc();

            Assert.AreEqual(c3, r.LastChild);
            Assert.AreEqual(c1, r.FirstChild);
        }

        [TestMethod()]
        public void SortByModifiedDateCountDesc()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            c1.Modified = DateTime.Now;
            var c11 = new MapNode(c1, "6");
            var c12 = new MapNode(c1, "2");
            var c13 = new MapNode(c1, "4");
            var c14 = new MapNode(c1, "7");
            var c15 = new MapNode(c1, "1");
            var c16 = new MapNode(c1, "5");
            var c17 = new MapNode(c1, "3");
            var c121 = new MapNode(c12, "c121");
            var c2 = new MapNode(r, "c2");
            c2.Modified = DateTime.Now.AddSeconds(5);
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c32 = new MapNode(c3, "c32");
            c3.Modified = DateTime.Now.AddSeconds(10);
            r.Selected = true;

            mapCtrl.SortByModifiedDateDesc();

            Assert.AreEqual(c1, r.LastChild);
            Assert.AreEqual(c3, r.FirstChild);
        }

        [TestMethod()]
        public void AddHyperlink()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "6");
            var c12 = new MapNode(c1, "2");
            var c13 = new MapNode(c1, "4");
            var c14 = new MapNode(c1, "7");
            var c15 = new MapNode(c1, "1");
            var c16 = new MapNode(c1, "5");
            var c17 = new MapNode(c1, "3");
            var c121 = new MapNode(c12, "c121");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c32 = new MapNode(c3, "c32");
            r.Selected = true;
            t.SelectedNodes.Add(c32, true);

            mapCtrl.AddHyperlink("abc");

            Assert.AreEqual("abc", r.Link);
            Assert.AreEqual("abc", c32.Link);
            Assert.IsNull(c31.Link);
        }

        [TestMethod()]
        public void ChangeHyperlink()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "6");
            var c12 = new MapNode(c1, "2");
            var c13 = new MapNode(c1, "4");
            var c14 = new MapNode(c1, "7");
            var c15 = new MapNode(c1, "1");
            var c16 = new MapNode(c1, "5");
            var c17 = new MapNode(c1, "3");
            var c121 = new MapNode(c12, "c121");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c32 = new MapNode(c3, "c32");
            r.Selected = true;
            t.SelectedNodes.Add(c32, true);

            mapCtrl.AddHyperlink("abc");
            mapCtrl.AddHyperlink("xyz");

            Assert.AreEqual("xyz", r.Link);
            Assert.AreEqual("xyz", c32.Link);
            Assert.IsNull(c31.Link);
        }

        [TestMethod()]
        public void RemoveHyperlink()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "6");
            var c12 = new MapNode(c1, "2");
            var c13 = new MapNode(c1, "4");
            var c14 = new MapNode(c1, "7");
            var c15 = new MapNode(c1, "1");
            var c16 = new MapNode(c1, "5");
            var c17 = new MapNode(c1, "3");
            var c121 = new MapNode(c12, "c121");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c32 = new MapNode(c3, "c32");
            r.Selected = true;
            t.SelectedNodes.Add(c32, true);
            mapCtrl.AddHyperlink("abc");

            mapCtrl.RemoveHyperlink();

            Assert.IsNull(r.Link);
            Assert.IsNull(c32.Link);
            Assert.IsNull(c31.Link);
        }
        
        [TestMethod()]
        public void ChangeNodeShapeFork()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            r.AddToSelection();

            mapCtrl.ChangeNodeShapeFork();

            Assert.AreEqual(NodeShape.Fork, r.Shape);
        }

        [TestMethod()]
        public void ChangeNodeShapeBubble()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            r.AddToSelection();

            mapCtrl.ChangeNodeShapeBubble();

            Assert.AreEqual(NodeShape.Bubble, r.Shape);
        }

        [TestMethod()]
        public void ChangeNodeShapeBox()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            c32.AddToSelection();

            mapCtrl.ChangeNodeShapeBox();

            Assert.AreEqual(NodeShape.Box, c32.Shape);
        }

        [TestMethod()]
        public void ChangeNodeShapeBullet()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            c32.AddToSelection();

            mapCtrl.ChangeNodeShapeBullet();

            Assert.AreEqual(NodeShape.Bullet, c32.Shape);
        }

        [TestMethod()]
        public void ChangeNodeShapeBullet_ChangeManagerUndoCount()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            c32.AddToSelection();
            c311.AddToSelection();
            var undoCount = t.ChangeManager.UndoStackCount;

            mapCtrl.ChangeNodeShapeBullet();

            Assert.AreEqual(undoCount + 1, t.ChangeManager.UndoStackCount);
        }

        [TestMethod()]
        public void ChangeNodeShapeBullet_NoSelection()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            var undoCount = t.ChangeManager.UndoStackCount;

            mapCtrl.ChangeNodeShapeBullet();

            Assert.AreEqual(undoCount, t.ChangeManager.UndoStackCount);
        }

        [TestMethod()]
        public void ClearNodeShape()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            c32.AddToSelection();
            c3111.AddToSelection();
            mapCtrl.ClearNodeShape();

            Assert.AreEqual(NodeShape.None, c32.Shape);
        }

        [TestMethod()]
        public void ChangeNodeShape()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            c32.AddToSelection();

            mapCtrl.ChangeNodeShape(NodeShape.Bullet);

            Assert.AreEqual(NodeShape.Bullet, c32.Shape);
        }

        [TestMethod()]
        public void ChangeNodeShape_StringParameter()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            c32.AddToSelection();

            mapCtrl.ChangeNodeShape("Bubble");

            Assert.AreEqual(NodeShape.Bubble, c32.Shape);
        }

        [TestMethod()]
        public void ChangeLineColor()
        {
            MapCtrl mapCtrl = SetupMapCtrlWithEmptyTree();
            var t = mapCtrl.MapView.Tree;
            var r = t.RootNode;
            var c1 = new MapNode(r, "c1");
            var c11 = new MapNode(c1, "c11");
            var c12 = new MapNode(c1, "c12");
            var c13 = new MapNode(c1, "c13");
            var c131 = new MapNode(c13, "c131");
            var c2 = new MapNode(r, "c2");
            var c3 = new MapNode(r, "c3", NodePosition.Left);
            var c31 = new MapNode(c3, "c31");
            var c311 = new MapNode(c31, "c311");
            var c3111 = new MapNode(c311, "c3111");
            var c32 = new MapNode(c3, "c32");
            c1.AddToSelection();
            c2.AddToSelection();

            mapCtrl.ChangeLineColor(Color.Brown);

            Assert.AreEqual(Color.Brown, c1.LineColor);
            Assert.AreEqual(Color.Brown, c2.LineColor);
            Assert.AreEqual(Color.Empty, c3.LineColor);
        }

        [TestMethod()]
        public void ChangeLineColorUsingPicker()
        {
            MapTree tree = new MapTree();
            MapNode r = new MapNode(tree, "r");
            var form = new System.Windows.Forms.Form();
            MetaModel.MetaModel.Initialize();
            var mainCtrl = A.Fake<IMainCtrl>();
            A.CallTo(() => mainCtrl.ShowColorPicker(Color.Empty)).WithAnyArguments().Returns(Color.Chocolate);
            MapCtrl mapCtrl = new MapCtrl(new MapView(tree), mainCtrl);
            form.Controls.Add(mapCtrl.MapView.Canvas);
            tree.TurnOnChangeManager();
            r.AddToSelection();

            mapCtrl.ChangeLineColorUsingPicker();

            Assert.AreEqual(Color.Chocolate, r.LineColor);

        }

        [TestMethod()]
        public void ChangeLinePattern()
        {
            MapTree tree = new MapTree();
            MapNode r = new MapNode(tree, "r");
            var form = new System.Windows.Forms.Form();
            MetaModel.MetaModel.Initialize();
            var mainCtrl = A.Fake<IMainCtrl>();
            MapCtrl mapCtrl = new MapCtrl(new MapView(tree), mainCtrl);
            form.Controls.Add(mapCtrl.MapView.Canvas);
            tree.TurnOnChangeManager();
            r.AddToSelection();

            mapCtrl.ChangeLinePattern(DashStyle.Dash);

            Assert.AreEqual(DashStyle.Dash, r.LinePattern);
        }

        [TestMethod()]
        public void ChangeLineWidth()
        {
            MapTree tree = new MapTree();
            MapNode r = new MapNode(tree, "r");
            var form = new System.Windows.Forms.Form();
            MetaModel.MetaModel.Initialize();
            var mainCtrl = A.Fake<IMainCtrl>();
            MapCtrl mapCtrl = new MapCtrl(new MapView(tree), mainCtrl);
            form.Controls.Add(mapCtrl.MapView.Canvas);
            tree.TurnOnChangeManager();
            r.AddToSelection();

            mapCtrl.ChangeLineWidth(4);

            Assert.AreEqual(4, r.LineWidth);
        }

        [TestMethod()]
        public void CreateNodeStyle_NodeStylesCountGoesUp()
        {
            MapTree tree = new MapTree();
            MapNode r = new MapNode(tree, "r");
            var form = new System.Windows.Forms.Form();
            MetaModel.MetaModel.Initialize();
            var mainCtrl = A.Fake<IMainCtrl>();
            A.CallTo(() => mainCtrl.ShowInputBox("Enter the style name:", null)).Returns(DateTime.Now.Ticks.ToString());
            MapCtrl mapCtrl = new MapCtrl(new MapView(tree), mainCtrl);
            form.Controls.Add(mapCtrl.MapView.Canvas);
            tree.TurnOnChangeManager();
            r.AddToSelection();
            int count = MetaModel.MetaModel.Instance.NodeStyles.Count;

            mapCtrl.CreateNodeStyle();

            Assert.AreEqual(count + 1, MetaModel.MetaModel.Instance.NodeStyles.Count);
        }

        [TestMethod()]
        public void CreateNodeStyle_NullIfNothingSelected()
        {
            MapTree tree = new MapTree();
            MapNode r = new MapNode(tree, "r");
            var form = new System.Windows.Forms.Form();
            MetaModel.MetaModel.Initialize();
            var mainCtrl = A.Fake<IMainCtrl>();
            MapCtrl mapCtrl = new MapCtrl(new MapView(tree), mainCtrl);
            form.Controls.Add(mapCtrl.MapView.Canvas);
            tree.TurnOnChangeManager();

            var style = mapCtrl.CreateNodeStyle();

            Assert.IsNull(style);
        }

        [TestMethod()]
        public void CreateNodeStyle_NullIfMultipleSelected()
        {
            MapTree tree = new MapTree();
            MapNode r = new MapNode(tree, "r");
            var c1 = new MapNode(r, "c1");
            var form = new System.Windows.Forms.Form();
            MetaModel.MetaModel.Initialize();
            var mainCtrl = A.Fake<IMainCtrl>();
            MapCtrl mapCtrl = new MapCtrl(new MapView(tree), mainCtrl);
            form.Controls.Add(mapCtrl.MapView.Canvas);
            tree.TurnOnChangeManager();
            r.AddToSelection();
            c1.AddToSelection();

            var style = mapCtrl.CreateNodeStyle();

            Assert.IsNull(style);
        }

        [TestMethod()]
        public void ApplyNodeStyle()
        {
            MapTree tree = new MapTree();
            MapNode r = new MapNode(tree, "r");
            r.FontSize = 15;
            var c1 = new MapNode(r, "c1");
            var c2 = new MapNode(r, "c2");
            var form = new System.Windows.Forms.Form();
            MetaModel.MetaModel.Initialize();
            var mainCtrl = A.Fake<IMainCtrl>();
            MapCtrl mapCtrl = new MapCtrl(new MapView(tree), mainCtrl);
            form.Controls.Add(mapCtrl.MapView.Canvas);
            tree.TurnOnChangeManager();
            r.AddToSelection();
            var style = mapCtrl.CreateNodeStyle();
            r.Selected = false;
            c1.AddToSelection();
            c2.AddToSelection();

            mapCtrl.ApplyNodeStyle(style);

            Assert.AreEqual(15, c1.FontSize);
            Assert.AreEqual(15, c2.FontSize);
        }

        [TestMethod()]
        public void ChangeBackColor()
        {
            MapTree tree = new MapTree();
            MapNode r = new MapNode(tree, "r");
            r.FontSize = 15;
            var c1 = new MapNode(r, "c1");
            var c2 = new MapNode(r, "c2");
            var form = new System.Windows.Forms.Form();
            MetaModel.MetaModel.Initialize();
            var mainCtrl = A.Fake<IMainCtrl>();
            MapCtrl mapCtrl = new MapCtrl(new MapView(tree), mainCtrl);
            form.Controls.Add(mapCtrl.MapView.Canvas);
            tree.TurnOnChangeManager();
            r.AddToSelection();

            mapCtrl.ChangeBackColor(Color.Aqua);

            c2.AddToSelection();

            Assert.AreEqual(c1.BackColor, Color.Empty);
            Assert.AreEqual(c2.BackColor, Color.Empty);
            Assert.AreEqual(r.BackColor, Color.Aqua);

        }

        [TestMethod()]
        public void ClearFormatting()
        {
            MapTree tree = new MapTree();
            MapNode r = new MapNode(tree, "r");
            r.FontSize = 15;
            var c1 = new MapNode(r, "c1");
            var c2 = new MapNode(r, "c2");
            var form = new System.Windows.Forms.Form();
            MetaModel.MetaModel.Initialize();
            var mainCtrl = A.Fake<IMainCtrl>();
            MapCtrl mapCtrl = new MapCtrl(new MapView(tree), mainCtrl);
            form.Controls.Add(mapCtrl.MapView.Canvas);
            tree.TurnOnChangeManager();
            r.AddToSelection();
            c2.AddToSelection();
            mapCtrl.ChangeBackColor(Color.Aqua);
            c2.Selected = false;

            mapCtrl.ClearFormatting();

            Assert.AreEqual(c1.BackColor, Color.Empty);
            Assert.AreEqual(c2.BackColor, Color.Aqua);
            Assert.AreEqual(r.BackColor, Color.Empty);
        }

        [TestMethod()]
        public void ChangeStrikeout_MultiSelect()
        {
            MapTree tree = new MapTree();
            MapNode r = new MapNode(tree, "r");
            r.FontSize = 15;
            var c1 = new MapNode(r, "c1");
            var c2 = new MapNode(r, "c2");
            var form = new System.Windows.Forms.Form();
            MetaModel.MetaModel.Initialize();
            var mainCtrl = A.Fake<IMainCtrl>();
            MapCtrl mapCtrl = new MapCtrl(new MapView(tree), mainCtrl);
            form.Controls.Add(mapCtrl.MapView.Canvas);
            tree.TurnOnChangeManager();
            r.AddToSelection();
            c2.AddToSelection();
            c2.Strikeout = true;
            mapCtrl.ChangeStrikeout(false);
            
            Assert.IsFalse(c2.Strikeout);
            Assert.IsFalse(r.Strikeout);
        }

        [TestMethod()]
        public void ToggleStrikeout()
        {
            MapTree tree = new MapTree();
            MapNode r = new MapNode(tree, "r");
            var c1 = new MapNode(r, "c1");
            var c2 = new MapNode(r, "c2");
            var form = new System.Windows.Forms.Form();
            MetaModel.MetaModel.Initialize();
            var mainCtrl = A.Fake<IMainCtrl>();
            MapCtrl mapCtrl = new MapCtrl(new MapView(tree), mainCtrl);
            form.Controls.Add(mapCtrl.MapView.Canvas);
            tree.TurnOnChangeManager();
            r.AddToSelection();
            c2.AddToSelection();
            c2.Strikeout = true;
            mapCtrl.ToggleStrikeout();

            Assert.IsFalse(c2.Strikeout);
            Assert.IsTrue(r.Strikeout);
        }


        [TestMethod()]
        public void ChangeBold()
        {
            MapTree tree = new MapTree();
            MapNode r = new MapNode(tree, "r");
            r.FontSize = 15;
            var c1 = new MapNode(r, "c1");
            var c2 = new MapNode(r, "c2");
            var form = new System.Windows.Forms.Form();
            MetaModel.MetaModel.Initialize();
            var mainCtrl = A.Fake<IMainCtrl>();
            MapCtrl mapCtrl = new MapCtrl(new MapView(tree), mainCtrl);
            form.Controls.Add(mapCtrl.MapView.Canvas);
            tree.TurnOnChangeManager();
            r.AddToSelection();
            c2.AddToSelection();
            c2.Bold = true;
            mapCtrl.ChangeBold(true);

            Assert.IsTrue(c2.Bold);
            Assert.IsTrue(r.Bold);
        }

        [TestMethod()]
        public void ToggleBold()
        {
            MapTree tree = new MapTree();
            MapNode r = new MapNode(tree, "r");
            var c1 = new MapNode(r, "c1");
            var c2 = new MapNode(r, "c2");
            var form = new System.Windows.Forms.Form();
            MetaModel.MetaModel.Initialize();
            var mainCtrl = A.Fake<IMainCtrl>();
            MapCtrl mapCtrl = new MapCtrl(new MapView(tree), mainCtrl);
            form.Controls.Add(mapCtrl.MapView.Canvas);
            tree.TurnOnChangeManager();
            r.AddToSelection();
            c2.AddToSelection();
            c2.Bold = true;
            mapCtrl.ToggleBold();

            Assert.IsFalse(c2.Bold);
            Assert.IsTrue(r.Bold);
        }

        [TestMethod()]
        public void ChangeItalic()
        {
            MapTree tree = new MapTree();
            MapNode r = new MapNode(tree, "r");
            var c1 = new MapNode(r, "c1");
            var c2 = new MapNode(r, "c2");
            var form = new System.Windows.Forms.Form();
            MetaModel.MetaModel.Initialize();
            var mainCtrl = A.Fake<IMainCtrl>();
            MapCtrl mapCtrl = new MapCtrl(new MapView(tree), mainCtrl);
            form.Controls.Add(mapCtrl.MapView.Canvas);
            tree.TurnOnChangeManager();
            r.AddToSelection();
            c2.AddToSelection();
            c2.Italic = true;
            mapCtrl.ChangeItalic(true);

            Assert.IsTrue(c2.Italic);
            Assert.IsTrue(r.Italic);
        }

        [TestMethod()]
        public void ToggleItalic()
        {
            MapTree tree = new MapTree();
            MapNode r = new MapNode(tree, "r");
            var c1 = new MapNode(r, "c1");
            var c2 = new MapNode(r, "c2");
            var form = new System.Windows.Forms.Form();
            MetaModel.MetaModel.Initialize();
            var mainCtrl = A.Fake<IMainCtrl>();
            MapCtrl mapCtrl = new MapCtrl(new MapView(tree), mainCtrl);
            form.Controls.Add(mapCtrl.MapView.Canvas);
            tree.TurnOnChangeManager();
            r.AddToSelection();
            c2.AddToSelection();
            c2.Italic = true;
            mapCtrl.ToggleItalic();

            Assert.IsFalse(c2.Italic);
            Assert.IsTrue(r.Italic);
        }

        //[TestMethod()]
        //public void EditHyperlink()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void BeginCurrentNodeEdit()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void BeginNodeEdit()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void MultiLineNodeEdit()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void MultiLineNodeEdit1()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void EndNodeEdit()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void UpdateNodeText()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void AppendNodeAndEdit()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void AppendChildNodeAndEdit()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void AppendChildNode()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void AppendMultiLineNodeAndEdit()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void AppendSiblingNodeAndEdit()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void AppendSiblingAboveAndEdit()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void InsertParentAndEdit()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void DeleteSelectedNodes()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void MoveNodeUp()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void MoveNodeDown()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void SelectAllSiblingsAbove()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void SelectAllSiblingsBelow()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void SelectNodeAbove()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void SelectNodeBelow()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void SelectNodeRightOrUnfold()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void SelectNodeLeftOrUnfold()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void ToggleFolded()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void ToggleFolded1()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void RemoveLastIcon()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void RemoveAllIcon()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void AppendIcon()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void AppendIconFromIconSelectorExt()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void FollowLink()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void SetFontFamily()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void SetFontSize()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void ChangeTextColorByPicker()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void ChangeTextColor()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void ChangeBackColorByPicker()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void ChangeFont()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void Copy()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void Paste()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void Cut()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void MoveNodes()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void SetMapViewBackColor()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void CopyFormat()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void EnableFormatMultiApply()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void PasteFormat()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void ClearFormatPainter()
        //{
        //    Assert.Fail();
        //}
    }
}
