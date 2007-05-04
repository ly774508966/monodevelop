//
// RefactoryCommands.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2006 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Threading;

using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components.Commands;
using MonoDevelop.Projects.Text;
using MonoDevelop.Projects.Parser;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Ide.Gui.Search;
using MonoDevelop.Projects.CodeGeneration;
using MonoDevelop.Ide.Gui.Dialogs;

namespace MonoDevelop.Ide.Commands
{
	public enum RefactoryCommands
	{
		CurrentRefactoryOperations
	}
	
	public class CurrentRefactoryOperationsHandler: CommandHandler
	{
		protected override void Run (object data)
		{
			RefactoryOperation del = (RefactoryOperation) data;
			if (del != null)
				del ();
		}
		
		protected override void Update (CommandArrayInfo ainfo)
		{
			Document doc = IdeApp.Workbench.ActiveDocument;
			if (doc != null) {
				ITextBuffer editor = IdeApp.Workbench.ActiveDocument.GetContent <ITextBuffer>();
				if (editor != null) {
					bool added = false;
					int line, column;
					
					editor.GetLineColumnFromPosition (editor.CursorPosition, out line, out column);
					IParserContext ctx;
					if (doc.Project != null)
						ctx = IdeApp.ProjectOperations.ParserDatabase.GetProjectParserContext (doc.Project);
					else
						ctx = IdeApp.ProjectOperations.ParserDatabase.GetFileParserContext (doc.FileName);
					
					// Look for an identifier at the cursor position
					
					string id = editor.SelectedText;
					if (id.Length == 0) {
						IExpressionFinder finder = Services.ParserService.GetExpressionFinder (editor.Name);
						if (finder == null)
							return;
						id = finder.FindFullExpression (editor.Text, editor.CursorPosition).Expression;
						if (id == null) return;
					}
					
					ILanguageItem item = ctx.ResolveIdentifier (id, line, column, editor.Name, null);
					if (item != null) {
						CommandInfo ci = BuildRefactoryMenuForItem (ctx, item);
						if (ci != null) {
							ainfo.Add (ci, null);
							added = true;
						}
					}
					
					// Look for the enclosing language item
					ILanguageItem eitem = ctx.GetEnclosingLanguageItem (line, column, editor);
					if (eitem != null && !eitem.Equals (item)) {
						CommandInfo ci = BuildRefactoryMenuForItem (ctx, eitem);
						if (ci != null) {
							ainfo.Add (ci, null);
							added = true;
						}
					}
					
					if (added)
						ainfo.AddSeparator ();
				}
			}
		}
		
