local ItemData = class("ItemData", import("cframework.mvc.BaseData"))

--------------------
--- 物品道具Data ---
--------------------

ItemData.itemList = nil

function ItemData:onInit()

	self.itemList = {}
	
	for i=1,40 do
		
		table.insert(self.itemList, {id=i, num=i})
		
	end

end

return ItemData