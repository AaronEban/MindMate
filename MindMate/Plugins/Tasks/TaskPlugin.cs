﻿using MindMate.MetaModel;
using MindMate.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MindMate.Plugins.Tasks
{
    public partial class TaskPlugin : IPlugin
    {
        
        private DateTimePicker dateTimePicker; 
        private TaskList taskList;                

        public void Initialize(IPluginManager pluginMgr)
        {
            dateTimePicker = new DateTimePicker();
            taskList = new TaskList();
            taskList.TaskViewEvent += taskList_TaskViewEvent;
        }

        void taskList_TaskViewEvent(TaskView tv, TaskView.TaskViewEvent e)
        {
            switch(e)
            { 
                case TaskView.TaskViewEvent.Remove:
                    RemoveTask(tv.MapNode);
                    break;
                case TaskView.TaskViewEvent.Complete:
                    CompleteTask(tv.MapNode);
                    break;
                case TaskView.TaskViewEvent.Defer:
                    MoveDown(tv);
                    break;
                case TaskView.TaskViewEvent.Expedite:
                    MoveUp(tv);
                    break;
                case TaskView.TaskViewEvent.Edit:
                    SetDueDate(tv.MapNode);
                    break;
                case TaskView.TaskViewEvent.Today:
                    SetDueDate(tv.MapNode, DateHelper.GetDefaultDueDateToday());
                    break;
                case TaskView.TaskViewEvent.Tomorrow:
                    SetDueDate(tv.MapNode, DateHelper.GetDefaultDueDateTomorrow());
                    break;
                case TaskView.TaskViewEvent.NextWeek:
                    SetDueDate(tv.MapNode, DateHelper.GetDefaultDueDateNextWeek());
                    break;
                case TaskView.TaskViewEvent.NextMonth:
                    SetDueDate(tv.MapNode, DateHelper.GetDefaultDueDateNextMonth());
                    break;
                case TaskView.TaskViewEvent.NextQuarter:
                    SetDueDate(tv.MapNode, DateHelper.GetDefaultDueDateNextQuarter());
                    break;

            }
        }
                                       
        public void CreateMainMenuItems(out MenuItem[] menuItems, out MainMenuLocation position)
        {
            throw new NotImplementedException();
        }

        public Control[] CreateSideBarWindows()
        {
            taskList.Text = "Tasks";
            return new Control [] { taskList };
        }

        public void OnCreatingTree(Model.MapTree tree)
        {
            tree.AttributeChanged += tree_AttributeChanged;

            tree.SelectedNodes.NodeSelected += SelectedNodes_NodeSelected;
            tree.SelectedNodes.NodeDeselected += SelectedNodes_NodeDeselected;

            tree.NodePropertyChanged += tree_NodePropertyChanged;
            tree.TreeStructureChanged += tree_TreeStructureChanged;
        }

        void SelectedNodes_NodeSelected(MapNode node, SelectedNodes selectedNodes)
        {
            if(node.DueDateExists())
            {
                TaskView tv = taskList.FindTaskView(node, node.GetDueDate());
                if(tv != null) tv.Selected = true;
            }
        }

        void SelectedNodes_NodeDeselected(MapNode node, SelectedNodes selectedNodes)
        {
            if (node.DueDateExists())
            {
                TaskView tv = taskList.FindTaskView(node, node.GetDueDate());
                if(tv != null) tv.Selected = false;
            }
        }

        private void tree_AttributeChanged(MapNode node, AttributeChangeEventArgs e)
        {
            // Due Date attribute changed
            if (e.ChangeType == AttributeChange.Removed && e.oldValue.AttributeSpec.IsDueDate()) 
            {
                TaskView tv = taskList.FindTaskView(node, DateHelper.ToDateTime(e.oldValue.value));
                if (tv != null) taskList.RemoveTask(tv);

                TaskDueIcon.FireStatusChangeEvent(node, SystemIconStatusChange.Hide);
            }
            else if (e.ChangeType == AttributeChange.Added && e.newValue.AttributeSpec.IsDueDate())
            {
                taskList.Add(node, DateHelper.ToDateTime(e.newValue.value));

                TaskDueIcon.FireStatusChangeEvent(node, SystemIconStatusChange.Show);
            }
            else if (e.ChangeType == AttributeChange.ValueUpdated && e.newValue.AttributeSpec.IsDueDate())
            {
                TaskView tv = taskList.FindTaskView(node, DateHelper.ToDateTime(e.oldValue.value));
                if (tv != null) taskList.RemoveTask(tv);
                taskList.Add(node, DateHelper.ToDateTime(e.newValue.value));
            }
            // Comletion Date attribute changed
            else if (e.ChangeType == AttributeChange.Added && e.newValue.AttributeSpec.IsCompletionDate())
            {
                TaskCompleteIcon.FireStatusChangeEvent(node, SystemIconStatusChange.Show);
            }
            else if(e.ChangeType == AttributeChange.Removed && e.oldValue.AttributeSpec.IsCompletionDate())
            {
                TaskCompleteIcon.FireStatusChangeEvent(node, SystemIconStatusChange.Hide);
            }
        }

        void tree_NodePropertyChanged(MapNode node, NodePropertyChangedEventArgs e)
        {
            if(e.ChangedProperty == NodeProperties.Text && node.DueDateExists())
            {
                TaskView tv = taskList.FindTaskView(node, node.GetDueDate());
                if (tv != null) tv.TaskTitle = node.Text;
            }            
        }

        void tree_TreeStructureChanged(MapNode node, TreeStructureChangedEventArgs e)
        {
            if(e.ChangeType == TreeStructureChange.Deleting)
            {
                RefreshTaskList(node, tv => taskList.RemoveTask(tv));
            }
        }

        /// <summary>
        /// Refreshes TaskList for any changes to changedNode or its descendents
        /// </summary>
        /// <param name="changedNode"></param>
        /// <param name="operation"></param>
        private void RefreshTaskList(MapNode changedNode, Action<TaskView> operation)
        {
            //int taskViewCount = taskList.GetControlCount();
            //for(int i = 0; i < taskViewCount; i++)
            //{
            //    TaskView tv = (TaskView)taskList.GetControl(i);
            //    if (tv.MapNode == changedNode || tv.MapNode.isDescendent(changedNode))
            //        operation(tv);
            //}

            if (!changedNode.HasChildren && changedNode.DueDateExists())
            {
                taskList.RemoveTask(taskList.FindTaskView(changedNode, changedNode.GetDueDate()));
            }
            else
            {
                TaskView ctrl = (TaskView)taskList.GetFirstControl();
                TaskView nextCtrl;

                while (ctrl != null)
                {
                    nextCtrl = (TaskView)taskList.GetNextControl(ctrl); //this method has to be called before operation as operation might delete the ctrl
                    if (ctrl.MapNode == changedNode || ctrl.MapNode.isDescendent(changedNode))
                        operation(ctrl);
                    ctrl = nextCtrl;
                }
            }
        }

        
                
        public void OnDeletingTree(Model.MapTree tree)
        {
            throw new NotImplementedException();
        }

        private void SetDueDate(MapNode node, DateTime dateTime)
        {
            node.SetDueDate(dateTime);
            node.RemoveCompletionDate();
        }
                       
        private void CompleteTask(MapNode node)
        {
            if (node.DueDateExists())
            {
                node.SetTargetDate(node.GetDueDate());
                node.RemoveDueDate();
                
            }

            node.SetCompletionDate(DateTime.Now);
        }

        private void RemoveTask(MapNode node)
        {
            node.RemoveDueDate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tv"></param>
        private void MoveDown(TaskView tv)
        {
            taskList.MoveDown(tv);
        }

        private void MoveUp(TaskView tv)
        {
            taskList.MoveUp(tv);
        }
    }
}
