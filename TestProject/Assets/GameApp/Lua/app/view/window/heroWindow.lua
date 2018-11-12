local HeroWindow = class("HeroWindow", import("cframework.mvc.BaseWindow"))

--------------------
--- 窗口 ---
--------------------

--- <summary>
--- 初始化
--- </summary>
function HeroWindow:Start()
	
	self:addEvent(EventType.Click, self.uiRoot.Btn_close, handler(self, self.doClose))
	
	self:bindData()
end

--- <summary>
--- 注册消息
--- </summary>
function HeroWindow:listNotificationInterests()

    return {}

end

--- <summary>
--- 消息处理
--- </summary>
function HeroWindow:handleNotification(id, sid, data)

	if sid == Message.ACTIVITY_ON_LOTTERY_PLAY_ANI then
		
		
	end
	
end

--- <summary>
--- 刷新界面数据消息(用于可多次刷新)
--- </summary>
function HeroWindow:bindData()

	api:newUISpine("GameScene", self.uiRoot.Pnl_hero,"spineani/battleani/houwangc", "houwangc",function ()
		log("end")
	end,"stand")

end

-------------------- 事件 --------------------

--- <summary>
--- 关闭窗口
--- </summary>
function HeroWindow:doClose(target)

	self:close()

end

--- <summary>
--- 注销窗口
--- </summary>
function HeroWindow:Dispose()
	
	
end

return HeroWindow