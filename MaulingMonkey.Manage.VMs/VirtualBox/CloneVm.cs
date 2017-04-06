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
using System.Threading.Tasks;

namespace MaulingMonkey.Manage.VMs {
	partial class VirtualBox {
		public VmNameId CloneVm(VmNameId original                        ) { var guid = Guid.NewGuid().ToString(); return CloneVm(original.Guid, new VmNameId() { Name = $"Clone of {original.Name} {guid}", Guid = guid }); }
		public VmNameId CloneVm(string   originalGuid                    ) { var guid = Guid.NewGuid().ToString(); return CloneVm(originalGuid,  new VmNameId() { Name = $"Clone of {originalGuid} {guid}" , Guid = guid }); }
		public VmNameId CloneVm(VmNameId original,     string   newName  ) { var guid = Guid.NewGuid().ToString(); return CloneVm(original.Guid, new VmNameId() { Name = newName, Guid = guid }); }
		public VmNameId CloneVm(string   originalGuid, string   newName  ) { var guid = Guid.NewGuid().ToString(); return CloneVm(originalGuid,  new VmNameId() { Name = newName, Guid = guid }); }
		public VmNameId CloneVm(VmNameId original,     VmNameId newNameId) { return CloneVm(original.Guid, newNameId); }
		public VmNameId CloneVm(string   originalGuid, VmNameId newNameId) {
			if (VBoxManagePath == null) throw new MissingToolException("VBoxManage", VBoxManage_Paths);

			// TODO:  More streamy stdout parser so we can sanely parse:
			//      0%...10%...20%...30%...40%...50%...60%...70%...80%...90%...100%
			//      Machine has been successfully cloned as "Name"
			var exit = Proc.ExecIn(null, VBoxManagePath, $"clonevm {originalGuid} --register --uuid {newNameId.Guid} --name \"{newNameId.Name}\"", stdout => { }, stderr => { }, ProcessWindowStyle.Hidden);
			if (exit != 0) throw new ToolResultSyntaxException("VBoxManage clonevm ...", "Returned nonzero");
			return newNameId;
		}

		public VmNameId TryCloneVm(VmNameId original                        ) { try { return CloneVm(original               ); } catch (VmManagementException) { return default(VmNameId); } }
		public VmNameId TryCloneVm(string   originalGuid                    ) { try { return CloneVm(originalGuid           ); } catch (VmManagementException) { return default(VmNameId); } }
		public VmNameId TryCloneVm(VmNameId original,     string   newName  ) { try { return CloneVm(original    , newName  ); } catch (VmManagementException) { return default(VmNameId); } }
		public VmNameId TryCloneVm(string   originalGuid, string   newName  ) { try { return CloneVm(originalGuid, newName  ); } catch (VmManagementException) { return default(VmNameId); } }
		public VmNameId TryCloneVm(VmNameId original,     VmNameId newNameId) { try { return CloneVm(original    , newNameId); } catch (VmManagementException) { return default(VmNameId); } }
		public VmNameId TryCloneVm(string   originalGuid, VmNameId newNameId) { try { return CloneVm(originalGuid, newNameId); } catch (VmManagementException) { return default(VmNameId); } }

		public Task<VmNameId> CloneVmAsync(VmNameId original                        ) { return Task.Run(()=>CloneVm(original               )); }
		public Task<VmNameId> CloneVmAsync(string   originalGuid                    ) { return Task.Run(()=>CloneVm(originalGuid           )); }
		public Task<VmNameId> CloneVmAsync(VmNameId original,     string   newName  ) { return Task.Run(()=>CloneVm(original,     newName  )); }
		public Task<VmNameId> CloneVmAsync(string   originalGuid, string   newName  ) { return Task.Run(()=>CloneVm(originalGuid, newName  )); }
		public Task<VmNameId> CloneVmAsync(VmNameId original,     VmNameId newNameId) { return Task.Run(()=>CloneVm(original,     newNameId)); }
		public Task<VmNameId> CloneVmAsync(string   originalGuid, VmNameId newNameId) { return Task.Run(()=>CloneVm(originalGuid, newNameId)); }

		public Task<VmNameId> TryCloneVmAsync(VmNameId original                        ) { return Task.Run(()=>TryCloneVm(original               )); }
		public Task<VmNameId> TryCloneVmAsync(string   originalGuid                    ) { return Task.Run(()=>TryCloneVm(originalGuid           )); }
		public Task<VmNameId> TryCloneVmAsync(VmNameId original,     string   newName  ) { return Task.Run(()=>TryCloneVm(original,     newName  )); }
		public Task<VmNameId> TryCloneVmAsync(string   originalGuid, string   newName  ) { return Task.Run(()=>TryCloneVm(originalGuid, newName  )); }
		public Task<VmNameId> TryCloneVmAsync(VmNameId original,     VmNameId newNameId) { return Task.Run(()=>TryCloneVm(original,     newNameId)); }
		public Task<VmNameId> TryCloneVmAsync(string   originalGuid, VmNameId newNameId) { return Task.Run(()=>TryCloneVm(originalGuid, newNameId)); }
	}
}
