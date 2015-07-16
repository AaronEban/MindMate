﻿using MindMate.Model;
using MindMate.Plugins.Tasks.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MindMate.Plugins.Tasks
{
    public partial class TaskPlugin
    {
        private void PendingTasks_TaskChanged(MapNode node, PendingTaskEventArgs e)
        {
            switch (e.TaskChange)
            {
                case PendingTaskChange.TaskAdded:
                    taskListView.Add(node);
                    break;
                case PendingTaskChange.TaskCompleted:
                case PendingTaskChange.TaskRemoved:
                    taskListView.RemoveTask(node, e.OldDueDate);
                    break;
                case PendingTaskChange.DueDateUpdated:
                    taskListView.RefreshTaskDueDate(node, e.OldDueDate);
                    break;
            }
        }

        private void PendingTasks_TaskTextChanged(MapNode node, TaskTextEventArgs e)
        {
            switch (e.ChangeType)
            {
                case TaskTextChange.TextChange:
                    taskListView.RefreshTaskText(node);
                    break;
                case TaskTextChange.AncestorTextChange:
                    taskListView.RefreshTaskPath(node);
                    break;
            }
        }

        private void PendingTasks_TaskSelectionChanged(MapNode node, TaskSelectionEventArgs e)
        {
            switch (e.ChangeType)
            {
                case TaskSelectionChange.Selected:
                    taskListView.SelectNode(node);
                    break;
                case TaskSelectionChange.Deselected:
                    taskListView.DeselectNode(node);
                    break;
            }
        }

        void taskList_TaskViewEvent(TaskView tv, TaskView.TaskViewEvent e)
        {
            switch (e)
            {
                case TaskView.TaskViewEvent.Remove:
                    tv.MapNode.RemoveTask();
                    break;
                case TaskView.TaskViewEvent.Complete:
                    tv.MapNode.CompleteTask();
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
                    tv.MapNode.SetDueDate(DateHelper.GetDefaultDueDateToday());
                    break;
                case TaskView.TaskViewEvent.Tomorrow:
                    tv.MapNode.SetDueDate(DateHelper.GetDefaultDueDateTomorrow());
                    break;
                case TaskView.TaskViewEvent.NextWeek:
                    tv.MapNode.SetDueDate(DateHelper.GetDefaultDueDateNextWeek());
                    break;
                case TaskView.TaskViewEvent.NextMonth:
                    tv.MapNode.SetDueDate(DateHelper.GetDefaultDueDateNextMonth());
                    break;
                case TaskView.TaskViewEvent.NextQuarter:
                    tv.MapNode.SetDueDate(DateHelper.GetDefaultDueDateNextQuarter());
                    break;

            }

            pluginManager.FocusMapEditor();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tv"></param>
        private void MoveDown(TaskView tv)
        {
            taskListView.MoveDown(tv);
        }

        private void MoveUp(TaskView tv)
        {
            taskListView.MoveUp(tv);
        }
    }
}
