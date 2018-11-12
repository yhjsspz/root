local Window1 = class("Window1", import("cframework.mvc.BaseWindow"))

function Window1:Start()
	log("Window1 start")
	self:addEvent(EventType.Click, self.uiRoot.Btn_close, handler(self, self.onClose))
	
	self:bindData()
end

function Window1:onEnterTransitionFinish()
end

function Window1:listNotificationInterests()
    return {Message.BATTLE}
end

function Window1:handleNotification(id, sid, data)
	
	if sid == Message.BEAUTY_GET_TEACH then
	end
--	
end

function Window1:bindData()

end

function Window1:onClose(target)
	--target 事件对象
	log("on exit---------------------------")
	api:closeWindow(self:getId())
end

function Window1:Dispose()
	log("dispose")
end

return Window1