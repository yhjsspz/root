local BattleWindow = class("BattleWindow", import("cframework.mvc.BaseWindow"))

function BattleWindow:Start()
	log("BattleWindow start")
	self:addEvent(EventType.Click, self.uiRoot.Btn_replay, handler(self, function ()
		api.BattleController:replay()
	end))
	
	self:bindData()
end

function BattleWindow:onEnterTransitionFinish()
end

function BattleWindow:listNotificationInterests()
    return {Message.BATTLE}
end

function BattleWindow:handleNotification(id, sid, data)
	
	if sid == Message.BEAUTY_GET_TEACH then
	end
--	
end

function BattleWindow:bindData()

end

function BattleWindow:onClose(target)
	--target 事件对象
	log("on exit---------------------------")
	api:closeWindow(self:getId())
end

function BattleWindow:Dispose()
	log("dispose")
end

return BattleWindow