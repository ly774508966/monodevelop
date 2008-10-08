// MemberCompletionData.cs
//
// Author:
//   Mike Krüger <mkrueger@novell.com>
//
// Copyright (c) 2008 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MonoDevelop.Projects.Gui.Completion;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Projects.Dom.Output;

using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;

namespace MonoDevelop.CSharpBinding
{
	public class MemberCompletionData : ICompletionDataWithMarkup, IActionCompletionData, IOverloadedCompletionData
	{
		IMember member;
		OutputFlags flags;
		bool hideExtensionParameter = true;
		static CSharpAmbience ambience = new CSharpAmbience ();
		bool descriptionCreated = false;
		
		string description, descriptionPango, completionString;
		string[] text;
		bool markObsolete;
		
		Dictionary<string, ICompletionData> overloads;
		
		public string Description {
			get {
				CheckDescription ();
				return description;
			}
		}
		
		public string CompletionString {
			get { return completionString; }
		}
		
		public string[] Text {
			get { return text; }
		}
		
		public string Image {
			get { return member.StockIcon; }
		}
		
		public string DescriptionPango {
			get {
				CheckDescription ();
				return descriptionPango;
			}
		}
		
		public bool HideExtensionParameter {
			get {
				return hideExtensionParameter;
			}
			set {
				hideExtensionParameter = value;
			}
		}
		
		bool MarkObsolete {
			get { return markObsolete; }
			set {
				if (value == markObsolete)
					return;
				markObsolete = value;
				if (value) {
					text[0] = "<s>" + text[0] + "</s>";
				} else {
					text[0] = text[0].Substring (3, text[0].Length - 7);
				}
			}
		}
		
		public MemberCompletionData (IMember member, OutputFlags flags)
		{
			this.member = member;
			this.text = new string [] { ambience.GetString (member, flags | OutputFlags.EmitMarkup) };
			this.completionString = ambience.GetString (member, flags | OutputFlags.IncludeGenerics);
			MarkObsolete = member.IsObsolete;
		}
		
		public void InsertAction (ICompletionWidget widget, ICodeCompletionContext context)
		{
			MonoDevelop.Ide.Gui.Content.IEditableTextBuffer buf = widget
				as MonoDevelop.Ide.Gui.Content.IEditableTextBuffer;
			
			if (buf == null) {
				LoggingService.LogError ("ICompletionWidget widget is not an IEditableTextBuffer");
				return;
			}
			
			buf.BeginAtomicUndo ();
			buf.DeleteText (context.TriggerOffset, buf.CursorPosition - context.TriggerOffset);
			buf.InsertText (context.TriggerOffset, this.CompletionString);
			
			//select any generic parameters e.g. T in <T>
			int offset = this.CompletionString.IndexOf ('<');
			if (offset >= 0) {
				int endOffset = offset + 1;
				while (endOffset < this.CompletionString.Length) {
					char ch = this.CompletionString[endOffset];
					if (!Char.IsLetterOrDigit(ch) && ch != '_')
						break;
					endOffset++;
				}
				buf.CursorPosition = context.TriggerOffset + offset + 1;
				buf.Select (buf.CursorPosition, context.TriggerOffset + endOffset);
			}
			buf.EndAtomicUndo ();
		}
		
		void CheckDescription ()
		{
			if (descriptionCreated)
				return;
			
			descriptionCreated = true;
			
			string doc = ambience.GetString (member,
				OutputFlags.ClassBrowserEntries | OutputFlags.IncludeParameterName
				| (HideExtensionParameter ? OutputFlags.HideExtensionsParameter : OutputFlags.None));
			string docMarkup = ambience.GetString (member,
				OutputFlags.ClassBrowserEntries | OutputFlags.IncludeParameterName | OutputFlags.EmitMarkup
				| (HideExtensionParameter ? OutputFlags.HideExtensionsParameter : OutputFlags.None));
			if (member.IsObsolete) {
				doc += Environment.NewLine + "[Obsolete]";
				docMarkup += Environment.NewLine + "[Obsolete]";
			}
			XmlNode node = member.GetMonodocDocumentation ();
			if (node != null) {
				node = node.SelectSingleNode ("summary");
				if (node != null) {
					string mdDoc = GetDocumentation (node.InnerXml);
					doc += Environment.NewLine + mdDoc;
					docMarkup += Environment.NewLine + mdDoc;
				}
			}
			description = doc;
			descriptionPango = docMarkup;
		}
	
