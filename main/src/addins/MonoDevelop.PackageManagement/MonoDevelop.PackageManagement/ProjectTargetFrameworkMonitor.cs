﻿//
// ProjectTargetFrameworkMonitor.cs
//
// Author:
//       Matt Ward <matt.ward@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (http://xamarin.com)
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

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using System.Linq;
using MonoDevelop.Projects;

namespace MonoDevelop.PackageManagement
{
	public class ProjectTargetFrameworkMonitor
	{
		IPackageManagementProjectService projectService;
		ISolution solution;
		List<IDotNetProject> projects = new List<IDotNetProject> ();

		public ProjectTargetFrameworkMonitor (IPackageManagementProjectService projectService)
		{
			this.projectService = projectService;

			projectService.SolutionLoaded += SolutionLoaded;
			projectService.SolutionUnloaded += SolutionUnloaded;
			projectService.ProjectReloaded += ProjectReloaded;
		}

		public event EventHandler<ProjectTargetFrameworkChangedEventArgs> ProjectTargetFrameworkChanged;

		protected virtual void OnProjectTargetFrameworkChanged (IDotNetProject project)
		{
			var handler = ProjectTargetFrameworkChanged;
			if (handler != null) {
				handler (this, new ProjectTargetFrameworkChangedEventArgs (project));
			}
		}

		void SolutionUnloaded (object sender, EventArgs e)
		{
			foreach (IDotNetProject project in projects) {
				project.Modified -= ProjectModified;
			}
			projects.Clear ();

			solution.ProjectAdded -= ProjectAdded;
			solution = null;
		}

		void SolutionLoaded (object sender, EventArgs e)
		{
			solution = projectService.OpenSolution;
			solution.ProjectAdded += ProjectAdded;
			projects = projectService.GetOpenProjects ().ToList ();

			foreach (IDotNetProject project in projects) {
				project.Modified += ProjectModified;
			}
		}

		void ProjectAdded (object sender, DotNetProjectEventArgs e)
		{
			e.Project.Modified += ProjectModified;
			projects.Add (e.Project);
		}

		void ProjectModified (object sender, ProjectModifiedEventArgs e)
		{
			if (e.IsTargetFramework ()) {
				OnProjectTargetFrameworkChanged (e.Project);
			}
		}

		void ProjectReloaded (object sender, ProjectReloadedEventArgs e)
		{
			if (HasTargetFrameworkChanged (e.NewProject, e.OldProject)) {
				OnProjectTargetFrameworkChanged (e.NewProject);
			}
		}

		static bool HasTargetFrameworkChanged (IDotNetProject newProject, IDotNetProject oldProject)
		{
			if (newProject.TargetFrameworkMoniker != null) {
				return !newProject.TargetFrameworkMoniker.Equals (oldProject.TargetFrameworkMoniker);
			}
			return false;
		}
	}
}
