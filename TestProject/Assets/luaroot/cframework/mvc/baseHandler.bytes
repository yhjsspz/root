local BaseHandler = class("BaseHandler")

function BaseHandler:ctor()
     for name in pairs(self.class) do          
        if string.match(name, "HANDLER_") ~= nil and string.match(name, "HANDLER_") ~= "" then

            local param = string.split(name,"_")
            local commandId = param[2]

            NetworkManager:registerHandler(tonumber(commandId), handler(self, self[name]))
        end
    end	
end

return BaseHandler