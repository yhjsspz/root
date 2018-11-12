local api = class("api", import("cframework.api"))

--- <summary>
--- 加载配置
--- </summary>
function api:loadConfig(name)
	return require("config/"..name)
end

--- <summary>
--- 设置文字
--- </summary>
function api:setText(txtObj,txt)
	txtObj.transform:GetComponent("Text").text = tostring(txt)
end

--- <summary>
--- 设置图片
--- </summary>
function api:setImage(viewId, imgObj, path, name, callback)
	
	api:loadPrefab(viewId, path, name.."_sprite", function (sprite)
		if sprite == nil then
			logError("api:loadPrefab error:",path)
			return
		end
		
		imgObj:GetComponent("Image").sprite = sprite
		
		if callback ~= nil then
			callback()
		end
	end)
end

--- <summary>
--- 设置进度条的值
--- </summary>
function api:setSliderValue(sliObj,value, isUseAni, callback, playTime)

    if playTime == nil then playTime = 0.5 end

	if isUseAni ~= nil and isUseAni == true then
		
		local sequence = DOTween.Sequence()
		sequence:Append(sliObj:GetComponent("Slider"):DOValue(value, playTime, false):SetEase(DG.Tweening.Ease.Linear))
		
		if callback ~= nil then
			
			sequence:AppendCallback(callback)
			
		end
		
	else
		
		sliObj:GetComponent("Slider").value = value
		
		if callback ~= nil then
			
			callback()
			
		end
	end
	
end

--- <summary>
--- 设置对象置灰
--- isRaycastTarget 参数为nil的时候 默认为false
--- isRecursion 是否递归置灰 参数为nil的时候 true
--- </summary>
function api:setGray(target, isRaycastTarget, isRecursion)

	isRecursion = isRecursion == nil and true or isRecursion
	
	api:loadPrefabSync(nil,"ui/common/material", "sprites_defaultgray")
	
	local mat = api:getAsset("ui/common/material", "sprites_defaultgray")

	if target:GetComponent("Image") ~= nil then
		
		
		local img = target:GetComponent("Image")
		img.material = mat
		img.raycastTarget = isRaycastTarget ~= nil and isRaycastTarget or false

	elseif target:GetComponent("SkeletonGraphic") ~= nil then
		
		local skeleton = target:GetComponent("SkeletonGraphic")
		skeleton.material = mat
		skeleton.raycastTarget = isRaycastTarget ~= nil and isRaycastTarget or false
		
	elseif target:GetComponent("Text") ~= nil then
		
		local txt = target:GetComponent("Text")
		
		txt.color = api:getGreyColor(txt.color)
		
	end
	
	
	
	if isRecursion == true then
		
		for i=0,target.transform.childCount - 1 do
			
			local child = target.transform:GetChild(i).gameObject
			
			if child:GetComponent("Image") ~= nil or 
				child:GetComponent("SkeletonGraphic") ~= nil or
				child:GetComponent("Text") ~= nil then
				
				api:setGray(target.transform:GetChild(i).gameObject, isRaycastTarget, isRecursion)
				
			end
			
		end
		
	end
	
end

