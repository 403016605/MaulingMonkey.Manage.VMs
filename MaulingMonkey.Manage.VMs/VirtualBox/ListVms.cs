// Copyright 2017 MaulingMonkey
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MaulingMonkey.Manage.VMs {
	partial class VirtualBox {
		// "Ubuntu Server 64-bit" {88591c2d-cc79-4acd-a632-8ab4057db351}
		// "Ubuntu Server 32-bit" {e97242cc-012f-47f9-a2f0-50a5f4fc4dd1}
		// "OpenBSD 64-bit" {9d5f090c-2aa1-4aec-a818-329793731299}
		// "OpenBSD 32-bit" {9adfb725-1a30-4bd4-b1b1-017d9eb5e08b}
		// "FreeBSD 32-bit" {4ffc5f71-3a7b-44f5-a574-a4aaa607112e}
		// "FreeBSD 64-bit" {399c1adb-e6b2-43ed-b4a1-c337320bfde2}
		// "Ubuntu Desktop" {5a497ace-b8b3-400f-a576-e6a257322a41}
		static readonly Regex reVbmListVms = new Regex(@"^""(?<name>.*)"" (?<guid>\{[-0-9a-fA-F]+\})$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

		public struct VmNameId {
			public string Name, Guid;
			public bool IsNull { get { return Name == null && Guid == null; } }
		}

		public IEnumerable<VmNameId> ListVms() {
			if (VBoxManagePath == null) throw new MissingToolException("VBoxManage", VBoxManage_Paths);

			var results = new List<VmNameId>();
			var exit = Proc.ExecIn(null, VBoxManagePath, "list vms", stdout => {
				var m = reVbmListVms.Match(stdout);
				if (!m.Success) throw new ToolResultSyntaxException("VBoxManage list vms", "Failed to match expected pattern: \"name\" {guid}");
				results.Add(new VmNameId() { Name = m.Groups["name"].Value, Guid = m.Groups["guid"].Value });
			}, stderr => { }, ProcessWindowStyle.Hidden);
			return results;
		}

		public IEnumerable<VmNameId> TryListVms() {
			try { return ListVms(); }
			catch (MissingToolException     ) { return new VmNameId[0]; }
			catch (ToolResultSyntaxException) { return new VmNameId[0]; }
		}
	}
}
