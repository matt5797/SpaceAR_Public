-- Tencent is pleased to support the open source community by making xLua available.
-- Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
-- Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
-- http://opensource.org/licenses/MIT
-- Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

--global speed
--global dis
--global basketBallOrigin
--global ARCamera
--global basketBall

local fireButton = CS.UnityEngine.GameObject.Find("FireButton");

function Start()
	fireButton = CS.UnityEngine.GameObject.Find("FireButton"):GetComponent("Button");
	print(fireButton)
	fireButton.onClick:AddListener(function()
		Shoot()
	end)
end

function Update()
	
end


function Shoot()
	mainCamera = CS.UnityEngine.Camera.main;
	
	basketBallOrigin = CS.UnityEngine.GameObject.Find("BasketBallOrigin");
	basketBall = CS.UnityEngine.GameObject.Instantiate(basketBallOrigin, mainCamera.transform.position, mainCamera.transform.rotation)
	
	rb = basketBall:GetComponent("Rigidbody")
	rb.useGravity = true;
	rb.isKinematic = false;
	
	dir = mainCamera.transform.forward
	rb.velocity = dir * CS.UnityEngine.Random.Range(3, 15);
end

function OnClick()
	Shoot()
end
