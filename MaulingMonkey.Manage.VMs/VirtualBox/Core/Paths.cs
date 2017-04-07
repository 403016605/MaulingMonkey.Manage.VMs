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
using System.IO;

namespace MaulingMonkey.Manage.VMs {
	public partial class VirtualBox {
		public string VBoxManagePath = TrySearch(VBoxManage_Paths);
		public string SshPath        = TrySearch(Ssh_Paths);
		public string ScpPath        = TrySearch(Scp_Paths);



		// TODO: Search registry paths as well.
		// TODO: Search *nix paths as well.

		static readonly string[] VBoxManage_Paths = new[] {
			@"%VBOX_MSI_INSTALL_PATH%\VBoxManage.exe",
			@"%ProgramFiles%\Oracle\VirtualBox\VBoxManage.exe",
			@"%ProgramFilesW6432%\Oracle\VirtualBox\VBoxManage.exe",
		};

		static readonly string[] Ssh_Paths = new[] {
			@"%ProgramW6432%\Git\usr\bin\ssh.exe",
			@"%ProgramFiles%\Git\usr\bin\ssh.exe",
		};

		static readonly string[] Scp_Paths = new[] {
			@"%ProgramW6432%\Git\usr\bin\scp.exe",
			@"%ProgramFiles%\Git\usr\bin\scp.exe",
		};

		static string TrySearch(string[] paths) {
			foreach (var path in paths) {
				// TODO: Replace \ with OS-native path seperators?
				var exp = Environment.ExpandEnvironmentVariables(path);
				if (File.Exists(exp)) return exp;
			}
			return null;
		}
	}
}
