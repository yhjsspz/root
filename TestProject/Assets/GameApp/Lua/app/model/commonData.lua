local CommonData = class("CommonData", import("cframework.mvc.BaseData"))

function CommonData:onInit()
	self.popupList = {}
	self.defaultTipList = {}
end

return CommonData