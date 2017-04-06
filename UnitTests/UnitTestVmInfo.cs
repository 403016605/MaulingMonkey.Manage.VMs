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

using MaulingMonkey.Manage.VMs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace UnitTests {
	[TestClass] public class UnitTestVmInfo {
		VirtualBox VBox = new VirtualBox();

		[TestMethod] public void ListVms_Props_Consistent() {
			if (VBox.VBoxManagePath == null) { Assert.Inconclusive("Unable to test if Has_VMs without VBoxManage"); return; }

			var vms = VBox.ListVms().ToList();
			if (vms.Count == 0) { Assert.Inconclusive("Unable to test if Has_VMs without VBoxManage"); return; }

			foreach (var vm in vms) {
				var vmProps = VBox.VmInfoDictionary(vm);
				Assert.IsTrue(vmProps.ContainsKey("name"),    "VM properties should include 'name' field");
				Assert.IsTrue(vmProps.ContainsKey("chipset"), "VM properties should include 'chipset' field");
				Assert.IsTrue(vmProps.ContainsKey("ostype"),  "VM properties should include 'ostype' field");

				Assert.IsFalse(vmProps.ContainsKey("nonexistant"), "VM properties shouldn't include 'nonexistant' field");
				Assert.IsFalse(vmProps.ContainsKey("")           , "VM properties shouldn't include blank keys");

				Assert.AreEqual(vm.Name,                             vmProps["name"], "VM name property should match name from parsed list");
				Assert.AreEqual(vm.Guid.TrimStart('{').TrimEnd('}'), vmProps["UUID"], "VM UUID property should match guid from parsed list (minus {}s)");
			}
		}
	}
}
