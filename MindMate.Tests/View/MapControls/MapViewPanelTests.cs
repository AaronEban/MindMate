﻿using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MindMate.Model;
using MindMate.Serialization;
using MindMate.View.MapControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MindMate.Tests.View.MapControls
{
    [TestClass()]
    public class MapViewPanelTests
    {
        private MapView view = null;
        private MapNode nodeEntered, nodeExit, nodeClicked, nodeRightClicked = null;
        private bool canvasClicked = false;

        [TestMethod()]
        public void MapViewPanel()
        {
            MapTree tree = new MapTree();
            tree.TurnOnChangeManager();
            var r = new MapNode(tree, "Root");
            var c1 = new MapNode(r, "c1");
            var c2 = new MapNode(r, "c2");
            
            MindMate.MetaModel.MetaModel.Initialize();
            view = new MapView(tree);
            var form = new Form();
            form.Controls.Add(view.Canvas);

            view.Canvas.NodeMouseEnter += (o, e) => nodeEntered = o;
            view.Canvas.NodeMouseExit += (o, e) => nodeExit = o;
            view.Canvas.NodeClick += (o, e) => nodeClicked = o;
            view.Canvas.NodeRightClick += (o, e) => nodeRightClicked = o;
            view.Canvas.CanvasClick += (e) => canvasClicked = true;

            //mouse move on canvas
            FireMouseMove(0, 0);
            ValidateAndReset();

            //mouse move on root
            FireMouseMove((int)r.NodeView.Left + 1, (int)r.NodeView.Top + 1);
            ValidateAndReset(entered: r);    

            //mouse move over canvas
            FireMouseMove(0, 0);
            ValidateAndReset(exit: r);

            //mouse move on root
            FireMouseMove((int)r.NodeView.Left + 1, (int)r.NodeView.Top + 1);
            ValidateAndReset(entered: r);

            //mouse move on c1
            FireMouseMove((int)c1.NodeView.Left + 1, (int)c1.NodeView.Top + 1);
            ValidateAndReset(entered: c1, exit: r);

            //mouse move on c1
            FireMouseMove((int)c1.NodeView.Right - 1, (int)c1.NodeView.Bottom - 1);
            ValidateAndReset();

            //mouse down on c1
            FireMouseDown((int)c1.NodeView.Right - 10, (int)c1.NodeView.Bottom - 5);
            ValidateAndReset();

            //click on c1
            FireMouseUp((int)c1.NodeView.Right - 10, (int)c1.NodeView.Bottom - 5);
            ValidateAndReset(clicked: c1);

            //click on canvas
            FireMouseUp(0, 0);
            ValidateAndReset(canvasClicked: true);

            //right click on c1
            FireMouseUp((int)c1.NodeView.Right - 10, (int)c1.NodeView.Bottom - 5, MouseButtons.Right);
            ValidateAndReset(nodeRightClicked: c1);

            //mouse move on c1
            FireMouseMove((int)c1.NodeView.Right - 1, (int)c1.NodeView.Bottom - 1);
            ValidateAndReset();

            FireMouseHover();

            //mouse move on c1
            FireMouseMove((int)c1.NodeView.Right - 1, (int)c1.NodeView.Bottom - 1);
            ValidateAndReset(entered: c1);

            //click with FormatPainter active
            view.FormatPainter.Copy(r);
            FireMouseUp((int)c1.NodeView.Right - 10, (int)c1.NodeView.Bottom - 5);
            ValidateAndReset(); // no click because format painter is active

            //mouse move on canvas
            FireMouseMove(0, 0);
            ValidateAndReset();

            //drag c1
            FireMouseMove((int)c1.NodeView.Right - 10, (int)c1.NodeView.Bottom - 5, MouseButtons.Right);
            ValidateAndReset();

            //drop c1 on c2
            FireMouseUp((int)c2.NodeView.Right - 1, (int)c2.NodeView.Bottom - 1, MouseButtons.Right);
            ValidateAndReset(); //no click because it's drop

            FirePreviewKeyDown();

            view.Canvas.Dispose();
        }      
        
        private void ValidateAndReset(MapNode entered = null, MapNode exit = null, MapNode clicked = null, MapNode nodeRightClicked = null, bool canvasClicked = false)
        {
            Assert.AreEqual(entered, nodeEntered);
            Assert.AreEqual(exit, nodeExit);
            Assert.AreEqual(clicked, nodeClicked);
            Assert.AreEqual(nodeRightClicked, this.nodeRightClicked);
            Assert.AreEqual(canvasClicked, this.canvasClicked);            

            this.nodeEntered = null;
            this.nodeExit = null;
            this.nodeClicked = null;
            this.canvasClicked = false;
            this.nodeRightClicked = null;
        }

        private void FireMouseMove(int x, int y, MouseButtons mouseButtons = MouseButtons.None)
        {
            view.Canvas.GetType().GetMethod("OnMouseMove", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(view.Canvas, new object[] {
                    new MouseEventArgs(mouseButtons, 0, x, y, 0) });
        }

        private void FireMouseDown(int x, int y, MouseButtons mouseButtons = MouseButtons.None)
        {
            view.Canvas.GetType().GetMethod("OnMouseDown", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(view.Canvas, new object[] {
                    new MouseEventArgs(mouseButtons, 0, x, y, 0) });
        }

        private void FireMouseUp(int x, int y, MouseButtons mouseButtons = MouseButtons.None)
        {
            view.Canvas.GetType().GetMethod("OnMouseUp", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(view.Canvas, new object[] {
                    new MouseEventArgs(mouseButtons, 0, x, y, 0) });
        }

        private void FireMouseHover()
        {
            view.Canvas.GetType().GetMethod("OnMouseHover", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(view.Canvas, new object[] {
                    new EventArgs() });
        }

        private void FirePreviewKeyDown()
        {
            view.Canvas.GetType().GetMethod("OnPreviewKeyDown", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(view.Canvas, new object[] {
                    new PreviewKeyDownEventArgs(Keys.Right) });
        }

        
    }
}