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

using System;
using System.Diagnostics;

namespace MaulingMonkey.Manage.VMs {
	partial class VirtualBox {
		public VmNameId CloneVm(VmNameId original              ) { var guid = Guid.NewGuid().ToString(); return CloneVm(original,  new VmNameId() { Name = $"Clone of {original.Name} {guid}" , Guid = guid }); }
		public VmNameId CloneVm(VmId original                  ) { var guid = Guid.NewGuid().ToString(); return CloneVm(original,  new VmNameId() { Name = $"Clone of {original} {guid}" , Guid = guid }); }
		public VmNameId CloneVm(VmId original, string   newName) { var guid = Guid.NewGuid().ToString(); return CloneVm(original,  new VmNameId() { Name = newName, Guid = guid }); }
		public VmNameId CloneVm(VmId original, VmNameId clone  ) {
			if (VBoxManagePath == null) throw new MissingToolException("VBoxManage", VBoxManage_Paths);

			// TODO:  More streamy stdout parser so we can sanely parse:
			//      0%...10%...20%...30%...40%...50%...60%...70%...80%...90%...100%
			//      Machine has been successfully cloned as "Name"
			var exit = Proc.ExecIn(null, VBoxManagePath, $"clonevm {original} --register --uuid {clone.Guid} --name \"{clone.Name}\"", stdout => { }, stderr => { }, ProcessWindowStyle.Hidden);
			if (exit != 0) throw new ToolResultSyntaxException("VBoxManage clonevm ...", "Returned nonzero");
			return clone;
		}

		public VmNameId TryCloneVm(VmNameId original              ) { try { return CloneVm(original         ); } catch (VmManagementException) { return default(VmNameId); } }
		public VmNameId TryCloneVm(VmId original                  ) { try { return CloneVm(original         ); } catch (VmManagementException) { return default(VmNameId); } }
		public VmNameId TryCloneVm(VmId original, string   newName) { try { return CloneVm(original, newName); } catch (VmManagementException) { return default(VmNameId); } }
		public VmNameId TryCloneVm(VmId original, VmNameId clone  ) { try { return CloneVm(original, clone  ); } catch (VmManagementException) { return default(VmNameId); } }
	}
}
