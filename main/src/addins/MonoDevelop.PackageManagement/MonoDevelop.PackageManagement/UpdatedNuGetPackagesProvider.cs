﻿//
// UpdatedNuGetPackagesProvider.cs
//
// Author:
//       Matt Ward <matt.ward@xamarin.com>
//
// Copyright (c) 2016 Xamarin Inc. (http://xamarin.com)
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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MonoDevelop.Core;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol.VisualStudio;

namespace MonoDevelop.PackageManagement
{
	internal class UpdatedNuGetPackagesProvider
	{
		List<SourceRepository> sourceRepositories;
		IDotNetProject dotNetProject;
		NuGetProject project;
		CancellationToken cancellationToken;
		UpdatedNuGetPackagesInProject updatedPackagesInProject;
		List<PackageIdentity> updatedPackages = new List<PackageIdentity> ();

		public UpdatedNuGetPackagesProvider (
			IDotNetProject project,
			IMonoDevelopSolutionManager solutionManager,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			this.dotNetProject = project;
			this.project = new MonoDevelopNuGetProjectFactory ().CreateNuGetProject (project);

			var sourceRepositoryProvider = solutionManager.CreateSourceRepositoryProvider ();
			this.sourceRepositories = sourceRepositoryProvider.GetRepositories ().ToList ();

			this.cancellationToken = cancellationToken;
		}

		public IEnumerable<PackageIdentity> UpdatedPackages {
			get { return updatedPackages; }
		}

		public UpdatedNuGetPackagesInProject UpdatedPackagesInProject {
			get {
				if (updatedPackagesInProject == null) {
					updatedPackagesInProject = new UpdatedNuGetPackagesInProject (dotNetProject, updatedPackages);
				}
				return updatedPackagesInProject;
			}
		}

		public async Task FindUpdatedPackages ()
		{
			var installedPackages = await project.GetInstalledPackagesAsync (cancellationToken);

			foreach (PackageReference packageReference in installedPackages) {
				if (cancellationToken.IsCancellationRequested) {
					break;
				}

				var tasks = sourceRepositories
					.Select (sourceRepository => GetUpdates (sourceRepository, packageReference))
					.ToList ();

				var ignored = tasks
					.Select (task => task.ContinueWith (LogError, TaskContinuationOptions.OnlyOnFaulted))
					.ToArray ();

				var updatedPackage = (await Task.WhenAll (tasks))
					.Where (package => package != null)
					.OrderByDescending (package => package.Version)
					.FirstOrDefault ();

				if (updatedPackage != null) {
					updatedPackages.Add (updatedPackage);
				}
			}
		}

		async Task<PackageIdentity> GetUpdates (SourceRepository sourceRepository, PackageReference packageReference)
		{
			var metadataResource = await sourceRepository.GetResourceAsync<UIMetadataResource> (cancellationToken);

			if (metadataResource == null)
				return null;

			var packages = await metadataResource.GetMetadata (
				packageReference.PackageIdentity.Id,
				includePrerelease: packageReference.PackageIdentity.Version.IsPrerelease,
				includeUnlisted: false,
				token: cancellationToken);

			var package = packages
				.Where (p => IsPackageVersionAllowed (p, packageReference))
				.OrderByDescending (p => p.Identity.Version).FirstOrDefault ();
			if (package == null)
				return null;

			if (package.Identity.Version > packageReference.PackageIdentity.Version)
				return package.Identity;

			return null;
		}

		void LogError (Task<PackageIdentity> task)
		{
			LoggingService.LogError ("Check for updates error.", task.Exception);
		}

		bool IsPackageVersionAllowed (UIPackageMetadata package, PackageReference packageReference)
		{
			if (!packageReference.HasAllowedVersions)
				return true;

			return packageReference.AllowedVersions.Satisfies (package.Identity.Version);
		}
	}
}

