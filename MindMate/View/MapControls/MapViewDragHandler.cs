﻿using MindMate.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MindMate.View.MapControls
{
    public class MapViewDragHandler
    {
        private Object dragObject;
        private Point dragStartPoint;

        private MapView MapView { get; set; }

        public delegate void NodeDragStartDelegate(MapNode node, NodeMouseEventArgs e);
        public event NodeDragStartDelegate NodeDragStart;
        public delegate void NodeDragDropDelegate(MapTree tree, DropLocation location);
        public event NodeDragDropDelegate NodeDragDrop;        

        internal MapViewDragHandler(MapView mapView)
        {
            MapView = mapView;
        }

        internal void OnMouseDrag(MouseEventArgs e)
        {
            if (!IsDragging)
            {
                DragStart(e);
            }
            else if (IsCanvasDragging)
            {
                MoveCanvas(e);
            }
            else if(IsNodeDragging)
            {

            }
        }        

        internal void OnMouseDrop(MouseEventArgs e)
        {
            if (IsNodeDragging)
            {
                DropLocation dropLocation = CalculateDropLocation(e.Location);
                if (IsValidDropLocation(dropLocation) && NodeDragDrop != null)
                {
                    NodeDragDrop(MapView.Tree, dropLocation);
                }
            }           

            dragObject = null;
            MapView.Canvas.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool IsDragging
        {
            get { return this.dragObject != null; }
        }

        public bool IsCanvasDragging
        {
            get { return dragObject == MapView.Canvas; }
        }

        public bool IsNodeDragging
        {
            get { return dragObject != null && dragObject != MapView.Canvas;  }
        }

        #region Private Methods

        private void DragStart(MouseEventArgs e)
        {
            MapNode node = MapView.GetMapNodeFromPoint(e.Location);
            if (node == null)
            {
                this.dragObject = MapView.Canvas;
                this.dragStartPoint = e.Location;                
            }
            else
            {
                this.dragObject = node;
                if(NodeDragStart != null) { NodeDragStart(node, new NodeMouseEventArgs(e)); }
            }
        }

        private void MoveCanvas(MouseEventArgs e)
        {
            MapView.Canvas.SuspendLayout();
            MapView.Canvas.Top = MapView.Canvas.Top + (e.Y - this.dragStartPoint.Y);
            MapView.Canvas.Left = MapView.Canvas.Left + (e.X - this.dragStartPoint.X);
            MapView.Canvas.ResumeLayout();

            MapView.Canvas.Cursor = Cursors.SizeAll;
            //new Cursor(new System.IO.MemoryStream(MindMate.Properties.Resources.move_r));
        }

        private DropLocation CalculateDropLocation(Point p)
        {
            MapNode node = MapView.GetMapNodeFromPoint(p);

            if (node != null)
            {
                return new DropLocation() { Parent = node, insertAfterSibling = true };
            }
            else
            {
                return new DropLocation();
            }
        }

        private bool IsValidDropLocation(DropLocation location)
        {
            if(location.IsEmpty) { return false; }

            foreach(MapNode n in MapView.Tree.SelectedNodes)
            {
                if(n == location.Parent) { return false; } //drop location is included in moved nodes

                if (n.Parent == location.Parent)
                {
                    if(n.Next != null && n.Next == location.Sibling && !location.insertAfterSibling) //same location as present
                    { return false; }
                    if(n.Previous != null && n.Previous == location.Sibling && location.insertAfterSibling) //same location as present
                    { return false; }                    
                }

                if (n.Pos == NodePosition.Root) { return false; } //can't move root

                if (location.Parent.isDescendent(n)) { return false; } //can't move ancentor to child
            }                
            
            return true;
        }

        #endregion Private Methods

    }

    public struct DropLocation
    {
        public MapNode Parent;
        public MapNode Sibling;
        public bool insertAfterSibling;

        public bool IsEmpty
        {
            get { return Parent == null; }
        }
    }
}
