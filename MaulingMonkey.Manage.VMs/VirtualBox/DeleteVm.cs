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

using System.Diagnostics;
using System.Threading.Tasks;

namespace MaulingMonkey.Manage.VMs {
	partial class VirtualBox {
		public void DeleteVm(VmNameId target) { DeleteVm(target.Guid); }
		public void DeleteVm(string   target) {
			if (VBoxManagePath == null) throw new MissingToolException("VBoxManage", VBoxManage_Paths);

			// TODO:  More streamy stdout parser so we can sanely parse:
			//      0%...10%...20%...30%...40%...50%...60%...70%...80%...90%...100%
			var exit = Proc.ExecIn(null, VBoxManagePath, $"unregistervm --delete {target}", stdout => { }, stderr => { }, ProcessWindowStyle.Hidden);
			if (exit != 0) throw new ToolResultSyntaxException("VBoxManage unregistervm --delete ...", "Returned nonzero");
		}

		public bool       TryDeleteVm     (VmNameId target) { try { DeleteVm(target); return true; } catch (VmManagementException) { return false; } }
		public bool       TryDeleteVm     (string   target) { try { DeleteVm(target); return true; } catch (VmManagementException) { return false; } }

		public Task       DeleteVmAsync   (VmNameId target) { return Task.Run(()=>DeleteVm(target)); }
		public Task       DeleteVmAsync   (string   target) { return Task.Run(()=>DeleteVm(target)); }

		public Task<bool> TryDeleteVmAsync(VmNameId target) { return Task.Run(()=>TryDeleteVm(target)); }
		public Task<bool> TryDeleteVmAsync(string   target) { return Task.Run(()=>TryDeleteVm(target)); }
	}
}
