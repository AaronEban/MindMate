﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MindMate.Controller;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MindMate.Model;
using MindMate.Serialization;
using MindMate.View.Dialogs;
using MindMate.View.NoteEditing;

namespace MindMate.Tests.Controller
{
    [TestClass()]
    public class NoteCtrlTests
    {
        [TestMethod()]
        public void NoteCtrl()
        {
            MetaModel.MetaModel.Initialize();
            var persistence = new PersistenceManager();
            var nodeEditor = new NoteEditor();
            var sut = new NoteCtrl(nodeEditor, persistence);

            Assert.IsNotNull(sut);
        }

        [TestMethod()]
        private Form CreateForm()
        {
            Form form = new Form();
            form.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            form.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            form.ClientSize = new System.Drawing.Size(415, 304);
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            form.KeyPreview = true;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.Name = "TestForm";
            form.ShowInTaskbar = false;
            form.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            form.Text = "Test Form";
            form.Size = new Size(320,320);
            return form;
        }

        [TestMethod()]
        public void NoteCtrl_AssignNote_EditorUpdated()
        {
            MetaModel.MetaModel.Initialize();
            var persistence = new PersistenceManager();
            var noteEditor = new NoteEditor();

            bool result = true;

            var form = CreateForm();
            form.Controls.Add(noteEditor);
            form.Shown += (sender, args) =>
            {
                var tree = persistence.NewTree();

                var sut = new NoteCtrl(noteEditor, persistence);

                tree.Tree.RootNode.NoteText = "ABC";

                result = noteEditor.HTML != null && noteEditor.HTML.Contains("ABC");

                form.Close();
            };

            form.ShowDialog();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void NoteCtrl_AssignNoteToUnselected()
        {
            MetaModel.MetaModel.Initialize();
            var persistence = new PersistenceManager();
            var noteEditor = new NoteEditor();

            bool result = true;

            var form = CreateForm();
            form.Controls.Add(noteEditor);
            form.Shown += (sender, args) =>
            {
                var tree = persistence.NewTree();
                var c1 = new MapNode(tree.Tree.RootNode, "c1");

                var sut = new NoteCtrl(noteEditor, persistence);

                c1.NoteText = "ABC";

                result = noteEditor.HTML == null;

                form.Close();
            };

            form.ShowDialog();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void NoteCtrl_AssignNoteToUnselected_ClearNoteEditor()
        {
            MetaModel.MetaModel.Initialize();
            var persistence = new PersistenceManager();
            var noteEditor = new NoteEditor();

            bool result = true;

            var form = CreateForm();
            form.Controls.Add(noteEditor);
            form.Shown += (sender, args) =>
            {
                var tree = persistence.NewTree();
                var c1 = new MapNode(tree.Tree.RootNode, "c1");
                c1.Selected = true;

                var sut = new NoteCtrl(noteEditor, persistence);

                c1.NoteText = "ABC";

                c1.Parent.Selected = true;

                result = noteEditor.HTML == null;

                form.Close();
            };

            form.ShowDialog();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void NoteCtrl_MultiSelection_ClearNoteEditor()
        {
            MetaModel.MetaModel.Initialize();
            var persistence = new PersistenceManager();
            var noteEditor = new NoteEditor();

            bool result = true;

            var form = CreateForm();
            form.Controls.Add(noteEditor);
            form.Shown += (sender, args) =>
            {
                var tree = persistence.NewTree();
                var c1 = new MapNode(tree.Tree.RootNode, "c1");
                c1.Selected = true;

                var sut = new NoteCtrl(noteEditor, persistence);

                c1.NoteText = "ABC";

                tree.Tree.SelectedNodes.Add(c1.Parent);

                result = noteEditor.HTML == null;

                form.Close();
            };

            form.ShowDialog();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void NoteCtrl_ChangeCurrentMapTree()
        {
            MetaModel.MetaModel.Initialize();
            var persistence = new PersistenceManager();
            var noteEditor = new NoteEditor();

            bool result = true;

            var form = CreateForm();
            form.Controls.Add(noteEditor);
            form.Shown += (sender, args) =>
            {
                var ptree1 = persistence.NewTree();
                var c1 = new MapNode(ptree1.Tree.RootNode, "c1");
                c1.Selected = true;

                var sut = new NoteCtrl(noteEditor, persistence);

                c1.NoteText = "ABC";

                var pTree2 = persistence.NewTree();

                result = noteEditor.HTML == null;

                form.Close();
            };

            form.ShowDialog();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void UpdateNodeFromEditor_WithoutCalling_MapNotUpdated()
        {
            MetaModel.MetaModel.Initialize();
            var persistence = new PersistenceManager();
            var noteEditor = new NoteEditor();

            bool result = true;

            var form = CreateForm();
            form.Controls.Add(noteEditor);
            form.Shown += (sender, args) =>
            {
                var ptree1 = persistence.NewTree();
                var c1 = new MapNode(ptree1.Tree.RootNode, "c1");
                c1.Selected = true;

                var sut = new NoteCtrl(noteEditor, persistence);

                c1.NoteText = "ABC";

                noteEditor.HTML = "EFG";
                
                result = c1.NoteText != null && c1.NoteText.Contains("ABC");

                form.Close();
            };

            form.ShowDialog();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void UpdateNodeFromEditor()
        {
            MetaModel.MetaModel.Initialize();
            var persistence = new PersistenceManager();
            var noteEditor = new NoteEditor();

            bool result = true;

            var form = CreateForm();
            form.Controls.Add(noteEditor);
            form.Shown += (sender, args) =>
            {
                var ptree1 = persistence.NewTree();
                var c1 = new MapNode(ptree1.Tree.RootNode, "c1");
                c1.Selected = true;

                var sut = new NoteCtrl(noteEditor, persistence);

                c1.NoteText = "ABC";

                noteEditor.HTML = "EFG";
                noteEditor.Dirty = true; 
                sut.UpdateNodeFromEditor();

                result = c1.NoteText != null && c1.NoteText.Contains("EFG");

                form.Close();
            };

            form.ShowDialog();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void UpdateNodeFromEditor_WithSettingDirty()
        {
            MetaModel.MetaModel.Initialize();
            var persistence = new PersistenceManager();
            var noteEditor = new NoteEditor();

            bool result = true;

            var form = CreateForm();
            form.Controls.Add(noteEditor);
            form.Shown += (sender, args) =>
            {
                var ptree1 = persistence.NewTree();
                var c1 = new MapNode(ptree1.Tree.RootNode, "c1");
                c1.Selected = true;

                var sut = new NoteCtrl(noteEditor, persistence);

                c1.NoteText = "ABC";

                noteEditor.HTML = "EFG";
                //noteEditor.Dirty = true;
                sut.UpdateNodeFromEditor();

                result = c1.NoteText != null && c1.NoteText.Contains("ABC");

                form.Close();
            };

            form.ShowDialog();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void SetNoteEditorBackColor()
        {
            MetaModel.MetaModel.Initialize();
            var persistence = new PersistenceManager();
            var noteEditor = new NoteEditor();

            bool result = true;

            var form = CreateForm();
            form.Controls.Add(noteEditor);
            form.Shown += (sender, args) =>
            {
                var ptree1 = persistence.NewTree();
                var c1 = new MapNode(ptree1.Tree.RootNode, "c1");
                c1.Selected = true;

                var sut = new NoteCtrl(noteEditor, persistence);
                sut.SetNoteEditorBackColor(Color.Azure);
                
                result = noteEditor.BackColor.Equals(Color.Azure);

                form.Close();
            };

            form.ShowDialog();

            Assert.IsTrue(result);
        }
    }
}