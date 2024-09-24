-- Tencent is pleased to support the open source community by making xLua available.
-- Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
-- Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
-- http://opensource.org/licenses/MIT
-- Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.


local thisbutton
local panel

local cam

local cnt = 1


function start()
	cam = CS.UnityEngine.GameObject.Find("ARCamera")
	thisbutton = self.transform:GetComponent(typeof(CS.UnityEngine.UI.Button))
	panel = CS.UnityEngine.GameObject.Find("Canvas")

	carpet1 = CS.UnityEngine.GameObject.Find("Carpet_01")
	carpet2 = CS.UnityEngine.GameObject.Find("Carpet_02")
	carpet3 = CS.UnityEngine.GameObject.Find("Carpet_03")
	carpet4 = CS.UnityEngine.GameObject.Find("Carpet_04")
	carpet5 = CS.UnityEngine.GameObject.Find("Carpet_05")
	carpet6 = CS.UnityEngine.GameObject.Find("Carpet_06")
	carpet7 = CS.UnityEngine.GameObject.Find("Carpet_07")
	carpet8 = CS.UnityEngine.GameObject.Find("Carpet_08")
	carpet9 = CS.UnityEngine.GameObject.Find("Carpet_09")

	carpet1:SetActive(true)
	carpet2:SetActive(false)
	carpet3:SetActive(false)
	carpet4:SetActive(false)
	carpet5:SetActive(false)
	carpet6:SetActive(false)
	carpet7:SetActive(false)
	carpet8:SetActive(false)
	carpet9:SetActive(false)

	thisbutton.onClick:AddListener(function()
		if cnt==0 then
			carpet1:SetActive(true)
			carpet2:SetActive(false)
			carpet3:SetActive(false)
			carpet4:SetActive(false)
			carpet5:SetActive(false)
			carpet6:SetActive(false)
			carpet7:SetActive(false)
			carpet8:SetActive(false)
			carpet9:SetActive(false)
		elseif cnt==1 then
			carpet1:SetActive(false)
			carpet2:SetActive(true)
			carpet3:SetActive(false)
			carpet4:SetActive(false)
			carpet5:SetActive(false)
			carpet6:SetActive(false)
			carpet7:SetActive(false)
			carpet8:SetActive(false)
			carpet9:SetActive(false)
		elseif cnt==2 then
			carpet1:SetActive(false)
			carpet2:SetActive(false)
			carpet3:SetActive(true)
			carpet4:SetActive(false)
			carpet5:SetActive(false)
			carpet6:SetActive(false)
			carpet7:SetActive(false)
			carpet8:SetActive(false)
			carpet9:SetActive(false)
		elseif cnt==3 then
			carpet1:SetActive(false)
			carpet2:SetActive(false)
			carpet3:SetActive(false)
			carpet4:SetActive(true)
			carpet5:SetActive(false)
			carpet6:SetActive(false)
			carpet7:SetActive(false)
			carpet8:SetActive(false)
			carpet9:SetActive(false)
		elseif cnt==4 then
			carpet1:SetActive(false)
			carpet2:SetActive(false)
			carpet3:SetActive(false)
			carpet4:SetActive(false)
			carpet5:SetActive(true)
			carpet6:SetActive(false)
			carpet7:SetActive(false)
			carpet8:SetActive(false)
			carpet9:SetActive(false)
		elseif cnt==5 then
			carpet1:SetActive(false)
			carpet2:SetActive(false)
			carpet3:SetActive(false)
			carpet4:SetActive(false)
			carpet5:SetActive(false)
			carpet6:SetActive(true)
			carpet7:SetActive(false)
			carpet8:SetActive(false)
			carpet9:SetActive(false)
		elseif cnt==6 then
			carpet1:SetActive(false)
			carpet2:SetActive(false)
			carpet3:SetActive(false)
			carpet4:SetActive(false)
			carpet5:SetActive(false)
			carpet6:SetActive(false)
			carpet7:SetActive(true)
			carpet8:SetActive(false)
			carpet9:SetActive(false)
		elseif cnt==7 then
			carpet1:SetActive(false)
			carpet2:SetActive(false)
			carpet3:SetActive(false)
			carpet4:SetActive(false)
			carpet5:SetActive(false)
			carpet6:SetActive(false)
			carpet7:SetActive(false)
			carpet8:SetActive(true)
			carpet9:SetActive(false)
		elseif cnt==8 then
			carpet1:SetActive(false)
			carpet2:SetActive(false)
			carpet3:SetActive(false)
			carpet4:SetActive(false)
			carpet5:SetActive(false)
			carpet6:SetActive(false)
			carpet7:SetActive(false)
			carpet8:SetActive(false)
			carpet9:SetActive(true)
		end
		if cnt==8 then
			cnt = 0
		else
			cnt = cnt+1
		end

	end
	)
end

function update()
	panel.transform.forward = -(cam.transform.position-panel.transform.position)
end

function carpetchange()
	

end


