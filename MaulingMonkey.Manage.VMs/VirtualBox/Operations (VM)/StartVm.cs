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

namespace MaulingMonkey.Manage.VMs {
	partial class VirtualBox {
		/// <summary>
		/// See https://www.virtualbox.org/manual/ch08.html#vboxmanage-startvm
		/// </summary>
		public enum StartVmType {
			/// <summary>
			/// Don't specify a --type when invoking startvm.
			/// </summary>
			Default,

			/// <summary>
			/// Starts a VM showing a GUI window. This is the default.
			/// </summary>
			Gui,

			/// <summary>
			/// Starts a VM with a minimal GUI and limited features.
			/// </summary>
			Sdl,

			/// <summary>
			/// Starts a VM without a window for remote display only.
			/// </summary>
			Headless,

			/// <summary>
			/// Starts a VM with detachable UI (technically it is a headless VM with user interface in a separate process). This is an experimental feature as it lacks certain functionality at the moment (e.g. 3D acceleration will not work).
			/// </summary>
			Separate
		}

		public void StartVm(VmId vm                  ) { StartVm(vm, StartVmType.Default); }
		public void StartVm(VmId vm, StartVmType type) {
			var typeFlag = "";
			switch (type) {
			case StartVmType.Default:  break;
			case StartVmType.Gui:      typeFlag = "--type=gui";      break;
			case StartVmType.Headless: typeFlag = "--type=headless"; break;
			case StartVmType.Sdl:      typeFlag = "--type=sdl";      break;
			case StartVmType.Separate: typeFlag = "--type=separate"; break;
			}

			// Typical stdout:
			//    Waiting for VM "OpenBSD 64-bit" to power on...
			//    VM "OpenBSD 64-bit" has been successfully started.
			// Note that the system has not finished booting by the time this returns!
			var exit = Proc.ExecIn(null, VBoxManagePath, $"startvm {vm} {typeFlag}", stdout => { }, stderr => { }, ProcessWindowStyle.Hidden);
			if (exit != 0) throw new ToolResultSyntaxException("VBoxManage startvm ...", "Returned nonzero");
		}

		public bool TryStartVm(VmId vm                  ) { try { StartVm(vm      ); return true; } catch (VmManagementException) { return false; } }
		public bool TryStartVm(VmId vm, StartVmType type) { try { StartVm(vm, type); return true; } catch (VmManagementException) { return false; } }
	}
}
