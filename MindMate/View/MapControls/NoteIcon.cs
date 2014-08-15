﻿/* Author: Syed Umar Anis (mail@umaranis.com)                    
 * Copyright (c) 2014 Syed Umar Anis                             
 * This software is licensed under MIT (see LICENSE.txt)    
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MindMate.View.MapControls
{
    public class NoteIcon
    {
        public NoteIcon()
        {
            bitmap = MindMate.Properties.Resources.knotes;
            
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
