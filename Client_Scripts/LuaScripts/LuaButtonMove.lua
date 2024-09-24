-- Tencent is pleased to support the open source community by making xLua available.
-- Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
-- Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
-- http://opensource.org/licenses/MIT
-- Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

local speed = 1
local dis
local dir
local moveCheck = true
local player
local thisbutton

local originPoscheck = false
local originPos

function start()
	
	thisbutton = self.transform:GetChild(0):GetChild(0):GetComponent(typeof(CS.UnityEngine.UI.Button))
	thisbutton.onClick:AddListener(function()
	if moveCheck==true then
		moveCheck = false
	else
		moveCheck=true
	end
	end)
	
end

function update()
	player = CS.UnityEngine.GameObject.Find("ARCamera");
	if originPoscheck == false then
		originPos = self.transform.position
		originPoscheck = true
	end
	thisbutton.gameObject.transform.forward = -(player.transform.position - self.transform.position)
	dis = CS.UnityEngine.Vector3.Distance( CS.UnityEngine.Vector3(self.transform.position.x, 0, self.transform.position.z),CS.UnityEngine.Vector3(player.transform.position.x, 0, player.transform.position.z))
	if dis <= 2.0 then
		thisbutton.gameObject:SetActive(true);
	else
		thisbutton.gameObject:SetActive(false);
	end

	if moveCheck==false then
		dir = self.transform.forward
		if CS.UnityEngine.Vector3.Distance(self.transform.position,originPos)>=0.4 then
		
		else
			self.transform.position = self.transform.position + dir * CS.UnityEngine.Time.deltaTime * speed
		end
	else
		dir = -self.transform.forward
		if CS.UnityEngine.Vector3.Distance(self.transform.position,originPos)<=0.02 then

		else
			self.transform.position = self.transform.position + dir * CS.UnityEngine.Time.deltaTime * speed
		end
	end
end


function moveUporDown()
	if moveCheck==true then
		moveCheck = false
	else
		moveCheck=true
	end
end


