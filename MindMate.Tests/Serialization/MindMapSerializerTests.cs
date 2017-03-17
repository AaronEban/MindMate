﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MindMate.Model;
using MindMate.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MindMate.Tests
{
    [TestClass()]
    public class MindMapSerializerTests
    {
        
        [TestMethod()]
        public void Deserialize_FeatureDisplay()
        {
            MindMapSerializer s = new MindMapSerializer();
            string originalText = File.ReadAllText(@"Resources\Feature Display.mm");
            MapTree tree = new MapTree();
            s.Deserialize(originalText, tree);

            Assert.AreEqual(tree.RootNode.ChildNodes.Count(), 9);
        }

        [TestMethod()]
        public void Deserialize_RootNode()
        {
            MindMapSerializer s = new MindMapSerializer();
            string XMLString = File.ReadAllText(@"Resources\Feature Display.mm");
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument(); 
            xmlDoc.LoadXml(XMLString);
            XmlElement x = xmlDoc.DocumentElement;

            var t = new MapTree();
            var r = new MapNode(t, null);

            for (int i = 0; i < x.ChildNodes.Count; i++)
            {
                XmlNode xnode = x.ChildNodes[i];
                if (xnode.Name == "node")
                {
                    s.Deserialize(xnode, t);
                }
            }

            Assert.AreEqual(t.RootNode.ChildNodes.Count(), 9);
        }

        [TestMethod]
        public void DeserializeSerialize_FeatureDisplay()
        {
            MindMapSerializer s = new MindMapSerializer();
            string originalText = File.ReadAllText(@"Resources\Feature Display.mm");
            MapTree tree = new MapTree();
            s.Deserialize(originalText, tree);

            using (MemoryStream stream = new MemoryStream())
            {
                s.Serialize(stream, tree);
                stream.Position = 0;
                string generatedText = new StreamReader(stream).ReadToEnd();

                Assert.AreEqual(originalText, generatedText, "Serialized xml doesn't match this original.");
            }
        }

        [TestMethod]
        public void DeserializeSerialize_RichNotesMap()
        {
            MindMapSerializer s = new MindMapSerializer();
            string originalText = File.ReadAllText(@"Resources\RichNotesMap.mm");
            MapTree tree = new MapTree();
            s.Deserialize(originalText, tree);

            using (MemoryStream stream = new MemoryStream())
            {
                s.Serialize(stream, tree);
                stream.Position = 0;
                string generatedText = new StreamReader(stream).ReadToEnd();

                Assert.AreEqual(originalText, generatedText, "Serialized xml doesn't match this original.");
            }
        }

        [TestMethod]
        public void Serialize_WithImagePath()
        {
            MapTree tree = new MapTree();
            var r = new MapNode(tree, "r");
            r.Image = "MyImage.png";
            r.TextAlignment = TextAlignment.AfterCenter;

            MindMapSerializer s = new MindMapSerializer();
            using (MemoryStream stream = new MemoryStream())
            {
                s.Serialize(stream, tree);
                stream.Position = 0;
                string serializedText = new StreamReader(stream).ReadToEnd();

                var tree2 = new MapTree();
                s.Deserialize(serializedText, tree2);
                Assert.AreEqual("MyImage.png", tree2.RootNode.Image);
                Assert.AreEqual(TextAlignment.AfterCenter, tree2.RootNode.TextAlignment);
            }
        }
    }
}