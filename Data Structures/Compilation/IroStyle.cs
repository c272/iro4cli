﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli
{
    /// <summary>
    /// Represents a single style in Iro.
    /// </summary>
    public class IroStyle
    {
        //The name of the style.
        public string Name;

        //Whether the text should be bold/italic for CSS generation.
        public bool Bold = false;
        public bool Italic = false;
        public string Colour = null;
        public string BackgroundColour = null;

        //Scopes for various text grammar formats.
        public string HighlightJSScope = null;
        public string AceScope = null;
        public string TextmateScope = null;
        public string PygmentsScope = null;

        public IroStyle(string name)
        {
            Name = name;
        }
    }
}
