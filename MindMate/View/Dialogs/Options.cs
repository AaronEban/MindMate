﻿/* Author: Syed Umar Anis (mail@umaranis.com)                    
 * Copyright (c) 2014 Syed Umar Anis                             
 * This software is license under MIT license (see LICENSE.txt)    
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MindMate.View.Dialogs
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
            this.propertyGrid1.SelectedObject = MetaModel.MetaModel.Instance;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            MetaModel.MetaModel.Instance.Save();
        }
    }
}
