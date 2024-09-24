-- Tencent is pleased to support the open source community by making xLua available.
-- Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
-- Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
-- http://opensource.org/licenses/MIT
-- Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

local speed = 1
local dis
local player
function Start()
	print("lua UnderGround start...")
	player = CS.UnityEngine.GameObject.Find("ARCamera");
	print(player)
end

function Update()
	dis = CS.UnityEngine.Vector3.Distance( CS.UnityEngine.Vector3(self.transform.position.x, 0, self.transform.position.z),CS.UnityEngine.Vector3(player.transform.position.x, 0, player.transform.position.z))
	if dis <= 2.7 then
		self.transform.localPosition = CS.UnityEngine.Vector3.Lerp(self.transform.localPosition, CS.UnityEngine.Vector3(0,0,0),1*CS.UnityEngine.Time.deltaTime)
	end
end


