local BaseWindow = class("BaseWindow", import("cframework.mvc.BaseView"))

function BaseWindow:OnAwake(view)
	log("BaseWindow:OnAwake");
	self.view = view
--	self.isClickExit = view.IsClickExit
--	self.view.IsClickExit = false
	api.CommonData.popupList[self:getId()] = self	
end

function BaseWindow:OnStart()
	self:registerNotification()
	if self.Start ~= nil then
		self:Start()
	end
	if self.view == nil then
		return
	end
	self:doStartTransition()
end

function BaseWindow:onExit()
	self:playSceneMusic()
end

function BaseWindow:doStartTransition()	
--[[	if self:isFullScreen() == true then
		self.isFullWindow = true
		self:doStartTransitionFinish()
	else
		self.view.UiRoot.transform.localScale = Vector3(0.9,0.9,0.9)
		self.view.UiRoot.transform:DOScale(1,0.2):SetEase(DG.Tweening.Ease.InOutBack):OnComplete(handler(self, self.doStartTransitionFinish))	
	end--]]
end

function BaseWindow:doStartTransitionFinish()
--[[	self:onEnterTransitionFinish()

	self.view.IsClickExit = self.isClickExit

	self:playMusic()
	
	if self.isFullWindow == true then
		api.CommonData.currSubScene.view.UiRoot:SetActive(false)
		SceneManager:HideUI(nil)
	end
	
	api:removeTimer(Const.TIMER_ID_GUIDE_WAIT.."WINDOW"..self:getId())
	api:setTimerout(Const.TIMER_ID_GUIDE_WAIT.."WINDOW"..self:getId(), 0.1, function ()
		api:sendNotification(Message.GUIDE, Message.GUIDE_TYPE_OPENWINDOW, self:getId())
		api:sendNotification(Message.HOT_DOT, Message.HOT_DOT_TYPE_OPENWINDOW, self)
		end)--]]
end

function BaseWindow:onEnterTransitionFinish()
end

function BaseWindow:doEndTransition()
	--self.view.UiRoot.transform:DOScale(0,0.5):SetEase(DG.Tweening.Ease.InOutCirc):OnComplete(handler(self, self.doEndTransitionFinish))
	self:doEndTransitionFinish()
end

function BaseWindow:doEndTransitionFinish()
	self:onEndTransitionFinish()
end

function BaseWindow:onEndTransitionFinish()	
	if self.isFullWindow == true then
		api.CommonData.currSubScene.view.UiRoot:SetActive(true)
		SceneManager:ShowUI(nil)
	end
	
	if api:getWindow(self:getId()) ~= nil then
		SceneManager:RemoveWindow(self:getId())
	else
--		SceneManager:RemoveSysPopup(self:getId())
	end
	if self.closeCallback ~= nil then
		self.closeCallback()
		self.closeCallback = nil
	end
--	api:sendNotification(Message.GUIDE, Message.GUIDE_TYPE_CLOSEWINDOW, self:getId())
end

function BaseWindow:close(closeCallback)	
	if api.CommonData.popupList[self:getId()] == nil then
		return
	end
	self.closeCallback = closeCallback
	api.CommonData.popupList[self:getId()] = nil
	self:doEndTransition()
end

function BaseWindow:isFullScreen()
	
	if FullScreenWindowList == nil then return false end
	
	for i = 1, #FullScreenWindowList do
		if FullScreenWindowList[i] == self:getId() then
			return true
		end
	end
	return false
end

return BaseWindow