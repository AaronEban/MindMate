﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MindMate.MetaModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MindMate.Serialization;

namespace MindMate.Tests.Model
{
    [TestClass()]
    public class MetaModelTests
    {
        [TestMethod()]
        public void Deserialize_NodeStyleImageNotNull()
        {
            File.Copy(@"Resources\Settings.Yaml", Dir.UserSettingsDirectory + "Settings.Yaml", true);
            MindMate.MetaModel.MetaModel.Initialize();
            MetaModel.MetaModel model = MindMate.MetaModel.MetaModel.Instance;

            Assert.IsTrue(model.NodeStyles.TrueForAll(s => s.Image != null));
        }

        [TestMethod()]
        public void Initialize()
        {
            MindMate.MetaModel.MetaModel.Initialize();
        }

        [TestMethod()]
        public void Save()
        {
            File.Copy(@"Resources\Settings.Yaml", Dir.UserSettingsDirectory + "Settings.Yaml", true);
            MindMate.MetaModel.MetaModel.Initialize();
            MetaModel.MetaModel model = MindMate.MetaModel.MetaModel.Instance;
            model.Save();
        }

        [TestMethod()]
        public void GetIcon_NotExistingIcon_ReturnsErrorIcon()
        {
            MindMate.MetaModel.MetaModel.Initialize();
            MetaModel.MetaModel model = MindMate.MetaModel.MetaModel.Instance;
            Assert.IsNotNull(model.GetIcon("aalksdjfaksdfalds"));
        }
    }
}