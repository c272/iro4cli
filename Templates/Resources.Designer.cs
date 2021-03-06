﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iro4cli.Templates {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("iro4cli.Templates.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .vscode/**
        ///.vscode-test/**
        ///.gitignore
        ///vsc-extension-quickstart.md.
        /// </summary>
        internal static string _vscodeignore {
            get {
                return ResourceManager.GetString("_vscodeignore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to _ ___  __    _  _     ____   _  
        ///| | _ \/__\  | || |   / _/ | | | 
        ///| | v / \/ | `._  _| | \_| |_| | 
        ///|_|_|_\\__/     |_|   \__/___|_|.
        /// </summary>
        internal static string asciiArt {
            get {
                return ResourceManager.GetString("asciiArt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to # Change Log
        ///*Generated by Iro.*
        ///
        ///All notable changes to the &quot;{extension_name}&quot; extension will be documented in this file.
        ///
        ///Check [Keep a Changelog](http://keepachangelog.com/) for recommendations on how to structure this file.
        ///
        ///## [Unreleased]
        ///
        ///- Initial release.
        /// </summary>
        internal static string CHANGELOG_md {
            get {
                return ResourceManager.GetString("CHANGELOG_md", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // A launch configuration that launches the extension inside a new window
        ///// Use IntelliSense to learn about possible attributes.
        ///// Hover to view descriptions of existing attributes.
        ///// For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
        ///{
        ///	&quot;version&quot;: &quot;0.2.0&quot;,
        ///    &quot;configurations&quot;: [
        ///        {
        ///            &quot;name&quot;: &quot;Extension&quot;,
        ///            &quot;type&quot;: &quot;extensionHost&quot;,
        ///            &quot;request&quot;: &quot;launch&quot;,
        ///            &quot;runtimeExecutable&quot;: &quot;${execPath}&quot;,
        ///            &quot;args&quot;: [
        ///       [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string launch_json {
            get {
                return ResourceManager.GetString("launch_json", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to code --extensionDevelopmentPath=./.
        /// </summary>
        internal static string linuxDebug_sh {
            get {
                return ResourceManager.GetString("linuxDebug_sh", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to # {extension_name} README
        ///
        ///This is the README for your extension &quot;{extension_name}&quot;, automatically generated by Iro. After writing up a brief description, we recommend including the following sections.
        ///
        ///## Features
        ///
        ///Describe specific features of your extension including screenshots of your extension in action. Image paths are relative to this README file.
        ///
        ///For example if there is an image subfolder under your extension project workspace:
        ///
        ///\!\[feature X\]\(images/feature-x.png\)
        ///
        ///&gt; Tip: Many popu [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string README_md {
            get {
                return ResourceManager.GetString("README_md", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to # Welcome to your VS Code Extension
        ///
        ///## What&apos;s in the folder
        ///
        ///* This folder contains all of the files necessary for your extension.
        ///* `package.json` - this is the manifest file in which you declare your language support and define the location of the grammar file that has been copied into your extension.
        ///* `syntaxes/test.tmlanguage.txt` - this is the Text mate grammar file that is used for tokenization.
        ///* `language-configuration.json` - this is the language configuration, defining the tokens that are [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string vsc_extension_quickstart_md {
            get {
                return ResourceManager.GetString("vsc_extension_quickstart_md", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to code --extensionDevelopmentPath=%~dp0.
        /// </summary>
        internal static string winDebug_bat {
            get {
                return ResourceManager.GetString("winDebug_bat", resourceCulture);
            }
        }
    }
}
