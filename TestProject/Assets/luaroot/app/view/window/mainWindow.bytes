local MainWindow = class("MainWindow", import("cframework.mvc.BaseWindow"))

function MainWindow:Start()
	log("MainWindow start")
	self:addEvent(EventType.Click, self.uiRoot.Btn_bag, handler(self, function ()
		api:openSimpleWindow("BagWindow", {"prefabs/BagWindow_Item"})
	end))
	
	self:addEvent(EventType.Click, self.uiRoot.Btn_task, handler(self, function ()
		api:openSimpleWindow("TaskWindow", {"prefabs/TaskWindow_Item"})
	end))
	
	self:addEvent(EventType.Click, self.uiRoot.Btn_hero, handler(self, function ()
		api:openSimpleWindow("HeroWindow")
	end))
	
	self:addEvent(EventType.Click, self.uiRoot.Btn_sdk, handler(self, function ()
		Sdk:callSdk(10, function (content)
			log(content)
		end)
	end))
	
	self:bindData()
end

function MainWindow:onEnterTransitionFinish()
end

function MainWindow:listNotificationInterests()
    return {Message.BATTLE}
end

function MainWindow:handleNotification(id, sid, data)
	
	if sid == Message.BEAUTY_GET_TEACH then
	end
--	
end

function MainWindow:bindData()

end

function MainWindow:onClose(target)
	--target 事件对象
	log("on exit---------------------------")
	api:closeWindow(self:getId())
end

function MainWindow:Dispose()
	log("dispose")
end

return MainWindow