// 
// MacUpdater.cs
//  
// Author:
//       Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
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
using System.Linq;
using System.IO;
using System.Text;
using MonoDevelop.Core;
using System.Net;
using MonoDevelop.Core.Gui;
using System.Collections.Generic;

namespace MonoDevelop.Platform
{

	public static class AppUpdater
	{
		const int formatVersion = 1;
		const string updateAutoPropertyKey = "AppUpdater.CheckAutomatically";
		
		static UpdateInfo[] updateInfos;
		
		//FIXME: populate from an extension point
		public static UpdateInfo[] DefaultUpdateInfos {
			get {
				if (updateInfos == null) {
					var list = new List<UpdateInfo> ();
					var files = new string[] {
						"/Developer/MonoTouch/updateinfo",
						"/Library/Frameworks/Mono.framework/Versions/Current/updateinfo",
						Path.GetDirectoryName (typeof (MacPlatform).Assembly.Location) + "/../../../updateinfo",
					}.Where (File.Exists);
					
					foreach (string file in files) {
						try {
							list.Add (UpdateInfo.FromFile (file));
						} catch (Exception ex) {
							LoggingService.LogError ("Error reading update info file '" + file + "'", ex);
						}
					}
					
					//FIXME: workaround for older 2.4.x Mono not having updateinfo.
					// Remove this when MD launch script forces Mono 2.6
					if (!File.Exists ("/Library/Frameworks/Mono.framework/Versions/Current/updateinfo"))
						list.Add (new UpdateInfo (new Guid ("432959f9-ce1b-47a7-94d3-eb99cb2e1aa8=0"), 0));
					
					updateInfos = list.ToArray ();
				}
				
				return updateInfos;
			}
		}
		
		public static bool CheckAutomatically {
			get {
				return PropertyService.Get<bool> (updateAutoPropertyKey, true);
			}
			set {
				PropertyService.Set (updateAutoPropertyKey, value);
			}
		}
		
		public static void RunCheck (bool automatic)
		{
			RunCheck (DefaultUpdateInfos, automatic);
		}
		
		public static void RunCheck (UpdateInfo[] updateInfos, bool automatic)
		{
			if (updateInfos == null || updateInfos.Length == 0 || (automatic && !CheckAutomatically))
				return;
			
			var query = new StringBuilder ("http://go-mono.com/macupdate/update?v=");
			query.Append (formatVersion);
			foreach (var info in updateInfos)
				query.AppendFormat ("&{0}={1}", info.AppId, info.VersionId);
			
			if (!string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("MONODEVELOP_UPDATER_TEST")))
				query.Append ("&test=1");
			
			var request = (HttpWebRequest) WebRequest.Create (query.ToString ());
			
			//FIXME: use IfModifiedSince
			//request.IfModifiedSince = somevalue;
			
			request.BeginGetResponse (delegate (IAsyncResult ar) {
				ReceivedResponse (request, ar, automatic);
			}, null);
		}
		
		static void ReceivedResponse (HttpWebRequest request, IAsyncResult ar, bool automatic)
		{
			try {
				using (var response = (HttpWebResponse) request.EndGetResponse (ar)) {
					var encoding = Encoding.GetEncoding (response.CharacterSet);
					using (var reader = new StreamReader (response.GetResponseStream(), encoding)) {
						var doc = System.Xml.Linq.XDocument.Load (reader);
						var updates = (from x in doc.Root.Elements ("Application")
							let first = x.Elements ("Update").First ()
							select new Update () {
								Name = x.Attribute ("name").Value,
								Url = first.Attribute ("url").Value,
								Version = first.Attribute ("version").Value,
								Date = DateTime.Parse (first.Attribute ("date").Value),
								Releases = x.Elements ("Update").Select (y => new Release () {
									Version = y.Attribute ("version").Value,
									Date = DateTime.Parse (y.Attribute ("date").Value),
									Notes = y.Value
								}).ToList ()
							}).ToList ();
						
						if (!automatic || (updates != null && updates.Count > 0)) {
							Gtk.Application.Invoke (delegate {
								MessageService.ShowCustomDialog (new UpdateDialog (updates));
							});
						}
					}
				}
			} catch (WebException ex) {
				LoggingService.LogError ("Error retrieving update information", ex);
				if (!automatic)
					MessageService.ShowException (ex, GettextCatalog.GetString ("Error retrieving update information"));
			} catch (Exception ex) {
				LoggingService.LogError ("Error retrieving update information", ex);
				if (!automatic)
					MessageService.ShowException (ex, GettextCatalog.GetString ("Error retrieving update information"));
			}
		}
		
		public class Update
		{
			public string Name;
			public string Url;
			public string Version;
			public DateTime Date;
			public List<Release> Releases;
		}
		
		public class Release
		{
			public string Version;
			public DateTime Date;
			public string Notes;
		}
		
		public class UpdateInfo
		{
			UpdateInfo ()
			{
			}
			
			public UpdateInfo (Guid appId, long versionId)
			{
				this.AppId = appId;
				this.VersionId = versionId;
			}
			
			public readonly Guid AppId;
			public readonly long VersionId;
			
			public static UpdateInfo FromFile (string fileName)
			{
				using (var f = File.OpenText (fileName)) {
					var s = f.ReadLine ();
					var parts = s.Split (' ');
					return new UpdateInfo (new Guid (parts[0]), long.Parse (parts[1]));
				}
			}
		}
	}
}
