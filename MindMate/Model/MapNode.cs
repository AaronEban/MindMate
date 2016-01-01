﻿/* Author: Syed Umar Anis (mail@umaranis.com)                    
 * Copyright (c) 2014 Syed Umar Anis                             
 * This software is licensed under MIT (see LICENSE.txt)    
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MindMate.View.MapControls;
using System.Drawing;
using System.Diagnostics;

namespace MindMate.Model
{
    public partial class MapNode
    {

        #region Node Attributes

        #region Serializable
        /// <summary>
        /// It is used for hyperlinking nodes, it is null generally.
        /// </summary>
        public string Id { get; private set; }

        private NodePosition pos;
        public NodePosition Pos
        {
            get
            {
                return pos;
            }
        }

        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                object oldValue = text;
                text = value;
                modified = DateTime.Now;
                Tree.FireEvent(this, NodeProperties.Text, oldValue);
            }
        }

        private bool folded;
        public bool Folded
        {
            get
            {
                return folded;
            }
            set
            {
                if (folded == value) return;
                folded = value;
                Tree.FireEvent(this, NodeProperties.Folded, !folded);
            }
        }

        private bool bold;
        public bool Bold
        {
            get
            {
                return bold;
            }
            set
            {
                if (bold == value) return;
                bold = value;
                modified = DateTime.Now;
                Tree.FireEvent(this, NodeProperties.Bold, !bold);
            }
        }

        private bool italic;
        public bool Italic
        {
            get
            {
                return italic;
            }
            set
            {
                if (italic == value) return;
                italic = value;
                modified = DateTime.Now;
                Tree.FireEvent(this, NodeProperties.Italic, !italic);
            }
        }

        private bool strikeout;

        public bool Strikeout
        {
            get { return strikeout; }
            set
            {
                if (strikeout == value) return;
                strikeout = value;
                modified = DateTime.Now;
                Tree.FireEvent(this, NodeProperties.Strikeout, !strikeout);
            }
        }

        private string fontName;
        public string FontName {
            get
            {
                return fontName;
            }
            set
            {
                object oldValue = fontName;
                fontName = value;
                modified = DateTime.Now;
                Tree.FireEvent(this, NodeProperties.FontName, oldValue);
            }
        }

        private float fontSize;
        /// <summary>
        /// 0 is the default value, meaning Font Size is not defined (default size should be used)
        /// </summary>
        public float FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                object oldValue = fontSize;
                fontSize = value;
                modified = DateTime.Now;
                Tree.FireEvent(this, NodeProperties.FontSize, oldValue);
            }
        }

        public IconList Icons { get; private set; }

        private string link;
        public string Link
        {
            get
            {
                return link;
            }
            set
            {
                object oldValue = link;
                link = value;
                modified = DateTime.Now;
                Tree.FireEvent(this, NodeProperties.Link, oldValue);
            }
        }

        public DateTime Created { get; set; }

        private DateTime modified;
        public DateTime Modified {
            get
            {
                return modified;
            }
            set
            {
                modified = value;
            }
        }

        private Color backColor;
        /// <summary>
        /// Default value is Color.Empty
        /// </summary>
        public Color BackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                object oldValue = backColor;
                backColor = value;
                modified = DateTime.Now;
                Tree.FireEvent(this, NodeProperties.BackColor, oldValue);
            }
        }

        private Color color;
        /// <summary>
        /// Default value is Color.Empty
        /// </summary>
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                object oldValue = color;
                color = value;
                modified = DateTime.Now;
                Tree.FireEvent(this, NodeProperties.Color, oldValue);
            }
        }

        private NodeShape shape;
        public NodeShape Shape
        {
            get
            {
                return shape;
            }
            set
            {
                object oldValue = shape;
                shape = value;
                modified = DateTime.Now;
                Tree.FireEvent(this, NodeProperties.Shape, oldValue);
            }
        }

        private int lineWidth;
        /// <summary>
        /// 0 stands for default line width (as parent)
        /// </summary>
        public int LineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                object oldValue = lineWidth;
                lineWidth = value;
                modified = DateTime.Now;
                Tree.FireEvent(this, NodeProperties.LineWidth, oldValue);
            }
        }

        private System.Drawing.Drawing2D.DashStyle linePattern = System.Drawing.Drawing2D.DashStyle.Custom;
        /// <summary>
        /// Custom stands for default (as parent)
        /// </summary>
        public System.Drawing.Drawing2D.DashStyle LinePattern
        {
            get
            {
                return linePattern;
            }
            set
            {
                object oldValue = linePattern;
                linePattern = value;
                modified = DateTime.Now;
                Tree.FireEvent(this, NodeProperties.LinePattern, oldValue);
            }
        }

        private Color lineColor;
        public Color LineColor
        {
            get
            {
                return lineColor;
            }
            set
            {
                object oldValue = lineColor;
                lineColor = value;
                modified = DateTime.Now;
                Tree.FireEvent(this, NodeProperties.LineColor, oldValue);
            }
        }

        private NodeRichContentType richContentType;
        public NodeRichContentType RichContentType
        {
            get
            {
                return richContentType;
            }
            set
            {
                object oldValue = richContentType;
                richContentType = value;
                modified = DateTime.Now;
                Tree.FireEvent(this, NodeProperties.RichContentType, oldValue);
            }
        }

        private string richContentText;
        public string RichContentText
        {
            get
            {
                return richContentText;
            }
            set
            {
                object oldValue = richContentText;
                richContentText = value;
                modified = DateTime.Now;
                Tree.FireEvent(this, NodeProperties.RichContentText, oldValue);
            }
        }

        private Image image;
        public Image Image
        {
            get { return image; }
            set
            {
                Image oldValue = image;
                image = value;
                modified = DateTime.Now;
                Tree.FireEvent(this, NodeProperties.Image, oldValue); 
            }
        }

        private ImageAlignment imageAlignment;
        public ImageAlignment ImageAlignment
        {
            get { return imageAlignment; }
            set
            {
                ImageAlignment oldValue = imageAlignment;
                imageAlignment = value;
                modified = DateTime.Now;
                Tree.FireEvent(this, NodeProperties.ImageAlignment, oldValue);
            }
        }

        private string label;
        public string Label
        {
            get { return label; }
            set
            {
                string oldValue = Label;
                label = value;
                modified = DateTime.Now;
                Tree.FireEvent(this,NodeProperties.Label, oldValue);
            }
        }

        #endregion

        #region Non-serializable
        public MapTree Tree { get; private set; }
        public MapNode Parent { get; private set; }
        public MapNode Previous { get; private set; }
        public MapNode Next { get; private set; }
        public MapNode FirstChild { get; set; }
        public MapNode LastChild { get; set; }

        #endregion

        #endregion

        #region Supporting Properties (without attributes)

        public bool HasChildren { get { return FirstChild != null; } }

        public bool Selected {
            get { return Tree.SelectedNodes.Contains(this); }
            set
            {
                if(value)
                {
                    Tree.SelectedNodes.Add(this);
                }
                else
                {
                    Tree.SelectedNodes.Remove(this);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates root node
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="text"></param>
        /// <param name="id"></param>
        public MapNode(MapTree tree, string text, string id = null)
        {
            this.Id = id;
            this.text = text;
            this.Created = DateTime.Now;
            this.modified = DateTime.Now;
            this.richContentType = NodeRichContentType.NONE;
            this.Icons = new IconList(this);

            // setting NodePosition
            this.pos = NodePosition.Root;

            // attaching to tree
            this.Tree = tree;
            tree.RootNode = this;

            Tree.FireEvent(this, TreeStructureChange.New);
        }

        /// <summary>
        /// Creates a child node. 
        /// </summary>
        /// <param name="parent">Should not be null</param>
        /// <param name="text"></param>
        /// <param name="pos">If undefined, determined from parent node. In case of root node, balances the tree.</param>
        /// <param name="id">could be null</param>  
        /// <param name="adjacentToSib">Appended at the end if null.</param>
        public MapNode(MapNode parent, string text, NodePosition pos = NodePosition.Undefined,
            string id = null, MapNode adjacentToSib = null, bool insertAfterSib = true)
        {
            System.Diagnostics.Debug.Assert(parent != null, "parent parameter should not be null. Use other constructor for root node.");

            this.Id = id;
            this.text = text;
            this.Created = DateTime.Now;
            this.modified = DateTime.Now;
            this.richContentType = NodeRichContentType.NONE;
            this.Icons = new IconList(this);

            // attaching to tree
            AttachTo(parent, adjacentToSib, insertAfterSib, pos, false);

            Tree.FireEvent(this, TreeStructureChange.New);
        }

        private NodePosition GetNodePositionToBalance()
        {

            var leftNodeCnt = 0;
            var rightNodeCnt = 0;

            foreach (var node in ChildNodes)
            {
                if (node.Pos == NodePosition.Left)
                    leftNodeCnt++;
                else
                    rightNodeCnt++;
            }

            return leftNodeCnt < rightNodeCnt ? NodePosition.Left : NodePosition.Right;
        }

        public void AttachTo(MapNode parent, MapNode adjacentToSib = null, bool insertAfterSib = true,
                    NodePosition pos = NodePosition.Undefined, bool raiseAttachEvent = true)
        {
            //Debug.Assert(!(adjacentToSib == null && insertAfterSib == false)); //AttachTo handles this case

            this.Parent = parent;
            this.Tree = parent.Tree;

            // setting NodePosition
            if (pos != NodePosition.Undefined) this.pos = pos;
            else if (parent == null) this.pos = NodePosition.Root;
            else if (adjacentToSib != null) this.pos = adjacentToSib.Pos;
            else if (parent.Pos == NodePosition.Root) this.pos = parent.GetNodePositionToBalance();
            else this.pos = parent.Pos;

            // get the last sib if appendAfter is not given
            if (adjacentToSib == null) adjacentToSib = parent.GetLastChild(this.Pos);

            // if last child is not available on the given pos, then try on the other side
            if (adjacentToSib == null)
            {
                if (this.Pos == NodePosition.Left)
                {
                    adjacentToSib = parent.LastChild;
                    insertAfterSib = true;
                }
                else
                {
                    adjacentToSib = parent.FirstChild;
                    insertAfterSib = false;
                }
            }

            // link with siblings
            if (adjacentToSib != null && insertAfterSib == true)
            {
                this.Previous = adjacentToSib;
                this.Next = adjacentToSib.Next;
                adjacentToSib.Next = this;
                if (this.Next != null) this.Next.Previous = this;
            }
            else if(adjacentToSib != null && insertAfterSib == false)
            {
                this.Previous = adjacentToSib.Previous;
                this.Next = adjacentToSib;
                if(this.Previous != null) this.Previous.Next = this;
                adjacentToSib.Previous = this;
            }
            else
            {
                this.Previous = null;
                this.Next = null;
            }

            // link with parent
            if (this.Previous == null) parent.FirstChild = this;
            if (this.Next == null) parent.LastChild = this;

            if (this.HasChildren)
                ForEach(n => n.pos = this.pos);

            parent.modified = DateTime.Now;
            if(raiseAttachEvent)    Tree.FireEvent(this, TreeStructureChange.Attached);


        }
        #endregion


        #region Detached Node

        /// <summary>
        /// Post Conditions: Detached == true && Selected == false
        /// </summary>
        public void Detach()
        {
            if (Parent != null)
            {
                Selected = false;

                if (Parent.FirstChild == this) Parent.FirstChild = this.Next;
                if (Parent.LastChild == this) Parent.LastChild = this.Previous;

                if (this.Previous != null)
                {
                    this.Previous.Next = this.Next;
                }

                if (this.Next != null)
                {
                    this.Next.Previous = this.Previous;
                }

                Parent.modified = DateTime.Now;

                Tree.FireEvent(this, TreeStructureChange.Detached);

            }
        }

        /// <summary>
        /// No operation should be performed on detached node or its decendents except restoring them.
        /// Descendents of a detached node are still return Detached as false.
        /// </summary>
        public bool Detached
        {
            get
            {
                return (Previous != null && Previous.Next != this) ||
                        (Next != null && Next.Previous != this) ||
                        Parent != null && !Parent.ChildNodes.Contains(this);
            }
        }

        /// <summary>
        /// Create a detached node. Detached node represents deleted/cut/copied nodes. They should not be modified in any way, should only be restored.
        /// 
        /// Modifying them will generate events which is not expected.
        /// </summary>
        /// <returns></returns>
        public MapNode CloneAsDetached()
        {
            var newNode = new MapNode(Tree);
            newNode.pos = this.Pos;
            this.CopyNodePropertiesTo(newNode);
            
            foreach (MapNode childNode in this.ChildNodes)
            {
                childNode.CloneAsDetached(newNode);
            }

            return newNode;
        }

        /// <summary>
        /// Copy this node with descendents and attach it to the location (parameter)
        /// </summary>
        /// <param name="location"></param>
        /// <param name="includeDescendents"></param>
        private void CloneAsDetached(MapNode location)
        {
            var node = new MapNode(location.Tree);

            // attaching to tree
            node.AttachTo(location, null, true, this.pos, false);
            this.CopyNodePropertiesTo(node);

            foreach (MapNode childNode in this.ChildNodes)
            {
                childNode.CloneAsDetached(node);
            }
        }

        private MapNode(MapTree tree)
        {
            this.Created = DateTime.Now;
            this.modified = DateTime.Now;
            this.richContentType = NodeRichContentType.NONE;
            this.Icons = new IconList(this);

            this.Tree = tree;
        }
                
        /// <summary>
        /// Copy all current node properties on to the node passed as parameter. No property change notifications are triggered.
        /// </summary>
        /// <param name="node"></param>
        private void CopyNodePropertiesTo(MapNode node)
        {
            // node.Id, node.Created, node.Modified, node.Pos -- shouldn't be copied
            node.text = this.text;
            node.label = this.label;
            node.folded = this.folded;
                       
            node.link = this.link;

            node.richContentText = this.richContentText;
            node.richContentType = this.richContentType;

            node.image = this.image;
            node.imageAlignment = this.imageAlignment;

            this.CopyAttributesTo(node);
            this.CopyIconsTo(node);
            this.CopyFormattingTo(node);
        }

        /// <summary>
        /// No property change notifications are triggered
        /// </summary>
        /// <param name="node"></param>
        private void CopyFormattingTo(MapNode node)
        {
            node.backColor = this.backColor;
            node.bold = this.bold;
            node.color = this.color;
            node.fontName = this.fontName;
            node.fontSize = this.fontSize;
            node.italic = this.italic;
            node.strikeout = this.strikeout;
            node.lineColor = this.lineColor;
            node.linePattern = this.linePattern;
            node.lineWidth = this.lineWidth;
            node.shape = this.shape;
        }

        /// <summary>
        /// No property change notiifcations are triggered
        /// </summary>
        /// <param name="node"></param>
        private void CopyAttributesTo(MapNode node)
        {
            if (this.attributeList != null)
            {
                node.EnsureAttributeListCreated();
                foreach (Attribute att in attributeList)
                {
                    node.attributeList.Add(att);
                }
            }
            else if (node.attributeList != null)
            {
                node.attributeList.Clear();
            }
        }

        /// <summary>
        /// No property change notifications are generated
        /// </summary>
        /// <param name="node"></param>
        private void CopyIconsTo(MapNode node)
        {
            node.Icons.Clear();
            foreach (string icon in this.Icons)
            {
                node.Icons.Add(icon);
            }
        }

        #endregion Detached Node

        public MapNode GetFirstSib()
        {
            return this.Parent.FirstChild;
        }

        public MapNode GetLastSib()
        {
            return this.Parent.LastChild;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos">Left or Right. Root and Undefined are not supported.</param>
        /// <returns></returns>
        public MapNode GetFirstChild(NodePosition pos)
        {
            System.Diagnostics.Debug.Assert(pos != NodePosition.Undefined, "Undefined NodePosition is not supported.");
            System.Diagnostics.Debug.Assert(pos != NodePosition.Root, "Root NodePosition is not supported.");

            if (pos == NodePosition.Right)
            {
                return this.FirstChild;
            }
            else
            {
                MapNode cNode = FirstChild;
                while (cNode != null)
                {
                    if (pos == cNode.Pos) return cNode;
                    cNode = cNode.Next;
                }
                return null;
            }
        }

        public MapNode GetLastChild(NodePosition pos)
        {
            System.Diagnostics.Debug.Assert(pos != NodePosition.Undefined, "Undefined NodePosition is not supported.");
            System.Diagnostics.Debug.Assert(pos != NodePosition.Root, "Root NodePosition is not supported.");

            if(pos == NodePosition.Left)
            {
                return LastChild;
            }
            else
            {
                MapNode cNode = LastChild;
                while (cNode != null)
                {
                    if (pos == cNode.Pos) return cNode;
                    cNode = cNode.Previous;
                }
                return null;
            }
        }

        public IEnumerable<MapNode> ChildNodes
        {
            get
            {
                MapNode cNode = this.FirstChild;

                if (cNode != null)
                {
                    do
                    {
                        yield return cNode;
                        cNode = cNode.Next;
                    }
                    while (cNode != null);
                }
            }
        }

        public IEnumerable<MapNode> ChildRightNodes
        {
            get
            {
                MapNode cNode = this.FirstChild;

                if (cNode != null && cNode.Pos == NodePosition.Right)
                {
                    do
                    {
                        yield return cNode;
                        cNode = cNode.Next;
                    }
                    while (cNode != null && cNode.Pos == NodePosition.Right);
                }
            }
        }

        public IEnumerable<MapNode> ChildLeftNodes
        {
            get
            {
                MapNode cNode = this.GetFirstChild(NodePosition.Left);

                if (cNode != null)
                {
                    do
                    {
                        yield return cNode;
                        cNode = cNode.Next;
                    }
                    while (cNode != null);
                }
            }
        }


        private void ChangePos(NodePosition pos)
        {
            ForEach(n => n.pos = pos);
            modified = DateTime.Now;
            Tree.FireEvent(this, pos == NodePosition.Left? TreeStructureChange.MovedLeft : TreeStructureChange.MovedRight);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node">node to be tested for being ancestor of 'this' node</param>
        /// <returns></returns>
        public bool IsDescendent(MapNode node)
        {
            if(this.Parent != null)
            {
                if (this.Parent == node)
                    return true;
                else
                {
                    return this.Parent.IsDescendent(node);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the location of sibling (above this node, below this node or not sibling)
        /// </summary>
        /// <param name="sibling"></param>
        public SiblingLocaton GetSiblingLocation(MapNode sibling)
        {
            if(sibling.Parent == this.Parent)
            {
                MapNode temp = this.Parent.FirstChild;
                do
                {
                    if(temp == sibling)
                    {
                        return SiblingLocaton.Above;
                    }
                    else if(temp == this)
                    {
                        return SiblingLocaton.Below;
                    }
                    else
                    {
                        temp = temp.Next;
                    }
                } while (true);
            }
            else
            {
                return SiblingLocaton.NotSibling;
            }
        }

        public enum SiblingLocaton { NotSibling, Above, Below }


        public void DeleteNode()
        {
            if (this.Parent == null)    return;

            Selected = false;

            if (Parent.FirstChild == this) Parent.FirstChild = this.Next;
            if (Parent.LastChild == this) Parent.LastChild = this.Previous;

            if (this.Previous != null)
            {
                this.Previous.Next = this.Next;
            }
            if (this.Next != null)
            {
                this.Next.Previous = this.Previous;
            }            

            Parent.modified = DateTime.Now;

            Tree.FireEvent(this, TreeStructureChange.Deleted);
        }


        public bool MoveUp()
        {
            if (this.Pos == NodePosition.Root) // return if root
                return false;

            if(this.Previous == null) // if previous node is null, move from left to right else do nothing
            {
                if (this.Pos == NodePosition.Left && this.Parent.pos == NodePosition.Root)
                {
                    this.ChangePos(NodePosition.Right);
                    return true;
                }
                else
                    return false;
            }

            if (this.Previous.Pos != this.Pos) // move from left to right side
            {
                this.ChangePos(this.Previous.Pos);
                return true;
            }

            // move up
            MapNode previousNode = this.Previous;

            previousNode.Next = this.Next;
            if(this.Next != null) this.Next.Previous = previousNode;

            this.Previous = previousNode.Previous;
            if (previousNode.Previous != null)  previousNode.Previous.Next = this;

            this.Next = previousNode;
            previousNode.Previous = this;

            if (this.Previous == null) Parent.FirstChild = this;
            if (Parent.LastChild == this) Parent.LastChild = this.Next;

            Parent.modified = DateTime.Now;
            Tree.FireEvent(this, TreeStructureChange.MovedUp);

            return true;
        }

        public bool MoveDown()
        {
            if (this.Pos == NodePosition.Root) // return if root
                return false;

            if(this.Next == null) // if next node is null, move from right to left, else do nothing
            {
                if (this.Pos == NodePosition.Right && this.Parent.pos == NodePosition.Root)
                {
                    this.ChangePos(NodePosition.Left);
                    return true;
                }
                else
                    return false;
            }

            if(this.Next.Pos != this.Pos) // move from right to left
            {
                this.ChangePos(this.Next.Pos);
                return true;
            }


            // move down
            MapNode nextNode = this.Next;

            nextNode.Previous = this.Previous;
            if (this.Previous != null) this.Previous.Next = nextNode;

            this.Next = nextNode.Next;
            if (nextNode.Next != null) nextNode.Next.Previous = this;

            this.Previous = nextNode;
            nextNode.Next = this;

            if (this == Parent.FirstChild) Parent.FirstChild = this.Previous;
            if (this.Next == null) Parent.LastChild = this;

            Parent.modified = DateTime.Now;
            Tree.FireEvent(this, TreeStructureChange.MovedDown);

            return true;
        }

        /// <summary>
        /// Finds the first node that matches the provided condition.
        /// Node and it's descendents are searched.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns>MapNode that matches the condition or null if no such node found</returns>"></param>
        public MapNode Find(Func<MapNode, bool> condition)
        {
            if (condition(this)) return this;

            foreach (MapNode n in this.ChildNodes)
            {
                MapNode result = n.Find(condition);
                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Finds node and its descendents which match the condition 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<MapNode> FindAll(Func<MapNode, bool> condition)
        {
            var list = new List<MapNode>();
            FindAll(condition, list);
            return list;
        }

        private void FindAll(Func<MapNode, bool> condition, List<MapNode> list)
        {
            if (condition(this))
            {
                list.Add(this);
            }

            foreach (MapNode n in this.ChildNodes)
            {
                n.FindAll(condition, list);
            }
        }

        /// <summary>
        /// Performs the given action for node and it's descendents
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<MapNode> action)
        {
            action(this);

            foreach(MapNode cNode in this.ChildNodes)
            {
                cNode.ForEach(action);
            }
        }

        /// <summary>
        /// Performs the given action for node's ancestors (excluding the node itself)
        /// </summary>
        /// <param name="action"></param>
        public void ForEachAncestor(Action<MapNode> action)
        {
            MapNode parent = this.Parent;
            while(parent != null)
            {
                action(parent);
                parent = parent.Parent;
            }
        }

        public NodeLinkType GetLinkType()
        {
            NodeLinkType linkType; int i;

            if (link == null)
            {
                linkType = NodeLinkType.Empty;
            }
            else if (link.StartsWith("#"))
            {
                linkType = NodeLinkType.MindMapNode;
            }
            else if (link.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || link.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                linkType = NodeLinkType.InternetLink;
            }
            else if (link.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
            {
                linkType = NodeLinkType.EmailLink;
            }
            else if (link.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                linkType = NodeLinkType.Executable;
            }
            else if (link.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                || link.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                || link.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)
                || link.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                || link.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
            {
                linkType = NodeLinkType.ImageFile;
            }
            else if (link.EndsWith(".mov", StringComparison.OrdinalIgnoreCase)
                || link.EndsWith(".mpg", StringComparison.OrdinalIgnoreCase)
                || link.EndsWith(".mpeg", StringComparison.OrdinalIgnoreCase)
                || link.EndsWith(".wmv", StringComparison.OrdinalIgnoreCase)
                || link.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)
                || link.EndsWith(".flv", StringComparison.OrdinalIgnoreCase)
                || link.EndsWith(".avi", StringComparison.OrdinalIgnoreCase)
                || link.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
            {
                linkType = NodeLinkType.VideoFile;
            }
            else if((i = link.LastIndexOf('\\')) > 0 && link.IndexOf('.', i) < 0)
            {
                linkType = NodeLinkType.Folder;
            }
            else
            {
                linkType = NodeLinkType.File;
            }

            return linkType;
        }

        public int GetNodeDepth()
        {
            int depth = 0;
            MapNode node = this;
            while(node.Parent != null)
            {
                depth++;
                node = node.Parent;
            }
            return depth;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Text) &&
                   string.IsNullOrEmpty(Label) &&
                   Icons.Count == 0 &&
                   AttributeCount == 0 &&
                   RichContentType == NodeRichContentType.NONE &&
                   Image == null &&
                   !HasChildren;
        }

        public void CopyFormatTo(MapNode node)
        {
            node.BackColor = this.BackColor;
            node.Bold = this.Bold;
            node.Color = this.Color;
            node.FontName = this.FontName;
            node.FontSize = this.FontSize;
            node.Italic = this.Italic;
            node.Strikeout = this.Strikeout;
            node.LineColor = this.LineColor;
            node.LinePattern = this.LinePattern;
            node.LineWidth = this.LineWidth;
            node.Shape = this.Shape;
        }

        public override string ToString()
        {
            return Text;
        }

        #region Node View Property
        private NodeView nodeView = null;

        public NodeView NodeView
        {
            get { return nodeView; }
            set { nodeView = value; }
        }

        #endregion

    }
}
