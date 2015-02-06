﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MindMate.Plugins.Tasks
{
    public partial class TasksList : UserControl
    {
        public TasksList()
        {
            InitializeComponent();
        }        

        public void Add(MindMate.Model.MapNode node, DateTime dateTime)
        {
            if(DateHelper.IsToday(dateTime))
            {
                AddTask(this.collapsiblePanelToday, this.tableLayoutToday, node, dateTime,
                    DateHelper.GetTimePartString(dateTime));
            }              
            else if(DateHelper.IsTomorrow(dateTime))
            {
                AddTask(this.collapsiblePanelTomorrow, this.tableLayoutTomorrow, node, dateTime,
                    DateHelper.GetTimePartString(dateTime));
            }
            else if(DateHelper.DateInThisWeek(dateTime))
            {
                AddTask(this.collapsiblePanelThisWeek, this.tableLayoutThisWeek, node, dateTime,
                    DateHelper.GetWeekDayString(dateTime));
            }
            else if(DateHelper.DateInThisMonth(dateTime))
            {
                AddTask(this.collapsiblePanelThisMonth, this.tableLayoutThisMonth, node, dateTime,
                    DateHelper.GetDayOfMonthString(dateTime));
            }
            else if (DateHelper.DateInNextMonth(dateTime))
            {
                AddTask(this.collapsiblePanelNextMonth, this.tableLayoutNextMonth, node, dateTime,
                    DateHelper.GetDayOfMonthString(dateTime));
            }

            Control lastTaskGroup = GetLastTaskGroup();
            this.tablePanelMain.Height = lastTaskGroup.Location.Y + lastTaskGroup.Size.Height + lastTaskGroup.Margin.Bottom;
        }

        
        private void AddTask(MindMate.Plugins.Tasks.CollapsiblePanel collapsiblePanel, TableLayoutPanel tableLayout,
            MindMate.Model.MapNode node, DateTime dateTime, string dueOnText)
        {
            TaskView tv = new TaskView(node, dateTime, dueOnText);

            if (tableLayout.RowCount == 0) collapsiblePanel.Visible = true;
            
            InsertTaskView(tableLayout, tv);
            
            tableLayout.Height = tableLayout.RowCount * tv.Height + (tableLayout.Margin.Bottom * tableLayout.RowCount * 2);
            collapsiblePanel.Height = tableLayout.Height + tableLayout.Top;
        }

        private void InsertTaskView(TableLayoutPanel tableLayout, TaskView taskView)
        {
            if (tableLayout.RowCount == 0)
            {
                tableLayout.Controls.Add(taskView, 0, 0); // add if list is empty
                tableLayout.RowCount += 1;
                return;
            }
            else
            {
                for (int i = tableLayout.RowCount - 1; i >= 0; i--)
                {
                    TaskView tv = (TaskView)tableLayout.GetControlFromPosition(0, i);
                    if (tv.DueDate > taskView.DueDate)
                    {
                        tableLayout.SetRow(tv, i + 1);
                    }
                    else
                    {
                        tableLayout.Controls.Add(taskView, 0, i + 1); // add in the middle or end
                        tableLayout.RowCount += 1;
                        return;
                    }
                }

                tableLayout.Controls.Add(taskView, 0, 0); // add at the top after all controls are moved down using loop
                tableLayout.RowCount += 1;
                return;
            }
            
        }

        private Control GetLastTaskGroup()
        {
            for(int i = this.tablePanelMain.RowCount - 1; i >= 0; i--)
            {
                Control ctrl = this.tablePanelMain.GetControlFromPosition(0, i);
                if (ctrl != null)
                    return ctrl;
            }

            return null;
        }
        
    }
}
