﻿/* Author: Syed Umar Anis (mail@umaranis.com)                    
 * Copyright (c) 2014 Syed Umar Anis                             
 * This software is license under MIT license (see LICENSE.txt)    
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MindMate.Model;

namespace MindMate.View.MapControls
{
    public class Link
    {
        public Link(string link, NodeLinkType linkType)
        {
            this.linkType = linkType;

            switch(linkType)
            {
                case NodeLinkType.MindMapNode:
                    bitmap = MindMate.Properties.Resources.LinkLocal;
                    break;
                case NodeLinkType.InternetLink:
                    bitmap = MindMate.Properties.Resources.LinkWeb;
                    break;
                case NodeLinkType.Executable:
                    bitmap = MindMate.Properties.Resources.Executable;
                    break;
                case NodeLinkType.ExternalFile:
                    bitmap = MindMate.Properties.Resources.Link;
                    break;
            }            
        }

        
        private NodeLinkType linkType;

        public NodeLinkType LinkType
        {
            get { return linkType; }
            set { linkType = value; }
        }

        PointF location;

        public PointF Location
        {
            get { return location; }
            set { location = value; }
        }

               
        public Size Size
        {
            get 
            { 
                return bitmap.Size; 
            }            
        }

        Bitmap bitmap;

        public Bitmap Bitmap
        {
            get 
            { 
                return bitmap; 
            }            
        }

        public void Draw(Graphics g)
        {
            g.DrawImage(bitmap, location.X, location.Y, Size.Width, Size.Height);
        }
    }
}
