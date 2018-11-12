require("cframework.protobuf.protobuf")

local TcpNetworkManager = class("TcpNetworkManager")

function TcpNetworkManager:ctor()	
	self.id = nil
	self.ip = nil
	self.port = nil
	self.isConnecting = false
	self.isConnected = false
	self.isReconnect = false
	self.reconnectState = false	
	self.pbCommandDic = {}
	self.handlerMap = {}
	self.waitCmdList = {}
	self.connectedCallback = nil
	self.gameTime = 0
	self.sTime = 0
	self.lastAliveTime = 0

--[[	self:registerHandler(10204, handler(self, self.onSendCallback))
	self:registerHandler(10202, handler(self, self.onAliveCallback))	--]]
	
end

function TcpNetworkManager:connect(ip, port, callback)

	if self.isConnecting == true or self.reconnectState == true then
		return
	end	
	
	self.ip = ip
	self.port = port

	self.isConnecting = true
	self.connectedCallback = callback
	
--	api:sendNotification(Message.NETWORK, Message.NETWORK_TYPE_WAIT_START, true)
	
	TcpClientManager:Connect(ip, port,handler(self, self.onConnectedCallback), handler(self, self.onConnectErrorCallback),
						handler(self, self.onDisconnectedCallback), handler(self, self.onData))
end

function TcpNetworkManager:disconnect()
	self.isReconnect = false
	self.reconnectState = false
	TcpClientManager:Disconnect()
end

function TcpNetworkManager:registerPB(callback)

	local function __process(byteArray)
		log("注册pb", tostring(byteArray))
		protobuf.register(byteArray:ReadBuffer())
	end
--[[	local function __process(path)
		protobuf.register_file(path)
	end--]]
	
	local function __complete(pClassCommandList)		
	    for i = 0, pClassCommandList.Length - 1, 2 do
			log("pbCommandDic:"..tonumber(pClassCommandList[i])..","..pClassCommandList[i+1])
			self.pbCommandDic[tonumber(pClassCommandList[i])] = pClassCommandList[i+1]
		end
		callback()
	end
	
	TcpClientManager:RegisterPB(__process, __complete)

end

function TcpNetworkManager:onConnectedCallback()	
	
	self.waitCmdList = {}	
	
--	api:sendNotification(Message.NETWORK, Message.NETWORK_TYPE_WAIT_END)
	
	self.isConnected = true
	self.isConnecting = false

	if self.connectedCallback ~= nil then
		self.connectedCallback()
	end
	
	self.reconnectState = true	
	
	self.lastAliveTime = os.time()
	
end

function TcpNetworkManager:onConnectErrorCallback(state)
	if state == 34 then
		TcpClientManager:Reconnect()
	end
end

function TcpNetworkManager:onDisconnectedCallback(state)	

	self.isConnecting = false
	self.isConnected = false

	if self.isReconnect == true and state ~= 24 then
		
		api:sendNotification(Message.NETWORK, Message.NETWORK_TYPE_WAIT_START, true)
		
		TcpClientManager:Reconnect()
		
	end
	
end


function TcpNetworkManager:onData(commandId, pbBuffer)

	if commandId ~= 10202 then
		log("服务端指令"..commandId)	
		log("服务端指令"..commandId)		
	end
	
    local pbClassName = self.pbCommandDic[commandId]

    if pbClassName == nil then

        logWarn("未注册的PBClass:"..pbClassName)

        return

    end
	log("pbClassName："..pbClassName)	

    local result = nil

    if pbBuffer ~= 0 then
        result = protobuf.decode(pbClassName, pbBuffer)
    end
	
	log("result:"..type(result))
	log("account:"..result.account)
	log("sign:"..result.sign)
	log("channel:"..result.channel)

    local callback = self.handlerMap[commandId]

    if callback == nil then
        logWarn("未注册的Handler:"..commandId)
        return
    end

    local fun = function()
        callback(result)
--		api:sendNotification(Message.GUIDE, Message.GUIDE_TYPE_COMMAND, commandId)
    end

    xpcall(fun, __G__TRACKBACK__)

end

function TcpNetworkManager:sendCommand(commandId, data)

	if self.isConnected == false then
		logError("服务器未连接")
		return
	end
	
	if self.waitCmdList[commandId] ~= nil then
		logWarn("指令"..commandId.."在等待队列中，发送取消")
		return
	end

	self.waitCmdList[commandId] = self:getGameTime()
	
--	api:sendNotification(Message.NETWORK, Message.NETWORK_TYPE_WAIT_START, false)

    local pbClassName = self.pbCommandDic[commandId]

    if pbClassName == nil then

        logWarn("未注册的PBClass:"..pbClassName)

        return

    end

	if data == nil then
		TcpClientManager:SendCommand(commandId, nil)
	else		
		log("开始组装数据",pbClassName)
		dump(data)
		local pbBuffer = protobuf.encode(pbClassName, data)
		log(tostring(pbBuffer))
		local buffer = ByteBuffer.New(false)
		buffer:WriteBuffer(pbBuffer)
		
		TcpClientManager:SendCommand(commandId, buffer)
	end 
	
	log("客户端指令"..commandId)
	
end

function TcpNetworkManager:onSendCallback(data)	
	self.waitCmdList[data.proto_id] = nil
	if table.nums(self.waitCmdList) == 0 then
		api:sendNotification(Message.NETWORK, Message.NETWORK_TYPE_WAIT_END)
	end
end

function TcpNetworkManager:registerHandler(commandId, callback)
	if self.handlerMap[commandId] ~= nil then
		logError("重复注册的handler:"..commandId)
		return
	end
    self.handlerMap[commandId] = callback
end

function TcpNetworkManager:removeHandler(commandId)    
    self.handlerMap[commandId] = nil
end

function TcpNetworkManager:clearWaitCmdList()
	self.waitCmdList = {}
end

function TcpNetworkManager:setGameTime(t)
	self.gameTime = t
	self.sTime = os.time() * 1000000
end

function TcpNetworkManager:getGameTime()
	return self.gameTime + (os.time() * 1000000 - self.sTime)
end

function TcpNetworkManager:disableReconnect()
	self.isReconnect = false
end

return TcpNetworkManager