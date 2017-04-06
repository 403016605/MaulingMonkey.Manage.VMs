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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MaulingMonkey.Manage.VMs {
	static class Proc {
		public enum OutSource {
			Output  = 1,
			Error   = 2,
		}

		public struct OutLine {
			public OutSource    Source;
			public string       Line;
			public override string ToString() { return Line; }
		}

		static IEnumerable<OutLine> EnumProcess(Process process) {
			var rOut = process.StandardOutput;
			var rErr = process.StandardError;

			var srcs  = new[] { OutSource.Output, OutSource.Error };
			var pipes = new[] { rOut, rErr };
			var tasks = new[] { rOut.ReadLineAsync(), rErr.ReadLineAsync() };

			while (tasks.Length > 0) {
				int index = Task.WaitAny(tasks);
				var line = tasks[index].Result;
				if (line != null) {
					tasks[index] = pipes[index].ReadLineAsync();
					yield return new OutLine() { Line = line, Source = srcs[index] };
				} else if (tasks.Length > 1) {
					srcs  = new[] { srcs [1-index] };
					pipes = new[] { pipes[1-index] };
					tasks = new[] { tasks[1-index] };
				} else {
					break;
				}
			}
		}

		public static IEnumerable<OutLine> EnumExecIn(string dir, string command, string args, ProcessWindowStyle style, out int exitCode) {
			var psi = new ProcessStartInfo(command, args) {
				WorkingDirectory		= dir,
				WindowStyle				= style,
				RedirectStandardOutput	= true,
				RedirectStandardError	= true,
				UseShellExecute			= false,
				CreateNoWindow			= style == ProcessWindowStyle.Hidden,
			};

			using (var process = Process.Start(psi)) {
				var output = EnumProcess(process).ToList();
				process.WaitForExit();
				exitCode = process.ExitCode;
				return output;
			}
		}

		public static int ExecIn(string dir, string command, string args, Action<OutLine> onOutput, ProcessWindowStyle style) {
			var psi = new ProcessStartInfo(command, args) {
				WorkingDirectory		= dir,
				WindowStyle				= style,
				RedirectStandardOutput	= true,
				RedirectStandardError	= true,
				UseShellExecute			= false,
				CreateNoWindow			= style == ProcessWindowStyle.Hidden,
			};

			using (var process = Process.Start(psi)) {
				foreach (var line in EnumProcess(process)) onOutput(line);
				process.WaitForExit();
				return process.ExitCode;
			}
		}

		public static int ExecIn(string dir, string command, string args, Action<string> onStdout, Action<string> onStderr, ProcessWindowStyle style) {
			return ExecIn(dir, command, args, o => {
				switch (o.Source) {
				case OutSource.Output: onStdout(o.Line); break;
				case OutSource.Error:  onStderr(o.Line); break;
				}
			}, style);
		}
	}
}
