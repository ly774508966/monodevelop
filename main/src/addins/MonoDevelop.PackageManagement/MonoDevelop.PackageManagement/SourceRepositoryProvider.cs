﻿//
// SourceRepositoryProvider.cs
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

using System;
using System.Collections.Generic;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol.Core.v2;
using NuGet.Protocol.Core.v3;
using NuGet.Protocol.VisualStudio;

namespace MonoDevelop.PackageManagement
{
	internal static class SourceRepositoryProviderFactory
	{
		public static ISourceRepositoryProvider CreateSourceRepositoryProvider ()
		{
			var settings = Settings.LoadDefaultSettings (null, null, null);
			return CreateSourceRepositoryProvider (settings);
		}

		public static ISourceRepositoryProvider CreateSourceRepositoryProvider (ISettings settings)
		{
			return new SourceRepositoryProvider (settings, GetResourceProviders ());
		}

		static IEnumerable<Lazy<INuGetResourceProvider>> GetResourceProviders ()
		{
			yield return new Lazy<INuGetResourceProvider> (() => new UISearchResourceV2Provider ());
			yield return new Lazy<INuGetResourceProvider> (() => new UISearchResourceV3Provider ());
			yield return new Lazy<INuGetResourceProvider>(() => new UIMetadataResourceV2Provider ());
			yield return new Lazy<INuGetResourceProvider>(() => new UIMetadataResourceV3Provider ());

			foreach (var provider in Repository.Provider.GetCoreV2 ()) {
				yield return provider;
			}

			foreach (var provider in Repository.Provider.GetCoreV3 ()) {
				yield return provider;
			}
		}
	}
}

