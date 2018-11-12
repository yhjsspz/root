local ControllerManager = class("ControllerManager")

function ControllerManager:ctor()
    self.DataClassCache = {}
end

function ControllerManager:registerClass(className)	
	if self.DataClassCache[className] == nil then	    
        self.DataClassCache[className] = require("app.controller."..className).new()
    end
	return self.DataClassCache[className]
end

return ControllerManager