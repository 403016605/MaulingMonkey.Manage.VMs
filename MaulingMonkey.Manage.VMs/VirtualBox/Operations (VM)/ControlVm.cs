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
		/// <summary>
		/// See https://www.virtualbox.org/manual/ch08.html#vboxmanage-controlvm
		/// </summary>
		public enum ControlVmType {
			/// <summary>
			/// Temporarily puts a virtual machine on hold, without changing its state for good. The VM window will be painted in gray to indicate that the VM is currently paused. (This is equivalent to selecting the "Pause" item in the "Machine" menu of the GUI).
			/// </summary>
			Pause,

			/// <summary>
			/// Undos a previous Pause command. (This is equivalent to selecting the "Resume" item in the "Machine" menu of the GUI.)
			/// </summary>
			Resume,

			/// <summary>
			/// Has the same effect on a virtual machine as pressing the "Reset" button on a real computer: a cold reboot of the virtual machine, which will restart and boot the guest operating system again immediately. The state of the VM is not saved beforehand, and data may be lost. (This is equivalent to selecting the "Reset" item in the "Machine" menu of the GUI).
			/// </summary>
			Reset,

			/// <summary>
			/// Sends an ACPI shutdown signal to the VM, as if the power button on a real computer had been pressed. So long as the VM is running a fairly modern guest operating system providing ACPI support, this should trigger a proper shutdown mechanism from within the VM.
			/// </summary>
			AcpiPowerButton,

			/// <summary>
			/// Has the same effect on a virtual machine as pulling the power cable on a real computer. Again, the state of the VM is not saved beforehand, and data may be lost. (This is equivalent to selecting the "Close" item in the "Machine" menu of the GUI or pressing the window's close button, and then selecting "Power off the machine" in the dialog).
			/// </summary>
			PowerOff,
		}

		public void ControlVm(VmId vm, ControlVmType type) {
			string args = null;
			switch (type) {
			case ControlVmType.AcpiPowerButton: args = $"controlvm {vm} acpipowerbutton"; break;
			case ControlVmType.PowerOff:        args = $"controlvm {vm} poweroff";        break;
			}
			if (args == null) throw new ArgumentOutOfRangeException(nameof(type), "ControlVm passed invalid ControlVmType enumeration value");

			var exit = Proc.ExecIn(null, VBoxManagePath, args, stdout => { }, stderr => { }, ProcessWindowStyle.Hidden);
			if (exit != 0) throw new ToolResultSyntaxException("VBoxManage controlvm ...", "Returned nonzero");
		}

		public bool TryControlVm(VmId vm, ControlVmType type) { try { ControlVm(vm, type); return true; } catch (VmManagementException) { return false; } }
	}
}
