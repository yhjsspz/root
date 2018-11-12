local BaseView = class("BaseView")
BaseView.view = nil
BaseView.uiRoot = nil

function BaseView:OnAwake(view)
	self.view = view
	self.musicUrl = nil
end

function BaseView:OnStart()
	self:registerNotification()
	if self.Start ~= nil then
		self:Start()
	end
	if self.UIStart ~= nil then
		self:UIStart()
	end
	if self.view == nil then
		return
	end
	self:playMusic()
end

function BaseView:Destroy()	
	self:removeNotification()
	if self.Dispose ~= nil then
		self:Dispose()
	end	
	if self.onExit ~= nil then
		self:onExit()
	end
end

function BaseView:playMusic()
	if api.GuideData.isInPlayerVide == true then
		return
	end
	
	local viewId = self:getId()
	if viewId == nil then
		return
	end
	local url = api.MusicData:getSrc(1, viewId)

	if url ~= nil and url ~= "" then
		self.musicUrl = url
		api:playMusic(url, true)
	end
end

function BaseView:playSceneMusic()

	if self.musicUrl == nil then
		return
	end
	
	if SceneManager.CurrScene == nil or SceneManager.CurrScene.SubScene == nil then
		return
	end
	
	local subSceneId = SceneManager.CurrScene.SubScene.Id
	if subSceneId == nil then
		return
	end

	local url = api.MusicData:getSrc(1, subSceneId)

	if url ~= nil and url ~= "" then
		api:playMusic(url, true)
	end
end


function BaseView:getView()
	if self.view ~= nil then
		return self.view.UiRoot
	end
	return null
end

function BaseView:addEvent(eventId, target, callback)
	if self.view == nil then
		return
	end
	api:addEvent(self.view.Id, eventId, target, callback)
end

function BaseView:removeEvent(eventId, target)
	if self.view == nil then
		return
	end
	api:removeEvent(self.view.Id, eventId, target)
end

function BaseView:addCellClickEvent(cell, tableViewId, targetName, callback)
	api:addCellClickEvent(self.view.Id, cell, tableViewId, targetName, callback)
end

function BaseView:getId()
	if self.view == nil then
		return ""
	end
	return self.view.Id
end

function BaseView:UpdateUIControl(idList, objList)
	self.uiRoot = {}
	for i = 0, idList.Length - 1 do
--		log("------------:", idList[i], objList[i])
		self.uiRoot[idList[i]] = objList[i]
	end
end

function BaseView:registerNotification()

    if self.listNotificationInterests == nil then

        return

    end

    local interests = self:listNotificationInterests()

    if interests == nil or #interests == 0 then

        return

    end

    for k, v in pairs(interests) do

        NotificationManager:registerObserver(v, self, self.handleNotification)

    end

end

function BaseView:removeNotification()

    if self.listNotificationInterests == nil then

        return

    end

    local interests = self:listNotificationInterests()

    if #interests == 0 then

        return

    end

    for k, v in pairs(interests) do

        NotificationManager:removeObserver(v, self, self.handleNotification)

    end

end

function BaseView:getChild(name)
	if self.view ~= nil then
		return self.view:GetChild(name)
	end	
	return null
end

function BaseView:addChild(parent, child, preStr)
	
	if self.view == nil then
		return		
	end

	return self.view:AddChild(parent, child, preStr)
end

function BaseView:removeChild(child)
	self.view:RemoveChild(child)
end

function BaseView:removeAllChild(parent)
	self.view:RemoveAllChild(parent)
end

function BaseView:loadPrefab(abName, assetNameList, callback)
	api:loadPrefab(self.view.Id, abName, assetNameList, callback)
end

function BaseView:addScrollButonEvent(go, scrollView, onClick, normalColor, selectColor, imgTarget)
	api:addScrollButonEvent(self.view.Id, go, scrollView, onClick, normalColor, selectColor, imgTarget)
end

return BaseView