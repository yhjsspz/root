local BagWindow = class("BagWindow", import("cframework.mvc.BaseWindow"))

--------------------
--- 窗口 ---
--------------------

--- <summary>
--- 初始化
--- </summary>
function BagWindow:Start()
	
	self:addEvent(EventType.Click, self.uiRoot.Btn_close, handler(self, self.doClose))
	
	self:bindList()
	
end

--- <summary>
--- 注册消息
--- </summary>
function BagWindow:listNotificationInterests()

    return {}

end

--- <summary>
--- 消息处理
--- </summary>
function BagWindow:handleNotification(id, sid, data)

	if sid == Message.ACTIVITY_ON_LOTTERY_PLAY_ANI then
		
		
	end
	
end

--- <summary>
--- 刷新界面数据消息(用于可多次刷新)
--- </summary>
function BagWindow:bindData()


end

--- <summary>
--- 绑定列表
--- </summary>
function BagWindow:bindList(list)
	
	if list == nil then
		self.showList = {}
		
		for i=1,20 do
			table.insert(self.showList, {Id = 55111+(i%10), Num = i, ItemType = 1})
		end
	else
		self.showList = list
	end
	
	self.cellNum = 8
    local row_count = math.ceil(#self.showList /self.cellNum)

	if self.tableView == nil then
		
		self.tableView = api:createTableView(self.uiRoot.Scv_item, 110, row_count, handler(self,self.bindListRow))
		self.tableView:SetItemSkin(api:getAsset("prefabs/BagWindow_Item", "BagWindow_Item").gameObject)
		
	else
		self.tableView:SetItemCount(row_count)
	end

	self.tableView:UpdateView()
	
end

function BagWindow:bindListRow(cell, index, isNew)
	
	for i = 1, self.cellNum do

		local item_cell = index * self.cellNum + i
		local itemInfo = self.showList[item_cell]
		
		local Item = cell:GetChild("Item"..i)
		
		if itemInfo == nil then
			
			Item:SetActive(false)
		else

			Item:SetActive(true)
		
			
			local Btn_icon = cell:GetChild("Btn_icon"..i)
			local Txt_num = cell:GetChild("Txt_num"..i)
			
			api:setImage(self:getId(), Btn_icon, "dongtai/item/item_"..itemInfo.Id, "item_"..itemInfo.Id)
			api:setText(Txt_num, itemInfo.Num)
			
			
			-- 注册事件

			if isNew == true then
				
				cell:AddClickEvent(self:getId(), cell.name, "Btn_icon"..i, function ()
					--任务详情
					
					local item_cell = cell.CellIndex * cellNum + i
					api:flyTip(item_cell)
					
				end)
				
			end
		end


	end
	
	
end
-------------------- 事件 --------------------

--- <summary>
--- 关闭窗口
--- </summary>
function BagWindow:doClose(target)

	self:close()

end

--- <summary>
--- 注销窗口
--- </summary>
function BagWindow:Dispose()
	
	
end

return BagWindow