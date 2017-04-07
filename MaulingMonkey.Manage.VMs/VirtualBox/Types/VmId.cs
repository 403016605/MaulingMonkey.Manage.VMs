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


namespace MaulingMonkey.Manage.VMs {
	partial class VirtualBox {
		public struct VmId {
			string Id;

			public VmId(VmNameId id) { Id = id.Guid ?? id.Name; }
			public VmId(string   id) { Id = id; }
			public static implicit operator VmId(VmNameId nameId) { return new VmId(nameId); }
			public static explicit operator VmId(string id) { return new VmId(id); }

			public static implicit operator string(VmId id) { return id.ToString(); }
			public override string ToString() { return Id; }
		}
	}
}
