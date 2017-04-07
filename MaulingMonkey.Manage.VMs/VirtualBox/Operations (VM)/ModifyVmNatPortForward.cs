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
		public enum PortType { Tcp, Udp }

		public void ModifyVmNatPortForward(VmId vm, int natNo, string ruleName, PortType portType, string hostIp, int hostPort, string guestIp, int guestPort) {
			if (vm == null) throw new ArgumentNullException(nameof(vm), nameof(ModifyVmNatPortForward)+" requires a target");
			if (vm == ""  ) throw new ArgumentException(nameof(ModifyVmNatPortForward)+" requires a target", nameof(vm));
			if (natNo < 1 ) throw new ArgumentOutOfRangeException(nameof(natNo), natNo, "natNo is 1-based");

			ruleName = ruleName ?? "";
			hostIp   = hostIp   ?? "";
			guestIp  = guestIp  ?? "";

			var rule = $"{ruleName},{portType.ToString().ToLowerInvariant()},{hostIp},{hostPort},{guestIp},{guestPort}";
			var exit = Proc.ExecIn(null, VBoxManagePath, $"modifyvm {vm} --natpf{natNo} \"{rule}\"", stdout => { }, stderr => { }, ProcessWindowStyle.Hidden);
			if (exit != 0) throw new ToolResultSyntaxException("VBoxManage modifyvm .... --natpf...", "Returned nonzero");
		}

		public void ModifyVmDeleteNatPortForward(VmId vm, int natNo, string ruleName) {
			if (vm       == null) throw new ArgumentNullException(nameof(vm),   nameof(ModifyVmDeleteNatPortForward)+" requires a target to delete from");
			if (vm       == ""  ) throw new ArgumentException(nameof(ModifyVmDeleteNatPortForward)+" requires a target to delete from", nameof(vm));
			if (natNo    <  1   ) throw new ArgumentOutOfRangeException(nameof(natNo), natNo, "natNo is 1-based");
			if (ruleName == null) throw new ArgumentNullException(nameof(ruleName), nameof(ModifyVmDeleteNatPortForward)+" requires a ruleName to delete");
			if (ruleName == ""  ) throw new ArgumentException(nameof(ModifyVmDeleteNatPortForward)+" requires a ruleName to delete", nameof(ruleName));

			var exit = Proc.ExecIn(null, VBoxManagePath, $"modifyvm {vm} --natpf{natNo} delete \"{ruleName}\"", stdout => { }, stderr => { }, ProcessWindowStyle.Hidden);
			if (exit != 0) throw new ToolResultSyntaxException("VBoxManage modifyvm .... --natpf...", "Returned nonzero");
		}
	}
}
