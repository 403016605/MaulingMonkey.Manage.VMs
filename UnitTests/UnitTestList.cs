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
	[TestClass] public class UnitTestList {
		VirtualBox VBox = new VirtualBox();

		[TestMethod] public void Has_VMs() {
			if (VBox.VBoxManagePath == null) { Assert.Inconclusive("Unable to test if Has_VMs without VBoxManage"); return; }

			var vms = VBox.ListVms().ToList();
			Assert.IsTrue(vms.Count > 0, "Surely you have at least one VM installed");
		}

		[TestMethod] public void VMs_Have_Sane_Values() {
			if (VBox.VBoxManagePath == null) { Assert.Inconclusive("Unable to test if Has_VMs without VBoxManage"); return; }

			var vms = VBox.ListVms().ToList();
			if (vms.Count == 0) { Assert.Inconclusive("Unable to test if no VMs are configured"); return; }

			foreach (var vm in vms) {
				Assert.IsNotNull(vm.Name, "vm.Name should never be null");
				Assert.IsNotNull(vm.Guid, "vm.Guid should never be null");
				if (vm.Name != null) Assert.IsTrue(!string.IsNullOrWhiteSpace(vm.Name), "vm.Name surely shouldn't be blank");
				if (vm.Guid != null) Assert.IsTrue(!string.IsNullOrWhiteSpace(vm.Guid), "vm.Guid should never be blank");
			}
		}
	}
}
