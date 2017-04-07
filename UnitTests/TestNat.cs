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
	[TestClass] public class TestNat {
		VirtualBox VBox = new VirtualBox();

		[TestMethod] public void ModifyVm_NatForward_Delete() {
			if (VBox.VBoxManagePath == null) { Assert.Inconclusive("Unable to test without VBoxManage"); return; }
			var vms = VBox.ListVms().ToList();
			if (vms.Count == 0) { Assert.Inconclusive("Unable to test without VMs"); return; }

			bool foundVmToTest = false;
			foreach (var original in vms) {
				string desc;
				if (!VBox.VmInfoDictionary(original).TryGetValue("description", out desc) || !desc.Contains("clone testing vm")) continue;
				foundVmToTest = true;

				var clone = VBox.CloneVm(original);
				Assert.IsFalse(clone.IsNull);
				try {
					VBox.ModifyVmNatPortForward(clone, 1, "ssh", VirtualBox.PortType.Tcp, "127.0.0.2", 3022, null, 22);
					try {
						VBox.ModifyVmNatPortForward(clone, 1, "ssh", VirtualBox.PortType.Tcp, "127.0.0.2", 3022, null, 22);
						Assert.Fail("Rule collision should've thrown");
					} catch (VmManagementException) { }

					VBox.ModifyVmDeleteNatPortForward(clone, 1, "ssh");
					try {
						VBox.ModifyVmDeleteNatPortForward(clone, 1, "ssh");
						Assert.Fail("Rule should've already been deleted");
					} catch (VmManagementException) { }
				}
				finally { Assert.IsTrue(VBox.TryDeleteVm(clone)); }
			}

			if (!foundVmToTest) Assert.Inconclusive("Unable to test:  No VMs tagged 'clone testing vm' in their descriptions.");
		}
	}
}
