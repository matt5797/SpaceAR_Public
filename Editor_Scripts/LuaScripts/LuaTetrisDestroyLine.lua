-- Tencent is pleased to support the open source community by making xLua available.
-- Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
-- Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
-- http://opensource.org/licenses/MIT
-- Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.


local blockCount = 0
local blockList = {}
local blockListIndex = 1

function Start()
    
end

function Update()
	if blockCount > 8 then
		DestroyLine();
	end
end

function DestroyLine()
    for index = 1, 9 do
		local block = blockList[1]
		print("block:", block)
		table.remove(blockList, 1)
		CS.UnityEngine.GameObject.Destroy(block)
	end
    blockCount = 0
end

function OnTriggerEnter(other)
	print("OnTriggerEnter:", blockCount, blockListIndex, other.gameObject)
    blockCount = blockCount + 1
	blockList[blockListIndex] = other.gameObject
	blockListIndex = blockListIndex + 1
end

function OnTriggerExit(other)
    blockCount = blockCount - 1
	table.remove(blockList, blockListIndex)
	blockListIndex = blockListIndex - 1
end
