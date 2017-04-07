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
using System.IO;

namespace UnitTests {
	[TestClass] public class TestPaths {
		VirtualBox VBox = new VirtualBox();

		[TestMethod] public void Has_VBoxManage() { Assert.IsTrue(VBox.VBoxManagePath != null && File.Exists(VBox.VBoxManagePath)); }
		[TestMethod] public void Has_SSH()        { Assert.IsTrue(VBox.SshPath        != null && File.Exists(VBox.SshPath       )); }
		[TestMethod] public void Has_SCP()        { Assert.IsTrue(VBox.ScpPath        != null && File.Exists(VBox.ScpPath       )); }
	}
}
