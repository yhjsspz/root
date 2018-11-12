local TaskWindow = class("TaskWindow", import("cframework.mvc.BaseWindow"))

--------------------
--- 窗口 ---
--------------------

--- <summary>
--- 初始化
--- </summary>
function TaskWindow:Start()
	
	self:addEvent(EventType.Click, self.uiRoot.Btn_close, handler(self, self.doClose))
	
	self:bindList()
	
end

--- <summary>
--- 注册消息
--- </summary>
function TaskWindow:listNotificationInterests()

    return {}

end

--- <summary>
--- 消息处理
--- </summary>
function TaskWindow:handleNotification(id, sid, data)

	if sid == Message.ACTIVITY_ON_LOTTERY_PLAY_ANI then
		
		
	end
	
end

--- <summary>
--- 刷新界面数据消息(用于可多次刷新)
--- </summary>
function TaskWindow:bindData()


end

--- <summary>
--- 绑定列表
--- </summary>
function TaskWindow:bindList(list)
	
	if list == nil then
		self.showList = {}
		for i=1,10 do
			table.insert(self.showList, {Id = i, Msg = "等级提升至LV."..i, Status = i % 3, Percent = i*10, ItemList = {{Id = 1001, Num = i*100, ItemType = 1}, {Id = 1003, Num = i * 10, ItemType = 1}}})
		end
	else
		self.showList = list
	end
	
    local row_count = math.ceil(#self.showList /1)
	log("row_count", row_count)

	if self.tableView == nil then
		
		self.tableView = api:createTableView(self.uiRoot.Pnl_content, 110, row_count, handler(self,self.bindListRow))
		self.tableView:SetItemSkin(api:getAsset("prefabs/TaskWindow_Item", "TaskWindow_Item").gameObject)

	else
		self.tableView:SetItemCount(row_count)
	end

	self.tableView:UpdateView()

end

function TaskWindow:bindListRow(cell, index, isNew)
	
--	log("bindListRow", index, isNew)
	
	local Txt_Fix_task = cell:GetChild("Txt_Fix_task")
	local Sli_schedule = cell:GetChild("Sli_schedule")
	local Btn_go = cell:GetChild("Btn_go")
	local Btn_get = cell:GetChild("Btn_get")
	local Img_reveived = cell:GetChild("Img_reveived")
	
	local itemInfo = self.showList[index + 1]
	
	api:setText(Txt_Fix_task, itemInfo.Msg)
	api:setSliderValue(Sli_schedule, itemInfo.Percent)
	
	if itemInfo.Status == 0 then
		-- 0：前往
		Btn_go:SetActive(true)
		Btn_get:SetActive(false)
		Img_reveived:SetActive(false)
	elseif itemInfo.Status == 1 then
		-- 1：领取
		Btn_go:SetActive(false)
		Btn_get:SetActive(true)
		Img_reveived:SetActive(false)
	elseif itemInfo.Status == 2 then
		-- 2：完成
		Btn_go:SetActive(false)
		Btn_get:SetActive(false)
		Img_reveived:SetActive(true)
	end
	
	for i=1,2 do
		
		local Pnl_item = cell:GetChild("Item_Award"..i)
		if itemInfo.ItemList[i] == nil then
			Pnl_item:SetActive(false)
		else
			
			Pnl_item:SetActive(true)
			
			local Btn_props = cell:GetChild("Btn_props"..i)
			local Txt_Fix_num = cell:GetChild("Txt_Fix_num"..i)
			
			api:setImage(self:getId(), Btn_props, "dongtai/item/item_"..itemInfo.ItemList[i].Id, "item_"..itemInfo.ItemList[i].Id)
			api:setText(Txt_Fix_num, itemInfo.ItemList[i].Num)

		end
	
	end
	
	if isNew == true then
	
		cell:AddClickEvent(self:getId(), cell.name, "Btn_go", function()
			
			--前往
			local itemInfo = self.showList[cell.CellIndex + 1]
			api:flyTip(cell.CellIndex)
			
		end)
		cell:AddClickEvent(self:getId(), cell.name, "Btn_get", function()
			
			--领取
			local itemInfo = self.showList[cell.CellIndex + 1]
			api:flyTip(cell.CellIndex)
			
		end)
	end
end
-------------------- 事件 --------------------

--- <summary>
--- 关闭窗口
--- </summary>
function TaskWindow:doClose(target)

	self:close()

end

--- <summary>
--- 注销窗口
--- </summary>
function TaskWindow:Dispose()
	
	
end

return TaskWindow