		CommandInfo BuildRefactoryMenuForItem (IParserContext ctx, ILanguageItem item)
		{
			Refactorer refactorer = new Refactorer (ctx, item);
			CommandInfoSet ciset = new CommandInfoSet ();
			string txt;
			
			if ((item is LocalVariable) || (item is IParameter) || 
			    (((item is IMember) || (item is IClass)) && IdeApp.ProjectOperations.CanJumpToDeclaration (item))) {
				// we can rename local variables, method params, or class/member-variables if we can jump to their declarations
				ciset.CommandInfos.Add (GettextCatalog.GetString ("_Rename"), new RefactoryOperation (refactorer.Rename));
			}
			
			if (IdeApp.ProjectOperations.CanJumpToDeclaration (item))
				ciset.CommandInfos.Add (GettextCatalog.GetString ("_Go to declaration"), new RefactoryOperation (refactorer.GoToDeclaration));
			
			if ((item is IMember) && !(item is IClass))
				ciset.CommandInfos.Add (GettextCatalog.GetString ("_Find references"), new RefactoryOperation (refactorer.FindReferences));
			
			if (item is IClass) {
				IClass cls = (IClass) item;
				
				if (cls.ClassType == ClassType.Interface)
					txt = GettextCatalog.GetString ("Interface {0}", item.Name);
				else
					txt = GettextCatalog.GetString ("Class {0}", item.Name);
				
				if (cls.BaseTypes.Count > 0) {
					foreach (IReturnType rt in cls.BaseTypes) {
						IClass bc = ctx.GetClass (rt.FullyQualifiedName, null, true, true);
						if (bc != null && bc.ClassType != ClassType.Interface && IdeApp.ProjectOperations.CanJumpToDeclaration (bc)) {
							ciset.CommandInfos.Add (GettextCatalog.GetString ("Go to _base"), new RefactoryOperation (refactorer.GoToBase));
						}
					}
				}
				
				if (cls.ClassType == ClassType.Interface) {
					// An interface is selected, so just need to provide these 2 submenu items
					ciset.CommandInfos.Add (GettextCatalog.GetString ("Implement Interface (implicit)"), new RefactoryOperation (refactorer.ImplementImplicitInterface));
					ciset.CommandInfos.Add (GettextCatalog.GetString ("Implement Interface (explicit)"), new RefactoryOperation (refactorer.ImplementExplicitInterface));
				} else if (cls.BaseTypes.Count > 0) {
					// Class might have interfaces... offer to implement them
					CommandInfoSet impset = new CommandInfoSet ();
					CommandInfoSet expset = new CommandInfoSet ();
					bool added = false;
					
					foreach (IReturnType rt in cls.BaseTypes) {
						IClass iface = ctx.GetClass (rt.FullyQualifiedName, null, true, true);
						if (iface != null && iface.ClassType == ClassType.Interface) {
							Refactorer ifaceRefactorer = new Refactorer (ctx, iface);
							
							impset.CommandInfos.Add (iface.Name, new RefactoryOperation (ifaceRefactorer.ImplementImplicitInterface));
							expset.CommandInfos.Add (iface.Name, new RefactoryOperation (ifaceRefactorer.ImplementExplicitInterface));
							added = true;
						}
					}
					
					if (added) {
						impset.Text = GettextCatalog.GetString ("Implement Interface (implicit)");
						ciset.CommandInfos.Add (impset, null);
						
						expset.Text = GettextCatalog.GetString ("Implement Interface (explicit)");
						ciset.CommandInfos.Add (expset, null);
					}
				}
				
				ciset.CommandInfos.Add (GettextCatalog.GetString ("Find _derived classes"), new RefactoryOperation (refactorer.FindDerivedClasses));
				ciset.CommandInfos.Add (GettextCatalog.GetString ("_Find references"), new RefactoryOperation (refactorer.FindReferences));
			} else if (item is IField) {
				txt = GettextCatalog.GetString ("Field {0} : {1}", item.Name, ((IField)item).ReturnType.Name);
				AddRefactoryMenuForClass (ctx, ciset, ((IField)item).ReturnType.FullyQualifiedName);
			} else if (item is IProperty) {
				txt = GettextCatalog.GetString ("Property {0} : {1}", item.Name, ((IProperty)item).ReturnType.Name);
				AddRefactoryMenuForClass (ctx, ciset, ((IProperty)item).ReturnType.FullyQualifiedName);
			} else if (item is IEvent)
				txt = GettextCatalog.GetString ("Event {0}", item.Name);
			else if (item is IMethod)
				txt = GettextCatalog.GetString ("Method {0}", item.Name);
			else if (item is IIndexer)
				txt = GettextCatalog.GetString ("Indexer {0}", item.Name);
			else if (item is IParameter) {
				txt = GettextCatalog.GetString ("Parameter {0}", item.Name);
				AddRefactoryMenuForClass (ctx, ciset, ((IParameter)item).ReturnType.FullyQualifiedName);
			} else if (item is LocalVariable) {
				LocalVariable var = (LocalVariable) item;
				AddRefactoryMenuForClass (ctx, ciset, var.ReturnType.FullyQualifiedName);
				txt = GettextCatalog.GetString ("Variable {0}", item.Name);
			} else
				return null;
			
			if (item is IMember) {
				IClass cls = ((IMember)item).DeclaringType;
				if (cls != null) {
					CommandInfo ci = BuildRefactoryMenuForItem (ctx, cls);
					if (ci != null)
						ciset.CommandInfos.Add (ci, null);
				}
			} 

			ciset.Text = txt;
			return ciset;
		}
		
