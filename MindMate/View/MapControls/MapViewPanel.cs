﻿/* Author: Syed Umar Anis (mail@umaranis.com)                    
 * Copyright (c) 2014 Syed Umar Anis                             
 * This software is licensed under MIT (see LICENSE.txt)    
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MindMate.View.MapControls;
using MindMate.Model;

namespace MindMate.View.MapControls
{
    /// <summary>
    /// The drawing canvas for MapView.
    /// Manages and fires Node related events.
    /// </summary>
    public partial class MapViewPanel : Control
    {

        public delegate void NodeClickDelegate(MapNode node, NodeMouseEventArgs args);
        public event NodeClickDelegate NodeClick = delegate { };

        public event Action<MapNode, NodeMouseEventArgs> NodeRightClick = delegate { };

        public event Action<MouseEventArgs> CanvasClick = delegate { };

        public delegate void NodeMouseOverDelegate(MapNode node, NodeMouseEventArgs args);
        public event NodeMouseOverDelegate NodeMouseOver = delegate { };

        public event Action<MapNode, MouseEventArgs> NodeMouseEnter = delegate { };
        public event Action<MapNode, MouseEventArgs> NodeMouseExit = delegate { };

        /// <summary>
        /// Node where mouse lies right now.
        /// </summary>
        private MapNode mouseOverNode;
        private Object dragObject;
        private Point dragStartPoint;
        
        public MapViewPanel()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

        }

                
        protected override void OnPaint(PaintEventArgs pe)
        {
            
            if (mapView != null && mapView.Tree != null)
            {
                MapControls.Drawing.MapPainter.DrawTree(mapView, pe.Graphics);
                MapControls.Drawing.MapPainter.drawNodeLinker(mapView.Tree.RootNode, mapView, pe.Graphics);                
            }
            ////base.OnPaint(pe);            
        }

        private MapView mapView;
        public MapView MapView
        {
            get
            {
                return  mapView;
            }
            set
            {
                mapView = value;
            }
        }

                
        private void NodeLinksPanel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                case Keys.Tab:
                    e.IsInputKey = true;
                    break;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            MapNode node = mapView.GetMapNodeFromPoint(e.Location);
            if (node != null)
            {
                NodeMouseEventArgs args = new NodeMouseEventArgs(e);
                args.NodePortion = mapView.GetNodeView(node).GetNodeClickPortion(e.Location);
                NodeClick(node, args);
            }
            else
            {
                this.dragObject = this;
                this.dragStartPoint = e.Location;
                if (mapView.NodeTextEditor.IsTextEditing)
                {
                    mapView.NodeTextEditor.EndNodeEdit(true, true);
                }
                else
                {
                    mapView.Canvas.Focus();
                }
                
            }

            //base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.dragObject != null && !mapView.NodeTextEditor.IsTextEditing)
            {
                mapView.Canvas.SuspendLayout();
                mapView.Canvas.Top = mapView.Canvas.Top + (e.Y - this.dragStartPoint.Y);
                mapView.Canvas.Left = mapView.Canvas.Left + (e.X - this.dragStartPoint.X);
                mapView.Canvas.ResumeLayout();

                mapView.Canvas.Cursor = Cursors.SizeAll;
                    //new Cursor(new System.IO.MemoryStream(MindMate.Properties.Resources.move_r));
            }
            else
            {
                MapNode node = mapView.GetMapNodeFromPoint(e.Location);
                                
                if (node != null)
                {
                    if (node != mouseOverNode)
                    {
                        if(mouseOverNode != null)
                        {
                            NodeMouseExit(mouseOverNode, e);
                        }
                        mouseOverNode = node;
                        NodeMouseEnter(node, e);
                    }

                    //NodeMouseEventArgs args = new NodeMouseEventArgs(e);
                    //args.NodePortion = mapView.GetNodeView(node).GetNodeClickPortion(e.Location);
                    //NodeMouseOver(node, args);
                }
                else if(mouseOverNode != null)
                {
                    NodeMouseExit(mouseOverNode, e);
                    mouseOverNode = null;
                }

                if (resetHoverEvent)
                {
                    const uint HOVER_DEFAULT = 0xFFFFFFFF;
                    TRACKMOUSEEVENT trackMouseEvent = new TRACKMOUSEEVENT(TMEFlags.TME_HOVER, this.Handle, HOVER_DEFAULT);
                    TrackMouseEvent(ref trackMouseEvent);
                    resetHoverEvent = false;
                }
            }             

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (mouseOverNode == null && this.Cursor == Cursors.Default) // IF 'event is not over node' AND 'canvas is not dragged'
                CanvasClick(e);

            this.dragObject = null;
            this.Cursor = Cursors.Default;

            if (e.Button == System.Windows.Forms.MouseButtons.Right && mouseOverNode != null)
                NodeRightClick(mouseOverNode, new NodeMouseEventArgs(e));
              
          
            //base.OnMouseUp(e);               
            
        }


        #region Mouse Hover Event

        private bool resetHoverEvent = false;

        protected override void OnMouseHover(EventArgs e)
        {
            resetHoverEvent = true;

            Point clickPosition = this.PointToClient(Cursor.Position);
            MapNode node = mapView.GetMapNodeFromPoint(clickPosition);
            if(node != null)
            {
                NodeMouseEventArgs args = new NodeMouseEventArgs(
                    new MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, clickPosition.X, clickPosition.Y, 0));
                NodeMouseOver(mouseOverNode, args);
            }

            base.OnMouseHover(e);                      
        }


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct TRACKMOUSEEVENT
        {
            public Int32 cbSize;    // using Int32 instead of UInt32 is safe here, and this avoids casting the result  of Marshal.SizeOf()
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public TMEFlags dwFlags;
            public IntPtr hWnd;
            public UInt32 dwHoverTime;

            public TRACKMOUSEEVENT(TMEFlags dwFlags, IntPtr hWnd, UInt32 dwHoverTime)
            {
                this.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TRACKMOUSEEVENT));
                this.dwFlags = dwFlags;
                this.hWnd = hWnd;
                this.dwHoverTime = dwHoverTime;
            }
        }

        /// <summary>
        /// The services requested. This member can be a combination of the following values.
        /// </summary>
        /// <seealso cref="http://msdn.microsoft.com/en-us/library/ms645604%28v=vs.85%29.aspx"/>
        [Flags]
        public enum TMEFlags : uint
        {
            /// <summary>
            /// The caller wants to cancel a prior tracking request. The caller should also specify the type of tracking that it wants to cancel. For example, to cancel hover tracking, the caller must pass the TME_CANCEL and TME_HOVER flags.
            /// </summary>
            TME_CANCEL = 0x80000000,
            /// <summary>
            /// The caller wants hover notification. Notification is delivered as a WM_MOUSEHOVER message.
            /// If the caller requests hover tracking while hover tracking is already active, the hover timer will be reset.
            /// This flag is ignored if the mouse pointer is not over the specified window or area.
            /// </summary>
            TME_HOVER = 0x00000001,
            /// <summary>
            /// The caller wants leave notification. Notification is delivered as a WM_MOUSELEAVE message. If the mouse is not over the specified window or area, a leave notification is generated immediately and no further tracking is performed.
            /// </summary>
            TME_LEAVE = 0x00000002,
            /// <summary>
            /// The caller wants hover and leave notification for the nonclient areas. Notification is delivered as WM_NCMOUSEHOVER and WM_NCMOUSELEAVE messages.
            /// </summary>
            TME_NONCLIENT = 0x00000010,
            /// <summary>
            /// The function fills in the structure instead of treating it as a tracking request. The structure is filled such that had that structure been passed to TrackMouseEvent, it would generate the current tracking. The only anomaly is that the hover time-out returned is always the actual time-out and not HOVER_DEFAULT, if HOVER_DEFAULT was specified during the original TrackMouseEvent request.
            /// </summary>
            TME_QUERY = 0x40000000,
        }

        #endregion Mouse Hover Event

    }
}
