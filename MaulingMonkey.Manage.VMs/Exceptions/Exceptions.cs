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

namespace MaulingMonkey.Manage.VMs {
	class VmManagementException : Exception {
		internal VmManagementException(string message): base(message) { }
	}

	class MissingToolException : VmManagementException {
		public string              ToolName { get; private set; }
		public IEnumerable<string> SearchPaths { get; private set; }

		internal MissingToolException(string toolName, string[] searchPaths): base("Missing required tool: "+toolName) {
			ToolName    = toolName;
			SearchPaths = searchPaths;
		}
	}

	class ToolResultSyntaxException : VmManagementException {
		public string ToolName    { get; private set; }
		public string SyntaxError { get; private set; }

		internal ToolResultSyntaxException(string toolName, string syntaxError): base("Error parsing result of "+toolName+": "+syntaxError) {
			ToolName    = toolName;
			SyntaxError = syntaxError;
		}
	}
}
