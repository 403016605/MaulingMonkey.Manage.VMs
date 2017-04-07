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
using System;
using System.Linq;

namespace UnitTests {
	[TestClass] public class TestCloneDelete {
		VirtualBox VBox = new VirtualBox();

		[TestMethod] public void Clone_Delete_VMs() {
			if (VBox.VBoxManagePath == null) { Assert.Inconclusive("Unable to test without VBoxManage"); return; }

			var vms = VBox.ListVms().ToList();
			if (vms.Count == 0) { Assert.Inconclusive("Unable to test without VMs"); return; }

			bool foundVmToTest = false;
			foreach (var original in vms) {
				string desc;
				if (!VBox.VmInfoDictionary(original).TryGetValue("description", out desc) || !desc.Contains("clone testing vm")) continue;
				foundVmToTest = true;

				var clone = VBox.CloneVm(original);
				try {
					Guid g;
					Assert.IsTrue(clone.Name.StartsWith("Clone of "), "Cloned VMs should be named as such");
					Assert.IsTrue(Guid.TryParse(clone.Guid, out g), "Cloned VMs should have sane GUIDs");
					Assert.AreNotEqual(original.Guid, clone.Guid, "Cloned VMs should have new unique GUIDs");
				}
				finally { Assert.IsTrue(VBox.TryDeleteVm(clone)); }
			}

			if (!foundVmToTest) Assert.Inconclusive("Unable to test:  No VMs tagged 'clone testing vm' in their descriptions.");
		}

		[TestMethod] public void Clone_Delete_VMs_x2() {
			if (VBox.VBoxManagePath == null) { Assert.Inconclusive("Unable to test without VBoxManage"); return; }

			var vms = VBox.ListVms().ToList();
			if (vms.Count == 0) { Assert.Inconclusive("Unable to test without VMs"); return; }

			bool foundVmToTest = false;
			foreach (var original in vms) {
				string desc;
				if (!VBox.VmInfoDictionary(original).TryGetValue("description", out desc) || !desc.Contains("clone testing vm")) continue;
				foundVmToTest = true;

				var clone1 = default(VirtualBox.VmNameId);
				var clone2 = clone1;
				try {
					clone1 = VBox.CloneVm(original);
					clone2 = VBox.CloneVm(original);
					Assert.IsFalse(clone1.IsNull, "Cloning the VM should've succeeded");
					Assert.IsFalse(clone2.IsNull, "Cloning the VM should've succeeded");
					Guid g;
					Assert.IsTrue(clone1.Name.StartsWith("Clone of "), "Cloned VMs should be named as such");
					Assert.IsTrue(clone2.Name.StartsWith("Clone of "), "Cloned VMs should be named as such");
					Assert.IsTrue(Guid.TryParse(clone1.Guid, out g), "Cloned VMs should have sane GUIDs");
					Assert.IsTrue(Guid.TryParse(clone2.Guid, out g), "Cloned VMs should have sane GUIDs");
					Assert.AreNotEqual(original.Guid, clone1.Guid, "Cloned VMs should have new unique GUIDs");
					Assert.AreNotEqual(original.Guid, clone2.Guid, "Cloned VMs should have new unique GUIDs");
					Assert.AreNotEqual(clone1.Guid, clone2.Guid, "Cloned VMs should have new unique GUIDs");
				} finally {
					if (!clone1.IsNull) Assert.IsTrue(VBox.TryDeleteVm(clone1));
					if (!clone2.IsNull) Assert.IsTrue(VBox.TryDeleteVm(clone2));
				}
			}

			if (!foundVmToTest) Assert.Inconclusive("Unable to test:  No VMs tagged 'clone testing vm' in their descriptions.");
		}

		[TestMethod] public void Clone_Start_Poweroff_Delete_VMs() {
			if (VBox.VBoxManagePath == null) { Assert.Inconclusive("Unable to test without VBoxManage"); return; }

			var vms = VBox.ListVms().ToList();
			if (vms.Count == 0) { Assert.Inconclusive("Unable to test without VMs"); return; }

			bool foundVmToTest = false;
			foreach (var original in vms) {
				string desc;
				if (!VBox.VmInfoDictionary(original).TryGetValue("description", out desc) || !desc.Contains("clone testing vm")) continue;
				foundVmToTest = true;

				var clone = VBox.CloneVm(original);
				try {
					Guid g;
					Assert.IsTrue(clone.Name.StartsWith("Clone of "), "Cloned VMs should be named as such");
					Assert.IsTrue(Guid.TryParse(clone.Guid, out g), "Cloned VMs should have sane GUIDs");
					Assert.AreNotEqual(original.Guid, clone.Guid, "Cloned VMs should have new unique GUIDs");

					VBox.StartVm  (clone, VirtualBox.StartVmType.Headless);
					VBox.ControlVm(clone, VirtualBox.ControlVmType.AcpiPowerButton);
					VBox.ControlVm(clone, VirtualBox.ControlVmType.PowerOff);
				}
				finally { Assert.IsTrue(VBox.TryDeleteVm(clone)); }
			}

			if (!foundVmToTest) Assert.Inconclusive("Unable to test:  No VMs tagged 'clone testing vm' in their descriptions.");
		}
	}
}
