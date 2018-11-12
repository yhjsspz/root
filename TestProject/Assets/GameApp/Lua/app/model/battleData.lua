local BattleData = class("BattleData", import("cframework.mvc.BaseData"))

BattleData.BattleGroundConfig = nil			-- 战斗场地配置
BattleData.BattleBgConfig = nil				-- 战斗背景配置

function BattleData:onInit()
	self.skillConfig = {}
	self.battle_type = nil
	self.battle_setting = nil
	self.winner = nil
	self.roundList = nil
    self.actionList = nil
	self.currAction = nil
    self.round = 0
	self.team1 = nil
	self.team2 = nil
	
	self.BattleGroundConfig = api:loadConfig("BattleGroundConfig")
	self.BattleBgConfig = api:loadConfig("BattleBgConfig")
--	
	
end

function BattleData:getSkillConfig(skillId)
	local skillActionList = self.skillConfig[skillId]
	if skillActionList == nil then
		local path
		
		log("IS_DEBUG:"..tostring(AppConst.IS_DEBUG))
		path = FileUtil:GetScriptFilePath("/GameApp/Lua/config/battleSkill/"..skillId..".json")

		if AppConst.IS_DEBUG == true then
			path = FileUtil:GetScriptFilePath("GameApp/Lua/config/battleSkill/"..skillId..".json")
		else
			path = FileUtil:GetPackageFilePath("src/gameapp/lua/battleskill/"..skillId..".json")
		end
		
		local jsonFile = api:readTxtFile(path)
		skillActionList = cjson.decode(jsonFile)
		
		self.skillConfig[skillId] = skillActionList
	end	
	return skillActionList
end

function BattleData:getCampHaloImg(campLvId)
	return api.HeroData.HeroCampAuraConfig[api.HeroData.HeroCampAuraLvConfig[campLvId].aura_id].icon
	
end