--- <summary>
--- 解除对象置灰
--- isRaycastTarget 参数为nil的时候 默认为false
--- isRecursion 是否递归置灰 参数为nil的时候 默认为true
--- txtColor rrggbbaa
--- </summary>
function api:removeGray(target, txtColor, isRaycastTarget, isRecursion)

	isRecursion = isRecursion == nil and true or isRecursion
	
	api:loadPrefabSync(nil,"ui/common/material", "sprites_default")
	
	local mat = api:getAsset("ui/common/material", "sprites_default")

	if target:GetComponent("Image") ~= nil then
		
		
		local img = target:GetComponent("Image")
		img.material = mat
		img.raycastTarget = isRaycastTarget == nil and true or isRaycastTarget

	elseif target:GetComponent("SkeletonGraphic") ~= nil then
		
		local skeleton = target:GetComponent("SkeletonGraphic")
		skeleton.material = mat
		skeleton.raycastTarget = isRaycastTarget == nil and true or isRaycastTarget
		
	elseif target:GetComponent("Text") ~= nil then
		
		log("removeGray text", txtColor)
		local txt = target:GetComponent("Text")
		txt.color = txtColor == nil and api:hexToColor("000000ff") or api:hexToColor(txtColor)
		
	end
	
	
	
	if isRecursion == true then
		
		for i=0,target.transform.childCount - 1 do
			
			local child = target.transform:GetChild(i).gameObject
			
			if child:GetComponent("Image") ~= nil or 
				child:GetComponent("SkeletonGraphic") ~= nil or
				child:GetComponent("Text") ~= nil then
				
				api:removeGray(target.transform:GetChild(i).gameObject, txtColor, isRaycastTarget, isRecursion)
				
			end
			
		end
		
	end
	
end

--- <summary>
--- 加载配置
--- </summary>
function api:flyTip(tip)
	
	tip = tostring(tip)
	local DefaultTip = api:getAsset("prefabs/DefaultTip", "DefaultTip")
	DefaultTip:SetParent(SceneManager.CurrScene.TipLayer)
	DefaultTip:GetComponent("RectTransform").anchoredPosition = Vector2(0, 0)
	
	local Txt_tip = DefaultTip:Find("Txt_tip"):GetComponent("Text")
	local Img_bg = DefaultTip:Find("Img_bg"):GetComponent("Image")
	Txt_tip.text = tip
	
	local sequence = DOTween.Sequence()
	sequence:AppendInterval(0.8)
	sequence:Append(Txt_tip:DOFade(0, 0.5))
	sequence:Join(Img_bg:DOFade(0, 0.5))
	sequence:AppendCallback(function ()
		UnityEngine.GameObject.Destroy(DefaultTip.gameObject)
	end)
	
end

--- <summary>
--- TableView
--- scrollView scrollRect组件对象 不能为nil
--- itemHeight 行高 不能为nil
--- itemCount 总行数 初始化可以nil或是0
--- cellUpdateCallback 创建行或是更新行回调 不能为nil
--- newLoadingCallback 下拉或上拉请求新数据回调 可以不填
--- newLoadingLen 上拉或是上拉多少距离才回调 不填默认为60
--- </summary>
function api:createTableView(scrollView, itemHeight, itemCount, cellUpdateCallback, newLoadingCallback, newLoadingLen)
	if newLoadingLen == nil then
		newLoadingLen = 60
	end
	
	local tableView = UITableView.CreateTableView(scrollView.gameObject, itemHeight, newLoadingLen)
	tableView:setTableCellWithShowedDelegate(cellUpdateCallback)
	
	if itemCount ~= nil and itemCount > 0 then
		tableView:SetItemCount(itemCount)
	end
	
	if newLoadingCallback ~= nil then
		tableView:setNewLoadingDelegate(newLoadingCallback)
	end
	
	return tableView
	
end

--- <summary>
--- 进入主场景
--- </summary>
function api:enterMainScene()
	
	local preResList = {"prefabs/battlehero_canvas",
						"spineani/battleani/binnvc", 
						"spineani/battleskill/binci", 
						"prefabs/txt_battlenum_1", 
						"prefabs/txt_battlenum_2", 
						"prefabs/txt_battlenum_3", 
						"prefabs/txt_battlenum_4", 
						"prefabs/txt_battlenum_5",
						"prefabs/DefaultTip"
						}
	api:enterScene("GameScene", preResList, "GameScene", function (progress)
--		log("m:"..progress)
	end)
end

--- <summary>
--- 进入登录场景
--- </summary>
function api:enterLoginScene()
	
	api:enterScene("LoginScene", {}, "LoginScene", function (progress)
--		log("m:"..progress)
	end)
end


return api