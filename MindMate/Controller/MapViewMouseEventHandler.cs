﻿/* Author: Syed Umar Anis (mail@umaranis.com)                    
 * Copyright (c) 2014 Syed Umar Anis                             
 * This software is licensed under MIT (see LICENSE.txt)    
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MindMate.Serialization;
using MindMate.Model;
using MindMate.View.MapControls;

namespace MindMate.Controller
{
    /// <summary>
    /// Event Handler for MapView. Handles mouse events and passes them to the Controller
    /// </summary>
    public class MapViewMouseEventHandler
    {

        private MapCtrl mapCtrl = null;
        public MapViewMouseEventHandler(MapCtrl mapCtrl)
        {
            this.mapCtrl = mapCtrl;
        }

        /// <summary>
        /// Currently it is hooked up to mouse down event
        /// </summary>
        /// <param name="node"></param>
        /// <param name="evt"></param>
        public void MapNodeClick(MapNode node, NodeMouseEventArgs evt)
        {
            if (mapCtrl.MapView.NodeTextEditor.IsTextEditing) return;

            bool shiftKeyDown = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
            bool ctrlKeyDown = (Control.ModifierKeys & Keys.Control) == Keys.Control;
            
            // Toggle folding or follow link (node has children + no key modifier)
            if (node.HasChildren && !shiftKeyDown && !ctrlKeyDown)
            {
                mapCtrl.MapView.SelectedNodes.Add(node, shiftKeyDown || ctrlKeyDown);
                if (node.Link == null || evt.NodePortion == NodePortion.Body)
                {
                    if (node.Parent != null) mapCtrl.ToggleNode(node);
                }
                else
                {
                    mapCtrl.followLink(node);
                }
            }
            // (deselect already selected node)ctrl key + node already selected
            else if (mapCtrl.MapView.SelectedNodes.Count > 1 && mapCtrl.MapView.SelectedNodes.Contains(node) && ctrlKeyDown)
            {
                mapCtrl.MapView.SelectedNodes.Remove(node);
                mapCtrl.MapView.Canvas.Invalidate();
            }
            else
            {
                mapCtrl.MapView.SelectedNodes.Add(node, shiftKeyDown || ctrlKeyDown);

                //edit node or follow link (no children + only selected node + no key modifier)
                if (mapCtrl.MapView.SelectedNodes.Count == 1 && !node.HasChildren &&
                !shiftKeyDown && !ctrlKeyDown)
                {
                    if (node.Link == null)
                    {
                        mapCtrl.BeginNodeEdit(node, TextCursorPosition.End);
                    }
                    else
                    {
                        mapCtrl.followLink(node);
                    }
                }
                mapCtrl.MapView.Canvas.Invalidate();
            }

        }

        
        public void MapNodeMouseOver(MapNode node, NodeMouseEventArgs evt)
        {
            if (mapCtrl.MapView.NodeTextEditor.IsTextEditing) return;

            if (Control.ModifierKeys != Keys.None || mapCtrl.MapView.SelectedNodes.Count > 1)
            {
                return;
            }

            
            mapCtrl.MapView.SelectedNodes.Add(node, false);

            if (node.Link != null)
            {
                if (!node.HasChildren)
                {
                    mapCtrl.MapView.Canvas.Cursor = Cursors.Hand;
                }
                else if (node.HasChildren)
                {
                    if (evt.NodePortion == NodePortion.Head)
                        mapCtrl.MapView.Canvas.Cursor = Cursors.Hand;
                    else
                        mapCtrl.MapView.Canvas.Cursor = Cursors.Default;
                }
            }
            else
            {
                mapCtrl.MapView.Canvas.Cursor = Cursors.Default;
            }

            mapCtrl.MapView.Canvas.Invalidate();
            return;
            
            
        }

        public void NodeMouseExit(MapNode node, MouseEventArgs e)
        {
            if (mapCtrl.MapView.Canvas.Cursor != Cursors.Default)
            {
                mapCtrl.MapView.Canvas.Cursor = Cursors.Default;
            }
        }
       
    }
}
