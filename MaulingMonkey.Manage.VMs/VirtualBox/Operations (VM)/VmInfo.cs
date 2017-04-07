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
		static readonly Regex reShowVmInfo_SimpleKeyValue = new Regex(@"^   ( ""(?<key>[^=]+)"" | (?<key>[^=]+) )   =   ( ""(?<value>.*)"" | (?<value>\d*) )   $", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

		public IEnumerable<KeyValuePair<string,string>> VmInfo(VmId vm) {
			if (VBoxManagePath == null) throw new MissingToolException("VBoxManage", VBoxManage_Paths);

			var results = new List<KeyValuePair<string,string>>();
			var ambiguous = new List<string>();

			// Parse the first half of the key/value pairs - e.g. before the possibly multi-line description key/value pair.
			var preDescription = true;
			var exit = Proc.ExecIn(null, VBoxManagePath, "showvminfo --machinereadable "+vm, stdout => {
				if (preDescription) {
					var m = reShowVmInfo_SimpleKeyValue.Match(stdout);
					if      (m.Success)                            results.Add(new KeyValuePair<string, string>(m.Groups["key"].Value, m.Groups["value"].Value));
					else if (!stdout.StartsWith("description=\"")) throw new ToolResultSyntaxException("VBoxManage showvminfo --machinereadable ...", "Failed to match line before 'description' field with simple key/value pair");
					else                                           preDescription = false;
				}
				if (!preDescription) ambiguous.Add(stdout);
			}, stderr => { }, ProcessWindowStyle.Hidden);

			var revResults = new List<KeyValuePair<string,string>>();

			// Parse the second half of the key/value pairs - e.g. everything after the possibly multi-line description key/value pair.
			for (int i=ambiguous.Count; i-->0;) {
				var stdout = ambiguous[i];
				var m = reShowVmInfo_SimpleKeyValue.Match(stdout);
				if (m.Success) revResults.Add(new KeyValuePair<string, string>(m.Groups["key"].Value, m.Groups["value"].Value));
				else {
					ambiguous.RemoveRange(i+1, ambiguous.Count-i-1);
					break;
				}
			}

			// Hopefully 'ambiguous' is now only the description="..." field.  however, this is fundamentlaly an ambiguous parsing problem.  Boo!
			if (ambiguous.Count > 0) {
				var firstLine = ambiguous[0];
				var prefix = "description=\"";
				if (!firstLine.StartsWith(prefix)) throw new ToolResultSyntaxException("VBoxManage showvminfo --machinereadable ...", "Remaining 'ambiguous' lines should be the start of a description - instead got '"+firstLine+"'");
				ambiguous[0] = firstLine = firstLine.Substring(prefix.Length); // Trim prefix description="

				var lastLine  = ambiguous[ambiguous.Count-1];
				if (!lastLine.EndsWith("\"")) throw new ToolResultSyntaxException("VBoxManage showvminfo --machinereadable ...", "Remaining 'ambiguous' lines should end with a quote terminating the description field - instead got '"+lastLine+"'");
				ambiguous[ambiguous.Count-1] = lastLine = lastLine.Substring(0, lastLine.Length-1); // Trim final quote mark

				results.Add(new KeyValuePair<string, string>("description", string.Join("\n", ambiguous)));
			}


			var needCapacity = results.Count + revResults.Count;
			if (results.Capacity < needCapacity) results.Capacity = needCapacity;
			for (int i=revResults.Count; i-->0;) results.Add(revResults[i]);

			return results;
		}

		public IEnumerable<KeyValuePair<string,string>> TryVmInfo(VmId vm) {
			try { return VmInfo(vm); }
			catch (MissingToolException     ) { return new KeyValuePair<string,string>[0]; }
			catch (ToolResultSyntaxException) { return new KeyValuePair<string,string>[0]; }
		}

		public Dictionary<string,string> VmInfoDictionary   (VmId vm) { var d = new Dictionary<string,string>(); foreach (var kv in VmInfo(vm)   ) d.Add(kv.Key, kv.Value); return d; }
		public Dictionary<string,string> TryVmInfoDictionary(VmId vm) { var d = new Dictionary<string,string>(); foreach (var kv in TryVmInfo(vm)) d.Add(kv.Key, kv.Value); return d; }
	}
}
