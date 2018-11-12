local DataManager = class("DataManager")

function DataManager:ctor()
	self.DataClassCache = {}
end

function DataManager:registerClass(className)	
	local dataClass = nil
	if self.DataClassCache[className] == nil then	    
        dataClass = require("app.model."..className).new()
		self.DataClassCache[className] = dataClass
    end

	if dataClass ~= nil and dataClass.onInit then
		dataClass:onInit()
	end
	
	return dataClass
end

function DataManager:reset()
	for k, data in pairs(self.DataClassCache) do
        if data.reset ~= nil then
            --data:reset()
        end
    end
end

return DataManager