-- Tencent is pleased to support the open source community by making xLua available.
-- Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
-- Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
-- http://opensource.org/licenses/MIT
-- Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

-- List of CS.Photon.Pun.RpcTarget
---- CS.Photon.Pun.RpcTarget.All
---- CS.Photon.Pun.RpcTarget.Others
---- CS.Photon.Pun.RpcTarget.MasterClient
---- CS.Photon.Pun.RpcTarget.AllBuffered
---- CS.Photon.Pun.RpcTarget.OthersBuffered
---- CS.Photon.Pun.RpcTarget.AllViaServer
---- CS.Photon.Pun.RpcTarget.AllBufferedViaServer

function Start()
	if (self.photonView.IsMine) then
		--print("isMine", self.photonView.IsMine, "luaRPC")
		--print(self.photonView)
		--print(self.photonView.name)
		--print(self.photonView.IsMine)
	
		self:RPCLua("luarpc")
		self:RPCLua("luarpc2", {"test1"})
		--a = {"test1", "test2"}
		--self:RPCLua("luarpc2", a)
		self:RPCLua("luarpc2", {"test1", 123})
		
		self:RPC("Sphere", "TestRPCTargetFunc")
		self:RPC("Sphere", "TestRPCTargetFunc2", "test33")
		self:RPC("Sphere", "TestRPCTargetFunc5", {"test444", 12})
				
		self:RPCLua("luarpc2", {"test1", 123}, CS.Photon.Pun.RpcTarget.AllBuffered)
	else
	end
end

function Update()
	
end

function luarpc()
	if (self.photonView.IsMine) then
		print("isMine", self.photonView.IsMine, "luaRPC")
	else
		print("notMine", self.photonView.IsMine, "luaRPC")
	end
end

function luarpc2(value, value2)
	if (self.photonView.IsMine) then
		print("isMine", self.photonView.IsMine, "luaRPC2", value, value2)
	else
		print("notMine", self.photonView.IsMine, "luaRPC")
	end
end
