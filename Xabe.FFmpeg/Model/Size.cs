﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Xabe.FFmpeg.Model
{
    public class Size
    {
        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Height { get; set; }
        public int Width { get; set; }
    }
}