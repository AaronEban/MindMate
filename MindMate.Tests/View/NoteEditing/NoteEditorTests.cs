﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MindMate.View.NoteEditing;
using System.Drawing;
using System.Windows.Forms;

namespace MindMate.Tests.View.NoteEditing
{
    /// <summary>
    /// Test methods in this are executed in a separate thread to avoid following exception:
    ///     MindMate.Tests.View.NoteEditing.NoteEditorTests.CanExecuteCommand threw exception: 
    ///     System.Threading.ThreadStateException: ActiveX control '8856f961-340a-11d0-a96b-00c04fd705a2' cannot be instantiated because the current thread is not in a single-threaded apartment.    
    /// This exception occurs in some machines also (not sure what causes it).
    /// </summary>
    [TestClass()]
    public class NoteEditorTests
    {
        [TestMethod()]
        public void NoteEditor_Creation()
        {
            NoteEditor sut = null;
            System.Threading.Thread t = new System.Threading.Thread(() =>
            {
                sut = new NoteEditor();
            });
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            t.Join();
            Assert.IsNotNull(sut);
        }

        [TestMethod()]
        public void NoteEditor_Creation_HTMLIsNull()
        {
            var sut = new NoteEditor();
            bool result = true;
            var form = CreateForm();
            form.Controls.Add(sut);
            form.Shown += (sender, args) =>
            {
                result = sut.HTML == null;
                form.Close();
            };
            form.ShowDialog();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void NoteEditor_Creation_IsDirtyFalse()
        {
            var sut = new NoteEditor();
            bool result = true;
            var form = CreateForm();
            form.Controls.Add(sut);
            form.Shown += (sender, args) =>
            {
                result = !sut.Dirty;
                form.Close();
            };
            form.ShowDialog();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Setting HTML property programmatically doesn't make Dirty true. Only changes made through interface will make NoteEditor dirty.
        /// </summary>
        [TestMethod()]
        public void NoteEditor_SetHTML_IsDirtyFalse()
        {
            var sut = new NoteEditor();
            bool result = true;
            var form = CreateForm();
            form.Controls.Add(sut);
            form.Shown += (sender, args) =>
            {
                result = !sut.Dirty;
                form.Close();
            };
            form.ShowDialog();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void Clear()
        {
            bool result = false;
            System.Threading.Thread t = new System.Threading.Thread(() =>
            {
                var sut = new NoteEditor();                
                var form = CreateForm();
                form.Controls.Add(sut);
                form.Shown += (sender, args) =>
                {
                    sut.HTML = "Sample Test";
                    sut.Clear();
                    result = sut.HTML == null;
                    form.Close();
                };
                form.ShowDialog();                
            });
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            t.Join();
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CanExecuteCommand()
        {
            var sut = new NoteEditor();
            bool result = false;
            var form = CreateForm();
            form.Controls.Add(sut);
            form.Shown += (sender, args) =>
            {
                result = sut.CanExecuteCommand(NoteEditorCommand.Bold);
                form.Close();
            };
            form.ShowDialog();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void QueryCommandState()
        {
            var sut = new NoteEditor();
            bool result = true;
            var form = CreateForm();
            form.Controls.Add(sut);
            form.Shown += (sender, args) =>
            {
                result = !sut.QueryCommandState(NoteEditorCommand.Bold);
                form.Close();
            };
            form.ShowDialog();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void ExecuteCommand()
        {
            var sut = new NoteEditor();
            bool result = true;
            var form = CreateForm();
            form.Controls.Add(sut);
            form.Shown += (sender, args) =>
            {
                sut.ExecuteCommand(NoteEditorCommand.Bold);
                result = sut.QueryCommandState(NoteEditorCommand.Bold);
                form.Close();
            };
            form.ShowDialog();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void InsertHyperlink()
        {
            var sut = new NoteEditor();
            bool result = true;
            var form = CreateForm();
            form.Controls.Add(sut);
            form.Shown += (sender, args) =>
            {
                sut.HTML = "Website";
                sut.ExecuteCommand(NoteEditorCommand.SelectAll);
                sut.InsertHyperlink("umaranis.com");
                result = sut.HTML != null && sut.HTML.Contains("umar");
                form.Close();
            };
            form.ShowDialog();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void Paste()
        {
            var sut = new NoteEditor();
            bool result = true;
            var form = CreateForm();
            form.Controls.Add(sut);
            form.Shown += (sender, args) =>
            {
                Clipboard.SetText("This is clipboard text");
                sut.ExecuteCommand(NoteEditorCommand.Paste);
                result = sut.HTML != null && sut.HTML.Contains("This is clipboard text");
                form.Close();
            };
            form.ShowDialog();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void Paste_MakesDirty()
        {
            var sut = new NoteEditor();
            var form = CreateForm();
            Timer timer = new Timer { Interval = 50 }; //timer is used because the Dirty property is updated in the next event of GUI thread.
            timer.Tick += delegate { form.Close(); };
            form.Controls.Add(sut);
            form.Shown += (sender, args) =>
            {
                Clipboard.SetText("This is clipboard text");
                sut.Paste();
            };
            timer.Start();
            form.ShowDialog();
            timer.Stop();


            Assert.IsTrue(sut.Dirty);
        }

        [TestMethod()]
        public void Paste_SetHTMLNullThenPaste_MakesDirty()
        {
            var sut = new NoteEditor();
            var form = CreateForm();
            Timer timer = new Timer { Interval = 50 }; //timer is used because the Dirty property is updated in the next event of GUI thread.
            timer.Tick += delegate { form.Close(); };
            form.Controls.Add(sut);
            form.Shown += (sender, args) =>
            {
                sut.HTML = null; //Clears Dirty flag
                Clipboard.SetText("This is clipboard text");
                sut.Paste();
            };
            timer.Start();
            form.ShowDialog();
            timer.Stop();


            Assert.IsTrue(sut.Dirty);
        }

        /// <summary>
        /// 1- Set HTML as Null (clears dirty and sets flag to ignore next dirty notification)
        /// 2- Set HTML as some text (clears dirty and sets flag to ignore next dirty notification)
        /// 3- Set HTML as some text again in same GUI event (this is to test that setting HTML twice doesn't generate 2 dirty notifications)
        /// 4- Dirty notification is generated in next GUI event and ignored
        /// 5- Change content of NoteEditor from frontend (should make NoteEditor dirty) (done in a separate GUI thread event using Timer)
        /// 6- Dirty notification is generated in next GUI event (assert will check for this)
        /// 7- Close the form in separate GUI thread event using Timer
        /// 8- Assert that NoteEditor is Dirty
        /// </summary>
        [TestMethod()]
        public void Paste_SetHTMLThenPaste_MakesDirty()
        {
            var sut = new NoteEditor();
            var form = CreateForm();
            Timer timer = new Timer { Interval = 50 }; //timer is used because the Dirty property is updated in the next event of GUI thread.
            timer.Tick += delegate
            {
                if (timer.Tag == null)
                {
                    timer.Tag = "First Event Fired";
                    Clipboard.SetText("This is clipboard text");
                    sut.Paste();
                }
                else
                {
                    form.Close();
                }
            };
            form.Controls.Add(sut);
            form.Shown += (sender, args) =>
            {
                sut.HTML = null; //Clears Dirty flag
                sut.HTML = "Some Text"; //Clears Dirty flag
                sut.HTML = "Some Text"; //Clears Dirty flag
            };
            timer.Start();
            form.ShowDialog();
            timer.Stop();


            Assert.IsTrue(sut.Dirty);
        }

        /// <summary>
        /// 1- Set HTML as Null (clears dirty and sets flag to ignore next dirty notification)
        /// 2- Set HTML as some text (clears dirty and sets flag to ignore next dirty notification)
        /// 3- Set HTML as some text again in another GUI event (this is to test that setting HTML twice ignore both dirty notifications)
        /// 4- Dirty notification is generated in next GUI event and ignored
        /// 5- Change content of NoteEditor from frontend (should make NoteEditor dirty) (done in a separate GUI thread event using Timer)
        /// 6- Dirty notification is generated in next GUI event (assert will check for this)
        /// 7- Close the form in separate GUI thread event using Timer
        /// 8- Assert that NoteEditor is Dirty
        /// </summary>
        [TestMethod()]
        public void Paste_SetHTML2ThenPaste_MakesDirty()
        {
            var sut = new NoteEditor();
            var form = CreateForm();
            Timer timer = new Timer { Interval = 50 }; //timer is used because the Dirty property is updated in the next event of GUI thread.
            timer.Tick += delegate
            {
                if (timer.Tag == null)
                {
                    timer.Tag = "First Event Fired";
                    sut.HTML = "Some Text"; //Clears Dirty flag

                }
                else if (timer.Tag.Equals("First Event Fired"))
                {
                    timer.Tag = "Second Event Fired";
                    Clipboard.SetText("This is clipboard text");
                    sut.Paste();
                }
                else
                {
                    form.Close();
                }
            };
            form.Controls.Add(sut);
            form.Shown += (sender, args) =>
            {
                sut.HTML = null; //Clears Dirty flag
                sut.HTML = "Some Text"; //Clears Dirty flag
            };
            timer.Start();
            form.ShowDialog();
            timer.Stop();


            Assert.IsTrue(sut.Dirty);
        }

        //[TestMethod()]
        //public void InsertImage()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void Search()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void Cut()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void Copy()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void ClearUndoStack()
        //{
        //    Assert.Fail();
        //}

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
            form.Size = new Size(320, 320);
            return form;
        }
    }
}