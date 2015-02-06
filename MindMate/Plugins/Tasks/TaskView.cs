﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MindMate.Model;

namespace MindMate.Plugins.Tasks
{
    public partial class TaskView : UserControl
    {
        public TaskView(MapNode node, DateTime dueDate, string dueOnText)
        {
            InitializeComponent();

            TaskTitle = node.Text;

            //TODO: What will happen if node is moved or any of the ancestors is moved
            lblTaskPath.Text = "";
            MapNode parentNode = node.Parent;
            while (parentNode != null)
            {
                if (lblTaskPath.Text != "") lblTaskPath.Text += " <- ";
                lblTaskPath.Text += node.Parent.Text;
                parentNode = parentNode.Parent;
            }

            DueDate = dueDate;

            TaskDueOnText = dueOnText;
        }

        public string TaskTitle
        {
            get
            {
                return this.lblNodeName.Text; 
            }
            set
            {
                this.lblNodeName.Text = value;
            }
        }

        public string TaskPath
        {
            get
            {
                return this.lblTaskPath.Text;
            }
            set
            {
                this.lblTaskPath.Text = value;
            }
        }

        public string TaskDueOnText
        {
            get
            {
                return this.lblDueOn.Text;
            }
            set
            {
                this.lblDueOn.Text = value;
            }
        }

        public MapNode MapNode
        {
            get
            {
                return (MapNode)this.Tag;
            }
            set
            {
                this.Tag = value;
            }
        }

        public DateTime DueDate { get; set; }

    }    
}
