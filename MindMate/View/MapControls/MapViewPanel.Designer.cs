﻿namespace MindMate.View.MapControls
{
    partial class MapViewPanel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mEditNode = new System.Windows.Forms.ToolStripMenuItem();
            this.mInsertChild = new System.Windows.Forms.ToolStripMenuItem();
            this.mDeleteNode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mSelectIcon = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mEditNode,
            this.mInsertChild,
            this.toolStripSeparator1,
            this.mSelectIcon,
            this.toolStripSeparator2,
            this.mDeleteNode});
            this.contextMenu.Name = "contextMenuStrip1";
            this.contextMenu.Size = new System.Drawing.Size(167, 104);
            // 
            // mEditNode
            // 
            this.mEditNode.Name = "mEditNode";
            this.mEditNode.Size = new System.Drawing.Size(166, 22);
            this.mEditNode.Text = "Edit Node";
            // 
            // mInsertChild
            // 
            this.mInsertChild.Name = "mInsertChild";
            this.mInsertChild.Size = new System.Drawing.Size(166, 22);
            this.mInsertChild.Text = "Insert Child Node";
            // 
            // mDeleteNode
            // 
            this.mDeleteNode.Name = "mDeleteNode";
            this.mDeleteNode.Size = new System.Drawing.Size(166, 22);
            this.mDeleteNode.Text = "Delete Node";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(163, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(163, 6);
            // 
            // mSelectIcon
            // 
            this.mSelectIcon.Name = "mSelectIcon";
            this.mSelectIcon.Size = new System.Drawing.Size(166, 22);
            this.mSelectIcon.Text = "Select Icon ...";
            // 
            // MapViewPanel
            // 
            this.ContextMenuStrip = this.contextMenu;
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.NodeLinksPanel_PreviewKeyDown);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ToolStripMenuItem mEditNode;
        public System.Windows.Forms.ToolStripMenuItem mInsertChild;
        public System.Windows.Forms.ToolStripMenuItem mDeleteNode;
        public System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        public System.Windows.Forms.ToolStripMenuItem mSelectIcon;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}
