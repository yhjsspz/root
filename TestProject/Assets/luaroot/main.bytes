require "cframework.init"

--主入口函数。从这里开始lua逻辑
function Main()			
	
	logWarn("lua虚拟机启动成功.")	
	
	
	api:registerPB(function()
	
		log("PB注册成功")	
		
--[[		NetworkManager:connect("127.0.0.1", 8088, function ()
			log("网络连接成功")
--			NetworkManager:sendCommand(10101,nil);
			NetworkManager:sendCommand(10101,{account = 1, sign = "cb", channel="cn"});
		end)
		--]]
	end)
	
	Sdk = require("app/Sdk")
	
	--注册Data
	api:registerData("CommonData")
	api:registerData("ItemData")
	api:registerData("BattleData")
	api:registerData("HeroData")
		
	-- 注册Controller
	api:registerController("BattleController")
	
	-- 注册Handler
--	api:registerHandler("BattleHandler")

	api.BattleData.team1 = {hero_list = {{id = 22001,pos = 1, hp=100, ap=100, total_hp=100, total_ap = 100}}}
	api.BattleData.team2 = {hero_list = {{id = 22001,pos = 1, hp=100, ap=100, total_hp=100, total_ap = 100}}}
	api.BattleData.round = 0
	api.BattleData.roundList = {{round = 1, action_list = {{action_type = 0}
														,{action_type = 2, team_id = 1,param1 = 22001,param2=1,param3=0,param4=1}
														,{action_type = 16}
														,{action_type = 3, team_id = 1,param1 = 1,param2=0,param3=76,param4="2"}
														,{action_type = 3, team_id = 2,param1 = 1,param2=1,param3=76,param4="0"}
														,{action_type = 16}
														,{action_type = 15}
											}
								}}
	api.BattleData.roundListClone = clone(api.BattleData.roundList)
	
	api:enterLoginScene()
--	SdkFunc:CsTest("aaa")
end

function Start()

	
end