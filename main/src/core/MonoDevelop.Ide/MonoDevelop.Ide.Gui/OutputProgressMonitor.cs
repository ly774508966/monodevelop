// OutputProgressMonitor.cs
//
// Author:
//   Lluis Sanchez Gual <lluis@novell.com>
//   Mike Krüger <mike@icsharpcode.net>
//
// Copyright (c) 2007 Novell, Inc (http://www.novell.com)
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
//

using System;
using System.Collections;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using System.Text;

using MonoDevelop.Core;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Core.Gui;
using MonoDevelop.Ide.Gui.Pads;
using MonoDevelop.Core.Execution;
using MonoDevelop.Core.Gui.ProgressMonitoring;

using Gtk;
using Pango;

namespace MonoDevelop.Ide.Gui
{	
	internal class OutputProgressMonitor : BaseProgressMonitor, IConsole
	{
		DefaultMonitorPad outputPad;
		event EventHandler stopRequested;
		
		LogTextWriter logger = new LogTextWriter ();
		
		public OutputProgressMonitor (DefaultMonitorPad pad, string title, string icon)
		{
			pad.AsyncOperation = this.AsyncOperation;
			outputPad = pad;
			outputPad.BeginProgress (title);
			
			//using the DefaultMonitorPad's method here to make sure we don't mess with the remoted dispatch 
			//mechanisms *at all* (even just accessing a field from an anon delegate invokes remoting) 
			//We're hooking up our own logger to avoid cost of context switching via remoting through the 
			//AsyncDispatch of base class's WriteLogInternal
			//HORRIBLE HACK:To get it properly deteched, we have to replace the actual logger.
			logger.TextWritten += outputPad.WriteText;
			((LogTextWriter) base.Log).TextWritten += outputPad.WriteText;
		}
		
		[FreeDispatch]
		public override void BeginTask (string name, int totalWork)
		{
			if (outputPad == null) throw GetDisposedException ();
			outputPad.BeginTask (name, totalWork);
			base.BeginTask (name, totalWork);
		}
		
		[FreeDispatch]
		public override void EndTask ()
		{
			if (outputPad == null) throw GetDisposedException ();
			outputPad.EndTask ();
			base.EndTask ();
		}
		
		protected override void OnCompleted ()
		{
			if (outputPad == null) throw GetDisposedException ();
			outputPad.WriteText ("\n");
			
			foreach (string msg in SuccessMessages)
				outputPad.WriteText (msg + "\n");
			
			foreach (string msg in Warnings)
				outputPad.WriteText (msg + "\n");
			
			foreach (string msg in Errors)
				outputPad.WriteError (msg + "\n");
			
			outputPad.EndProgress ();
			base.OnCompleted ();
			
			outputPad = null;
		}
		
		Exception GetDisposedException ()
		{
			return new InvalidOperationException ("Output progress monitor already disposed.");
		}
		
		protected override void OnCancelRequested ()
		{
			base.OnCancelRequested ();
			if (stopRequested != null)
				stopRequested (this, null);
		}
		
		TextReader IConsole.In {
			[FreeDispatch]
			get { return new StringReader (""); }
		}
		
		TextWriter IConsole.Out {
			[FreeDispatch]
			get { return logger; }
		}
		
		TextWriter IConsole.Error {
			[FreeDispatch]
			get { return logger; }
		} 
		
		bool IConsole.CloseOnDispose {
			[FreeDispatch]
			get { return false; }
		}
		
		event EventHandler IConsole.CancelRequested {
			[FreeDispatch]
			add { stopRequested += value; }
			[FreeDispatch]
			remove { stopRequested -= value; }
		}
	}
}