		void AddRefactoryMenuForClass (IParserContext ctx, CommandInfoSet ciset, string className)
		{
			IClass cls = ctx.GetClass (className, true, true);
			if (cls != null) {
				CommandInfo ci = BuildRefactoryMenuForItem (ctx, cls);
				if (ci != null)
					ciset.CommandInfos.Add (ci, null);
			}
		}
		
		delegate void RefactoryOperation ();
	}
	
	class Refactorer
	{
		ILanguageItem item;
		IParserContext ctx;
		MemberReferenceCollection references;
		ISearchProgressMonitor monitor;
		
		public Refactorer (IParserContext ctx, ILanguageItem item)
		{
			this.item = item;
			this.ctx = ctx;
		}
		
		public void GoToDeclaration ()
		{
			IdeApp.ProjectOperations.JumpToDeclaration (item);
		}
		
		public void FindReferences ()
		{
			monitor = IdeApp.Workbench.ProgressMonitors.GetSearchProgressMonitor (true);
			Thread t = new Thread (new ThreadStart (FindReferencesThread));
			t.IsBackground = true;
			t.Start ();
		}
		
		void FindReferencesThread ()
		{
			using (monitor) {
				CodeRefactorer refactorer = IdeApp.ProjectOperations.CodeRefactorer;
				
				if (item is IMember) {
					IMember member = (IMember)item;
					if (member.IsPrivate || (!member.IsProtectedOrInternal && !member.IsPublic)) { // private is filled only in keyword case
						// look in project to be partial classes safe
						references = refactorer.FindMemberReferences (monitor, member.DeclaringType, member, RefactoryScope.Project);
					}
					else {
						// for all other types look in solution cause internal members can be used in friend assemblies
						references = refactorer.FindMemberReferences (monitor, member.DeclaringType, member, RefactoryScope.Solution);
					}
				} else if (item is IClass) {
					references = refactorer.FindClassReferences (monitor, (IClass)item, RefactoryScope.Solution);
				}
				
				if (references != null) {
					foreach (MemberReference mref in references) {
						monitor.ReportResult (mref.FileName, mref.Line, mref.Column, mref.TextLine);
					}
				}
			}
		}
		
		public void GoToBase ()
		{
			IClass cls = (IClass) item;
			if (cls == null) return;
			
			if (cls.BaseTypes != null) {
				foreach (IReturnType bc in cls.BaseTypes) {
					IClass bcls = ctx.GetClass (bc.FullyQualifiedName, true, true);
					if (bcls != null && bcls.ClassType != ClassType.Interface && bcls.Region != null) {
						IdeApp.Workbench.OpenDocument (bcls.Region.FileName, bcls.Region.BeginLine, bcls.Region.BeginColumn, true);
						return;
					}
				}
			}
		}
		
		public void FindDerivedClasses ()
		{
			monitor = IdeApp.Workbench.ProgressMonitors.GetSearchProgressMonitor (true);
			Thread t = new Thread (new ThreadStart (FindDerivedThread));
			t.IsBackground = true;
			t.Start ();
		}
		
		void FindDerivedThread ()
		{
			using (monitor) {
				IClass cls = (IClass) item;
				if (cls == null) return;
			
				IClass[] classes = IdeApp.ProjectOperations.CodeRefactorer.FindDerivedClasses (cls);
				foreach (IClass sub in classes) {
					if (sub.Region != null)
						monitor.ReportResult (sub.Region.FileName, sub.Region.BeginLine, sub.Region.BeginColumn, sub.FullyQualifiedName);
				}
			}
		}
		
		void ImplementInterface (bool explicitly)
		{
			// FIXME: implement me
		}
		
		public void ImplementImplicitInterface ()
		{
			ImplementInterface (false);
		}
		
		public void ImplementExplicitInterface ()
		{
			ImplementInterface (true);
		}
		
		public void Rename ()
		{
			RenameItemDialog dialog = new RenameItemDialog (ctx, item);
			dialog.Show ();
		}
	}
}
