local HandlerManager = class("HandlerManager")

function HandlerManager:ctor()
    self.DataClassCache = {}
end

function HandlerManager:registerClass(className)	
	if self.DataClassCache[className] == nil then	    
        self.DataClassCache[className] = require("app.handler."..className).new()
    end
	return self.DataClassCache[className]
end

return HandlerManager