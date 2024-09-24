-- Tencent is pleased to support the open source community by making xLua available.
-- Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
-- Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
-- http://opensource.org/licenses/MIT
-- Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.


local Block1 = this;
local Block2 = this;
local Block3 = this;

local startTime = 0;
local timer = 0;

local stage = 0;

function Start()
	Block1 = CS.UnityEngine.GameObject.Find("Tetris Block 1")
	Block2 = CS.UnityEngine.GameObject.Find("Tetris Block 2")
	Block3 = CS.UnityEngine.GameObject.Find("Tetris Block 3")

	startTime = os.clock()
end

function Update()
	timer = os.clock()
	if timer - startTime > 2 and stage==0 then
		Drop1()
		stage = 1
	end
	if timer - startTime > 4.5 and stage==1 then
		Drop2()
		stage = 2
	end
	if timer - startTime > 7 and stage==2 then
		Drop3()
		stage = 3
	end
end

function Drop1()
	rigidbody = Block1:GetComponent("Rigidbody")
	rigidbody.isKinematic = false
end

function Drop2()
	rigidbody = Block2:GetComponent("Rigidbody")
	rigidbody.isKinematic = false
end

function Drop3()
	rigidbody = Block3:GetComponent("Rigidbody")
	rigidbody.isKinematic = false
end
