﻿using MindMate.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MindMate.Plugins.Tasks.Model
{
    /// <summary>
    /// List of Completed Tasks with Completion Date in sorted order.
    /// List keeps itself updated by listening with MapTree events.
    /// Collection is only aware about CompletionDate and TaskStatus attributes and it doesn't change any MapNode attributes.
    /// If completion date of a task is changed, it should be removed and re-added to maintain sort order.
    /// </summary>
    public class CompletedTaskList : IComparer<MapNode>, IEnumerable<MapNode>/*, IList<MapNode>*/
    {
        private List<MapNode> tasks;

        public CompletedTaskList()
        {
            tasks = new List<MapNode>();
            completedTaskArgs = new CompletedTaskEventArgs();
        }
        public void RegisterMap(MapTree tree)
        {
            tree.AttributeChanged += Tree_AttributeChanged;

            tree.SelectedNodes.NodeSelected += SelectedNodes_NodeSelected;
            tree.SelectedNodes.NodeDeselected += SelectedNodes_NodeDeselected;

            tree.NodePropertyChanged += Tree_NodePropertyChanged;
            tree.TreeStructureChanged += Tree_TreeStructureChanged;
        }

        public void UnregisterMap(MapTree tree)
        {
            tree.AttributeChanged -= Tree_AttributeChanged;

            tree.SelectedNodes.NodeSelected -= SelectedNodes_NodeSelected;
            tree.SelectedNodes.NodeDeselected -= SelectedNodes_NodeDeselected;

            tree.NodePropertyChanged -= Tree_NodePropertyChanged;
            tree.TreeStructureChanged -= Tree_TreeStructureChanged;

            Clear(tree);
        }

        private void Clear(MapTree tree)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                MapNode node = this[i];
                if (node.Tree == tree)
                {
                    RemoveAt(i);
                    TaskChanged(node, new CompletedTaskEventArgs() { TaskChange = CompletedTaskChange.TaskRemoved, OldCompletionDate = node.GetCompletionDate(), OldTaskStatus = node.GetTaskStatus() });
                }
            }
        }

        private CompletedTaskEventArgs GetEventArgs(MapNode node, CompletedTaskChange change, AttributeChangeEventArgs e)
        {
            completedTaskArgs.TaskChange = change;
            completedTaskArgs.OldTaskStatus = (e.AttributeSpec.IsTaskStatus() && e.oldValue != null) ?
                (TaskStatus)Enum.Parse(typeof(TaskStatus),e.oldValue) : node.GetTaskStatus();
            if(e.AttributeSpec.IsCompletionDate())
            {
                if (e.oldValue == null)
                    completedTaskArgs.OldCompletionDate = DateTime.MinValue;
                else
                    completedTaskArgs.OldCompletionDate = DateHelper.ToDateTime(e.oldValue);
            }
            else
            {
                if (node.CompletionDateExists())
                    completedTaskArgs.OldCompletionDate = node.GetCompletionDate();
                else
                    completedTaskArgs.OldCompletionDate = DateTime.MinValue;
            }

            return completedTaskArgs;    
        }

        private void Tree_AttributeChanged(MapNode node, AttributeChangeEventArgs e)
        {
            // task completed
            if (e.ChangeType == AttributeChange.Added && e.AttributeSpec.IsCompletionDate())
            {
                Add(node);
                TaskChanged(node, GetEventArgs(node, CompletedTaskChange.TaskCompleted, e));                
            }
            // task removed
            else if (e.ChangeType == AttributeChange.Removed && e.AttributeSpec.IsCompletionDate())
            {
                if (Remove(node))
                {
                    TaskChanged(node, GetEventArgs(node, CompletedTaskChange.TaskRemoved, e));                 
                }
            }
            // completion date updated
            else if(e.ChangeType == AttributeChange.ValueUpdated && e.AttributeSpec.IsCompletionDate())
            {
                Remove(node);
                Add(node);
                TaskChanged(node, GetEventArgs(node, CompletedTaskChange.CompletionDateUpdated, e));
            }
        }        
        
        private void Tree_NodePropertyChanged(MapNode node, NodePropertyChangedEventArgs e)
        {
            if (TaskTextChanged == null) return;

            if (e.ChangedProperty == NodeProperties.Text)
            {
                // update task title
                if (node.CompletionDateExists())
                {
                    TaskTextChanged(node, new TaskTextEventArgs() { ChangeType = TaskTextChange.TextChange, OldText = (string)e.OldValue });
                }

                // update task parent
                if(node.HasChildren)
                {
                    foreach(MapNode n in this)
                    {
                        if (n.isDescendent(node))
                            TaskTextChanged(n, new TaskTextEventArgs() { ChangeType = TaskTextChange.TextChange, ChangedAncestor = node, OldText = (string)e.OldValue });
                    }
                }
            }
        }

        private void Tree_TreeStructureChanged(MapNode node, TreeStructureChangedEventArgs e)
        {
            if (e.ChangeType == TreeStructureChange.Deleted || e.ChangeType == TreeStructureChange.Detached)
            {
                node.ForEach(n =>
                    {
                        if (n.IsTaskComplete())
                        {
                            Remove(n);
                            var evtTask = new CompletedTaskEventArgs();
                            evtTask.TaskChange = CompletedTaskChange.TaskRemoved;
                            if (n.CompletionDateExists()) evtTask.OldCompletionDate = n.GetCompletionDate();
                            evtTask.OldTaskStatus = n.GetTaskStatus();
                            TaskChanged(n, evtTask);
                        }
                    });
            }
            else if (e.ChangeType == TreeStructureChange.Attached)
            {
                node.ForEach((n) =>
                {
                    if (n.GetTaskStatus() == TaskStatus.Open && n.CompletionDateExists())
                    {
                        Add(n);
                        var evtTask = new CompletedTaskEventArgs();
                        evtTask.TaskChange = CompletedTaskChange.TaskCompleted;
                        if (n.CompletionDateExists()) evtTask.OldCompletionDate = n.GetCompletionDate();
                        evtTask.OldTaskStatus = n.GetTaskStatus();
                        TaskChanged(n, evtTask);
                    }
                });
            }
        }
        private void SelectedNodes_NodeSelected(MapNode node, SelectedNodes selectedNodes)
        {
            if (TaskSelectionChanged == null) return;

            if (node.GetTaskStatus() == TaskStatus.Open && node.CompletionDateExists())
            {
                TaskSelectionChanged(node, new TaskSelectionEventArgs() { ChangeType = TaskSelectionChange.Selected });
            }
        }

        private void SelectedNodes_NodeDeselected(MapNode node, SelectedNodes selectedNodes)
        {
            if (TaskSelectionChanged == null) return;

            if (node.GetTaskStatus() == TaskStatus.Open && node.CompletionDateExists())
            {
                TaskSelectionChanged(node, new TaskSelectionEventArgs() { ChangeType = TaskSelectionChange.Deselected });
            }
        }

        /// <summary>
        /// Subscribing to this event can be performance intensive if many tasks are there. 
        /// For every MapNode text change, it traverses through the task list to check if the changed MapNode is ancestor of any task.
        /// If no one subscribes, than no overhead is incurred.
        /// </summary>
        public event TaskTextChangedDelegate TaskTextChanged;

        public event TaskSelectionChangedDelegate TaskSelectionChanged;

        private CompletedTaskEventArgs completedTaskArgs;
        public event CompletedTaskChangedDelegate TaskChanged = delegate { };
        

        #region IList<MapNode>

        public MapNode this[int index]
        {
            get
            {
                return ((IList<MapNode>)tasks)[index];
            }
            //set
            //{
            //    ((IList<MapNode>)tasks)[index] = value;
            //}
        }

        public int Count
        {
            get
            {
                return ((IList<MapNode>)tasks).Count;
            }
        }

        //public bool IsReadOnly
        //{
        //    get
        //    {
        //        return ((IList<MapNode>)tasks).IsReadOnly;
        //    }
        //}

        private void Add(MapNode item)
        {
            int index = tasks.BinarySearch(item, this);
            if (index > -1)
                tasks.Insert(index + 1, item);
            else
                tasks.Insert(~index, item);
        }
        /// <summary>
        /// Compares two tasks in terms of their CompletionDate
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(MapNode x, MapNode y)
        {
            return DateTime.Compare(x.GetCompletionDate(), y.GetCompletionDate());            
        }

        public bool Contains(MapNode item)
        {
            return ((IList<MapNode>)tasks).Contains(item);
        }

        public void CopyTo(MapNode[] array, int arrayIndex)
        {
            ((IList<MapNode>)tasks).CopyTo(array, arrayIndex);
        }

        public IEnumerator<MapNode> GetEnumerator()
        {
            return tasks.GetEnumerator();
        }

        public int IndexOf(MapNode item)
        {
            return tasks.BinarySearch(item);
        }

        public int IndexOfGreaterThan(DateTime value, bool includeEqualto = false)
        {
            int lo = 0;
            int hi = 0 + tasks.Count - 1;
            while (lo <= hi)
            {
                int i = lo + ((hi - lo) >> 1);
                int order = DateTime.Compare(tasks[i].GetCompletionDate(), value);

                if (order == 0) return i + (includeEqualto ? 0 : 1);
                if (order < 0)
                {
                    lo = i + 1;
                }
                else
                {
                    hi = i - 1;
                }
            }

            return lo;
        }

        private void Insert(int index, MapNode item)
        {
            ((IList<MapNode>)tasks).Insert(index, item);
        }

        private bool Remove(MapNode item)
        {
            return ((IList<MapNode>)tasks).Remove(item);
        }

        private void RemoveAt(int index)
        {
            ((IList<MapNode>)tasks).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return tasks.GetEnumerator();
        }

        #endregion #region IList<MapNode>

    }
}
