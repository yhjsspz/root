local LoginWindow = class("LoginWindow", import("cframework.mvc.BaseWindow"))

--------------------
--- 登录窗口 ---
--------------------

--- <summary>
--- 初始化
--- </summary>
function LoginWindow:Start()
	
--	self:addEvent(EventType.Click, self.uiRoot.Btn_close, handler(self, self.doClose))
	self:addEvent(EventType.Click, self.uiRoot.Btn_enter, handler(self, function ()
		log("text", self.uiRoot.Ipt_name:GetComponent("InputField").text)
		
--		api:sendCommand(10201, {SysTime=111})
		
		api:enterMainScene()
	end))
	
	
end

--- <summary>
--- 注册消息
--- </summary>
function LoginWindow:listNotificationInterests()

    return {}

end

--- <summary>
--- 消息处理
--- </summary>
function LoginWindow:handleNotification(id, sid, data)

	if sid == Message.ACTIVITY_ON_LOTTERY_PLAY_ANI then
		
		
	end
	
end

--- <summary>
--- 刷新界面数据消息(用于可多次刷新)
--- </summary>
function LoginWindow:bindData()



end

-------------------- 事件 --------------------

--- <summary>
--- 关闭窗口
--- </summary>
function LoginWindow:doClose(target)

	self:close()

end

--- <summary>
--- 注销窗口
--- </summary>
function LoginWindow:Dispose()
	
	
end

return LoginWindow