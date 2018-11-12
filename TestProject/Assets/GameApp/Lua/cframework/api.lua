local baseapi = {}

function baseapi:getCurrScene()
	return SceneManager.CurrScene
end

function baseapi:getCurrSceneId()
	local scene = self:getCurrScene()
	if scene == nil then
		return nil
	end
	return scene.Id
end

function baseapi:enterScene(id, preloadList, classpath, progressCallback, loadingClassPath, loadingPreloadList)	
	local loadingView = nil
	if loadingClassPath ~= nil then
		loadingView = require("app.view."..loadingClassPath)
	else
		loadingView = {}
	end
	SceneManager:EnterScene(id, preloadList, require("app.view."..classpath), progressCallback, "LoadingWindow",
			require("app.view.window.loadingWindow"), false)
end

function baseapi:sendNotification(id, sid, data)
	NotificationManager:sendNotification(id, sid, data)
end

function baseapi:registerData(className)
	self[className] = DataManager:registerClass(className)
end

function baseapi:registerController(className)
	self[className] = ControllerManager:registerClass(className)
end

function baseapi:registerHandler(className)
	HandlerManager:registerClass(className)
end

function baseapi:registerPB(callback)
	NetworkManager:registerPB(callback)
end

function baseapi:sendCommand(commandId, data)
	NetworkManager:sendCommand(commandId, data)
end

function baseapi:getAsset(abName, assetName)
	return ResManager:GetAsset(string.lower(tostring(abName)), assetName)
end

function baseapi:loadPrefab(viewId, abName, assetName, callback)
	CFunc:LoadPrefab(viewId, abName, assetName, callback)
end

function baseapi:loadPrefabSync(viewId, abName, assetName)
	local ab = ResManager:LoadPrefab(viewId, abName)
	return ResManager:GetAsset(abName, assetName)
	
end

--- <summary>
--- 打开简单窗口
--- </summary>
function baseapi:openSimpleWindow(windowName, preRes, windowResName)
	
	windowResName = (windowResName == nil and windowName or windowResName)
	
	if preRes == nil then
		preRes = {string.lower("prefabs/"..windowResName)}
	else
		
		for k,v in pairs(preRes) do
			
			if string.sub(v,1,7) ~= "prefabs" and string.sub(v,1,7) ~= "dongtai"  and string.sub(v,1,2) ~= "ui"  then
				
				preRes[k] = "prefabs/"..string.lower(v)
				
			else
				
				preRes[k] = string.lower(v)
				
			end
		end
		
		table.insert(preRes,string.lower("prefabs/"..windowResName))
	end
	
	api:openWindow(windowName, "prefabs/"..string.lower(windowResName), windowResName, windowName, true, preRes)
	
end

function baseapi:openWindow(id, abName, assetName, classpath, isModal, preloadList, progressCallback, isClickExit, isCache)
	SceneManager:AddWindow(id, abName, assetName, preloadList, isModal, require("app.view.window."..classpath), progressCallback, isClickExit, isCache)
end

function baseapi:closeWindow(id, view, closeCallback)	
	local window = api.CommonData.popupList[id]
	if window == nil then
		return
	end
	
	window:close(closeCallback)
end

function baseapi:getWindow(id)
	return SceneManager:GetWindow(id)
end

function baseapi:addEvent(viewId, eventId, target, callback)

	local function __callback(target, param)		
		callback(target, param)
	end

	SceneManager:AddEvent(viewId, eventId, target, __callback)
end

function baseapi:newSpine(viewId, parent, abName, assetName, callback, actionName, isloop)
	if actionName == nil then
		actionName = ""
	end
	if isloop == nil then
		isloop = true
	end
	CFunc:NewSpineAni(viewId, parent, abName, assetName, callback, actionName, isloop)
end

function baseapi:newUISpine(viewId, parent, abName, assetName, callback, actionName, isloop, hasButton, objName)
	if actionName == nil then
		actionName = ""
	end
	if isloop == nil then
		isloop = true
	end
	if hasButton == nil then
		hasButton = false
	end
	CFunc:NewUISpineAni(viewId, parent.gameObject, abName, assetName, callback, actionName, isloop, hasButton, objName)
end

function baseapi:addTimer(id, interval, updateCallback, isNowRun)
	if isNowRun == nil then
		isNowRun = false
	end
	CFunc:AddTimer(id, interval, updateCallback, isNowRun)
end

function baseapi:removeTimer(id)
	CFunc:RemoveTimer(id)
end

function baseapi:readTxtFile(path)
	log(path)
	local file = io.open(path, "r")
	local txt = file:read("*a")
	file:close()
	return txt
end

function baseapi:newSpineSync(viewId, parent, abName, assetName, actionName, isLoop)
	
	return CFunc:NewSpineAniSync(viewId, parent, abName, assetName, actionName, isloop)
end

function baseapi:setSortingLayer(sprite, sortingLayerName, order)
	local renderer = sprite:GetComponent("Renderer")
	renderer.sortingLayerName = sortingLayerName
	renderer.sortingOrder = order
end




return baseapi