function BattleData:test()
	self.team1 = {}
	self.team1.hero_list = {}
	for i = 1, 6 do
		local heroInfo = {}
		heroInfo.id = 1
		heroInfo.pos = i
		table.insert(self.team1.hero_list, heroInfo)
	end
	
	self.team2 = {}
	self.team2.hero_list = {}
	for i = 1, 6 do
		local heroInfo = {}
		heroInfo.id = 2
		heroInfo.pos = i
		table.insert(self.team2.hero_list, heroInfo)
	end

	-------------------------------------------	
	
	self.roundList = {}
	
	local round1 = {}
	round1.round = 1
	round1.action_list = {}	
	
	local action1 = {}
	action1.action_id = 1
	action1.action_type = 0
	action1.pre_action = 0
	action1.team_id = 1
	action1.param1 = 0
	action1.param2 = 0
	action1.param3 = 0
	action1.param4 = ""
	
	table.insert(round1.action_list, action1)
	
	local round2 = {}
	round2.round = 2
	round2.action_list = {}	
	
	local action1 = {}
	action1.action_id = 1
	action1.action_type = 1
	action1.pre_action = 0
	action1.team_id = 2
	action1.param1 = 0
	action1.param2 = 1
	action1.param3 = 0
	action1.param4 = "1"

	local action2 = {}
	action2.action_id = 2
	action2.action_type = 16
	action2.pre_action = 1	

	local action3 = {}
	action3.action_id = 3
	action3.action_type = 3
	action3.pre_action = 1
	action3.team_id = 1
	action3.param1 = 1
	action3.param2 = 1
	action3.param3 = 100
	action3.param4 = ""	
	
	local action4 = {}
	action4.action_id = 2
	action4.action_type = 16
	action4.pre_action = 1	
	
	local action5 = {}
	action5.action_id = 4
	action5.pre_action = 1
	action5.team_id = 2
	action5.action_type = 15
	action5.param2 = 1
	
	table.insert(round2.action_list, action1)
	table.insert(round2.action_list, action2)
	table.insert(round2.action_list, action3)
	table.insert(round2.action_list, action4)	
	table.insert(round2.action_list, action5)
	
	------------------------------------------

	local round3 = {}
	round3.round = 3
	round3.action_list = {}
	
	local action1 = {}
	action1.action_id = 1
	action1.action_type = 2
	action1.pre_action = 0
	action1.team_id = 1
	action1.param1 = 1
	action1.param2 = 2
	action1.param3 = 0
	action1.param4 = "2,3,6"

	local action2 = {}
	action2.action_id = 2
	action2.action_type = 16
	action2.pre_action = 1	

	local action3 = {}
	action3.action_id = 3
	action3.action_type = 3
	action3.pre_action = 1
	action3.team_id = 2
	action3.param1 = 2
	action3.param2 = 1
	action3.param3 = 100
	action3.param4 = ""		
	
	local action4 = {}
	action4.action_id = 2
	action4.action_type = 16
	action4.pre_action = 1	

	local action5 = {}
	action5.action_id = 3
	action5.action_type = 3
	action5.pre_action = 1
	action5.team_id = 2
	action5.param1 = 3
	action5.param2 = 1
	action5.param3 = 100
	action5.param4 = ""	

	local action7 = {}
	action7.action_id = 2
	action7.action_type = 16
	action7.pre_action = 1		

	local action8 = {}
	action8.action_id = 3
	action8.action_type = 3
	action8.pre_action = 1
	action8.team_id = 2
	action8.param1 = 6
	action8.param2 = 1
	action8.param3 = 100
	action8.param4 = ""		

	local action9 = {}
	action9.action_id = 2
	action9.action_type = 16
	action9.pre_action = 1		
	
	local action10 = {}
	action10.action_id = 4
	action10.pre_action = 1
	action10.team_id = 1
	action10.action_type = 15
	action10.param2 = 2
	
	table.insert(round3.action_list, action1)
	table.insert(round3.action_list, action2)
	table.insert(round3.action_list, action3)
	table.insert(round3.action_list, action4)
	table.insert(round3.action_list, action5)
	table.insert(round3.action_list, action6)
	table.insert(round3.action_list, action7)
	table.insert(round3.action_list, action8)
	table.insert(round3.action_list, action9)
	table.insert(round3.action_list, action10)
	
	------------------------------------------
	
	local round4 = {}
	round4.round = 4
	round4.action_list = {}

	local action1 = {}
	action1.action_id = 1
	action1.action_type = 2
	action1.pre_action = 0
	action1.team_id = 1
	action1.param1 = 2
	action1.param2 = 3
	action1.param3 = 0
	action1.param4 = "1,6"
	
	local action2 = {}
	action2.action_id = 2
	action2.action_type = 16
	action2.pre_action = 1	
	
	local action3 = {}
	action3.action_id = 3
	action3.action_type = 3
	action3.pre_action = 1
	action3.team_id = 2
	action3.param1 = 1
	action3.param2 = 1
	action3.param3 = 100
	action3.param4 = ""	

	local action4 = {}
	action4.action_id = 2
	action4.action_type = 16
	action4.pre_action = 1		

	local action5 = {}
	action5.action_id = 3
	action5.action_type = 3
	action5.pre_action = 1
	action5.team_id = 2
	action5.param1 = 6
	action5.param2 = 1
	action5.param3 = 100
	action5.param4 = ""		
	
	local action6 = {}
	action6.action_id = 2
	action6.action_type = 16
	action6.pre_action = 1	
	
	local action7 = {}
	action7.action_id = 4
	action7.pre_action = 1
	action7.team_id = 1
	action7.action_type = 15
	action7.param2 = 3

	table.insert(round4.action_list, action1)
	table.insert(round4.action_list, action2)
	table.insert(round4.action_list, action3)
	table.insert(round4.action_list, action4)
	table.insert(round4.action_list, action5)	
	table.insert(round4.action_list, action6)	
	table.insert(round4.action_list, action7)

	------------------------------------------

	table.insert(self.roundList, round1)
	table.insert(self.roundList, round2)
	table.insert(self.roundList, round3)
	table.insert(self.roundList, round4)
	
	-------------------------------------------
end

function BattleData:getCurrRound()
	return self.roundList[self.round]
end

return BattleData