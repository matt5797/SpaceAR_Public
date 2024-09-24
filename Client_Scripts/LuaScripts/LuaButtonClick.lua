-- Tencent is pleased to support the open source community by making xLua available.
-- Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
-- Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
-- http://opensource.org/licenses/MIT
-- Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.


local thisbutton
local panel
local changebutton1
local changebutton2
local changebutton3

local cam

local map1
local map2
local map3
function start()
	cam = CS.UnityEngine.GameObject.Find("ARCamera")
	thisbutton = self.transform:GetComponent(typeof(CS.UnityEngine.UI.Button))
	panel = CS.UnityEngine.GameObject.Find("Canvas")
	changebutton1 = CS.UnityEngine.GameObject.Find("changeBtn1")
	changebutton2 = CS.UnityEngine.GameObject.Find("changeBtn2")
	changebutton3 = CS.UnityEngine.GameObject.Find("changeBtn3")
	changebutton1:SetActive(false)
	changebutton2:SetActive(false)
	changebutton3:SetActive(false)

	map1 = CS.UnityEngine.GameObject.Find("01_Office_White")
	map2 = CS.UnityEngine.GameObject.Find("02_Office_Wood")
	map3 = CS.UnityEngine.GameObject.Find("03_Office_Vivid")

	map1:SetActive(true)
	map2:SetActive(false)
	map3:SetActive(false)

	thisbutton.onClick:AddListener(function()
		panel.transform.position =  cam.transform.position + cam.transform.forward * 4
		changebutton1:SetActive(true)
		changebutton2:SetActive(true)
		changebutton3:SetActive(true)
	end
	)

	changebutton1:GetComponent(typeof(CS.UnityEngine.UI.Button)).onClick:AddListener(function()
		map1:SetActive(true)
		map2:SetActive(false)
		map3:SetActive(false)
	end
	)
	changebutton2:GetComponent(typeof(CS.UnityEngine.UI.Button)).onClick:AddListener(function()
		map1:SetActive(false)
		map2:SetActive(true)
		map3:SetActive(false)
	end
	)
	changebutton3:GetComponent(typeof(CS.UnityEngine.UI.Button)).onClick:AddListener(function()
		map1:SetActive(false)
		map2:SetActive(false)
		map3:SetActive(true)
	end
	)
	
end

function update()
	panel.transform.forward = -(cam.transform.position-panel.transform.position)
end


