using iro4cli.Compile;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace iro4cli
{
    /// <summary>
    /// Compiles the style map defined in the Iro grammar into CSS colours.
    /// </summary>
    public class CSSCompiler : ICompileTarget
    {
        //Valid CSS colours.
        private static List<string> validColours = new List<string>()
        {
            "aliceblue","antiquewhite","aqua","aquamarine","azure","beige","bisque","black","blanchedalmond","blue","blueviolet","brown","burlywood","cadetblue","chatreuse","chocolate","coral","cornflowerblue","cornsilk","crimson","cyan","darkblue","darkcyan","darkgoldenrod","darkgray","darkgrey","darkgreen","darkkhaki","darkmagenta","darkolivegreen","darkorange","darkorchid","darkred","darksalmon","darkseagreen","darkslateblue","darkslategray","darkslategrey","darkturquoise","darkviolet","deeppink","deepskyblue","dimgray","dimgrey","dodgerblue","firebrick","floralwhite","forestgreen","fuchsia","gainsboro","ghostwhite","gold","goldenrod","gray","grey","green","greenyellow","honeydew","hotpink","indianred","indigo","ivory","khaki","lavender","lavenderblush","lawngreen","lemonchiffon","lightblue","lightcoral","lightcyan","lightgoldenrodyellow","lightgray","lightgrey","lightgreen","lightpink","lightsalmon","lightseagreen","lightskyblue","lightslategray","lightslategrey","lightsteelblue","lightyellow","lime","limegreen","linen","magenta","maroon","mediumaquamarine","mediumblue","mediumorchid","mediumpurple","mediumseagreen","mediumslateblue","mediumspringgreen","mediumturquoise","mediumvioletred","midnightblue","mintcream","mistyrose","moccasin","navajowhite","navy","oldlace","olive","olivedrab","orange","orangered","orchid","palegoldenrod","palegreen","paleturquoise","palevioletred","papayawhip","peachpuff","peru","pink","plum","powderblue","purple","rebeccapurple","red","rosybrown","royalblue","saddlebrown","salmon","sandybrown","seagreen","seashell","sienna","silver","skyblue","slateblue","slategray","slategrey","snow","springgreen","steelblue","tan","teal","thistle","tomato","turquoise","violet","wheat","white","whitesmoke","yellow","yellowgreen"
        };

        private static Regex validHexColour = new Regex("^#[0-9a-fA-F]{3}$|^#[0-9a-fA-F]{6}$");

        public CompileResult Compile(IroPrecompileData data)
        {
            string text = "";

            //Loop over styles and remove weird proprietary Iro colour values (WhyYYYYY)
            foreach (var style in data.Styles)
            {
                ReplaceIroColours(ref style.Colour);
                ReplaceIroColours(ref style.BackgroundColour);
            }

            //Loop over styles, generate.
            foreach (var style in data.Styles)
            {
                //Name of the CSS class.
                if (!style.Name.StartsWith("."))
                {
                    Error.CompileWarning("When compiling for CSS, style names should start with a '.', prepending.");
                    text += ".";
                }

                text += style.Name + " {";
                
                //Body.
                if (style.Colour == null)
                {
                    Error.Compile("When compiling for CSS, all styles must have a 'color' or 'colour' property.");
                    return null;
                }
                if (!validHexColour.IsMatch(style.Colour) && !validColours.Contains(style.Colour.ToLower()))
                {
                    Error.Compile("Invalid colour name given for style '" + style.Name + "', must be a valid CSS colour or hexadecimal colour value.");
                    return null;
                }
                text += " color:" + style.Colour + "; ";

                //Is there a background colour?
                if (style.BackgroundColour != null)
                {
                    //Valid?
                    if (!validHexColour.IsMatch(style.Colour) && !validColours.Contains(style.Colour.ToLower()))
                    {
                        Error.Compile("Invalid background colour given for style '" + style.BackgroundColour + "', must be a valid CSS colour or hexadecimal colour value.");
                        return null;
                    }

                    text += "background-color: " + style.BackgroundColour + "; ";
                }

                //Font stylings.s
                if (style.Bold)
                {
                    text += "font-weight: bold; ";
                }
                if (style.Italic)
                {
                    text += "font-style: italic; ";
                }

                //Close the style! :)
                text += "}\r\n";
            }

            //Done compiling, return target.
            return new CompileResult()
            {
                Target = Target.CSS,
                GeneratedFile = text
            };
        }

        /// <summary>
        /// Replaces weird proprietary Iro colours with valid CSS colours.
        /// </summary>
        private void ReplaceIroColours(ref string colour)
        {
            switch (colour)
            {
                case "light_blue":
                case "light_gray":
                case "light_grey":
                case "light_green":
                case "light_yellow":
                case "violet_red":
                    colour = colour.Replace("_", "");
                    break;
                case "red_2":
                    colour = "mediumvioletred";
                    break;
            }
        }
    }
}