		public static string FormatText (string text)
		{
			StringBuilder result = new StringBuilder ();
			bool wasWhitespace = false;
			foreach (char ch in text) {
				switch (ch) {
					case '\n':
					case '\r':
						break;
					case '<':
						result.Append ("&lt;");
						break;
					case '>':
						result.Append ("&gt;");
						break;
					case '&':
						result.Append ("&amp;");
						break;
					default:
						if (wasWhitespace && Char.IsWhiteSpace (ch))
							break;
						wasWhitespace = Char.IsWhiteSpace (ch);
						result.Append (ch);
						break;
				}
			}
			return result.ToString ();
		}
		static string GetCref (string cref)
		{
			if (cref == null)
				return "";
			
			if (cref.Length < 2)
				return cref;
			
			if (cref.Substring(1, 1) == ":")
				return cref.Substring (2, cref.Length - 2);
			
			return cref;
		}
		public static string GetDocumentation (string doc)
		{
			System.IO.StringReader reader = new System.IO.StringReader("<docroot>" + doc + "</docroot>");
			XmlTextReader xml   = new XmlTextReader(reader);
			StringBuilder ret   = new StringBuilder(70);
			int lastLinePos = -1;
			
			try {
				xml.Read();
				do {
					if (xml.NodeType == XmlNodeType.Element) {
						string elname = xml.Name.ToLower();
						if (elname == "remarks") {
							ret.Append("Remarks:\n");
						// skip <example>-nodes
						} else if (elname == "example") {
							xml.Skip();
							xml.Skip();
						} else if (elname == "exception") {
							ret.Append("Exception: " + GetCref(xml["cref"]) + ":\n");
						} else if (elname == "returns") {
							ret.Append("Returns: ");
						} else if (elname == "see") {
							ret.Append(GetCref(xml["cref"]) + xml["langword"]);
						} else if (elname == "seealso") {
							ret.Append("See also: " + GetCref(xml["cref"]) + xml["langword"]);
						} else if (elname == "paramref") {
							ret.Append(xml["name"]);
						} else if (elname == "param") {
							ret.Append(xml["name"].Trim() + ": ");
						} else if (elname == "value") {
							ret.Append("Value: ");
						} else if (elname == "para")
							continue; // Keep new line flag
						lastLinePos = -1;
					} else if (xml.NodeType == XmlNodeType.EndElement) {
						string elname = xml.Name.ToLower();
						if (elname == "para" || elname == "param") {
							if (lastLinePos == -1)
								lastLinePos = ret.Length;
							ret.Append("<span size=\"2000\">\n\n</span>");
						}
					} else if (xml.NodeType == XmlNodeType.Text) {
						string txt = xml.Value.Replace ("\r","").Replace ("\n"," ");
						if (lastLinePos != -1)
							txt = txt.TrimStart (' ');
						
						// Remove duplcate spaces.
						int len;
						do {
							len = txt.Length;
							txt = txt.Replace ("  ", " ");
						} while (len != txt.Length);
						
						txt = GLib.Markup.EscapeText (txt);
						ret.Append(txt);
						lastLinePos = -1;
					}
				} while (xml.Read ());
				if (lastLinePos != -1)
					ret.Remove (lastLinePos, ret.Length - lastLinePos);
			} catch (Exception ex) {
				LoggingService.LogError (ex.ToString ());
				return doc;
			}
			return ret.ToString ();
		}

		#region IOverloadedCompletionData implementation 
		
		public IEnumerable<ICompletionData> GetOverloads ()
		{
			if (overloads == null)
				return new ICompletionData[0];
			else
				return overloads.Values;
		}
		
		public bool HasOverloads {
			get { return overloads != null; }
		}
		
		public void AddOverload (MemberCompletionData overload)
		{
			if (overloads == null)
				overloads = new Dictionary<string, ICompletionData> ();
			
			if (!overload.member.IsObsolete)
				MarkObsolete = false;
			
			string description = overload.Description;
			if (description != this.description || !overloads.ContainsKey (description))
				overloads[description] = overload;
		}
		
		#endregion 
		
		
		public bool Sink {
			get {
				 return MarkObsolete;
			}
		}
	}
}
