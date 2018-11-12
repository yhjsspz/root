local BattleController = class("BattleController", import("cframework.mvc.BaseController"))

function BattleController:startBattle(battleData)
	
	self:doNextRound()	
end

function BattleController:doEnd()
	
--[[	if self:isWin() then
		api:openSimpleWindow("BattleVictoryWindow",{"Item_Award"})
	else
		api:openSimpleWindow("BattleDefeatWindow",{"BattleCampauraicon_Item"})
	end--]]
	
	
end

function BattleController:doNextRound()
	
	api.BattleData.round = api.BattleData.round + 1	
	if api.BattleData.round <= #api.BattleData.roundList then
		local currRound = api.BattleData:getCurrRound()
		api.BattleData.actionList = currRound.action_list
			
		log("=======================actionList	"..api.BattleData.round.."======================")
		log("action_type","","team_id","param1","param2","param3","param4")
		local strAction = {"0.开始","1.普通攻击","2.施法","3.伤害","4.回血","5.格挡","6.反击","7.禁锢","8.新增buffer","9.bufferCD更新","10. 怒气变更","11.复活","12.闪避","13.","14.","15行动玩家结束","16等待","17结束"}
		for i=1,#api.BattleData.actionList do
			local action = api.BattleData.actionList[i]

			log(strAction[action.action_type + 1], "",action.team_id, action.param1, action.param2, action.param3, action.param4)
			
			if action.action_type == 15 then
				
				log("--------------------------------------------------------")
			end
			
		end
		log("=======================actionList End======================")
		
		self:doNextAction()
		api:sendNotification(Message.BATTLE, Message.BATTLE_TYPE_NEXT_ROUND)
	else
		
		api.BattleData.round = api.BattleData.round - 1	
		log("Round End")
		self:doEnd()
	end
	
end

function BattleController:doNextAction()	
	local action = self:popAction()
    if action == nil then
		self:doNextRound()
	else
		api.BattleData.currAction = action
		self:doAction(action)
	end
end

function BattleController:popAction()
    if #api.BattleData.actionList > 0 then
        local action = api.BattleData.actionList[1]
        table.remove(api.BattleData.actionList, 1)
        return action
    end

    return nil
end

function BattleController:doAction(currAction)
	log("doAction:",currAction.action_type)
	if currAction.action_type == Const.BattleAction_Type_Start then
		--战斗开始
		api:sendNotification(Message.BATTLE, Message.BATTLE_TYPE_START)
		self:doNextAction()
	elseif currAction.action_type == Const.BattleAction_Type_Attack then
		--普通攻击
		api:sendNotification(Message.BATTLE, Message.BATTLE_TYPE_ATTACK, currAction)
		self:doNextAction()
	elseif currAction.action_type == Const.BattleAction_Type_CastSkill then
		--施法
		api:sendNotification(Message.BATTLE, Message.BATTLE_TYPE_CASTSKILL, currAction)
		self:doNextAction()
	elseif currAction.action_type == Const.BattleAction_Type_Damage then
		--伤害
		api:sendNotification(Message.BATTLE, Message.BATTLE_TYPE_DAMAGE, currAction)
		self:doNextAction()
	elseif currAction.action_type == Const.BattleAction_Type_Recover then
		--回复
		api:sendNotification(Message.BATTLE, Message.BATTLE_TYPE_RECOVER, currAction)
		self:doNextAction()
	elseif currAction.action_type == Const.BattleAction_Type_Parrying then
		--格挡
		api:sendNotification(Message.BATTLE, Message.BATTLE_TYPE_PARRYING, currAction)
		self:doNextAction()
	elseif currAction.action_type == Const.BattleAction_Type_Miss then
		--闪避
		api:sendNotification(Message.BATTLE, Message.BATTLE_TYPE_MISS, currAction)
		self:doNextAction()
	elseif currAction.action_type == Const.BattleAction_Type_Counterattack then
		--反击
		api:sendNotification(Message.BATTLE, Message.BATTLE_TYPE_COUNTERATTACK, currAction)
		self:doNextAction()
	elseif currAction.action_type == Const.BattleAction_Type_Imprison then
		--禁锢
	elseif currAction.action_type == Const.BattleAction_Type_Buffer_Add then
		--buff新增
		api:sendNotification(Message.BATTLE, Message.BATTLE_TYPE_BUFF_ADD, currAction)
		self:doNextAction()
	elseif currAction.action_type == Const.BattleAction_Type_Buffer_CD then
		--buffCD
		api:sendNotification(Message.BATTLE, Message.BATTLE_TYPE_BUFF_CD, currAction)
		self:doNextAction()
	elseif currAction.action_type == Const.BattleAction_Type_Rage_Update then
		--怒气更新
		api:sendNotification(Message.BATTLE, Message.BATTLE_TYPE_RAGE_UPDATE, currAction)
		self:doNextAction()
	elseif currAction.action_type == Const.BattleAction_Type_Revive then
		--复活
		api:sendNotification(Message.BATTLE, Message.BATTLE_TYPE_REVIVE, currAction)
		self:doNextAction()
	elseif currAction.action_type == Const.BattleAction_Type_Hero_Action_Complete then
		--英雄行动回合结束
		api:sendNotification(Message.BATTLE, Message.BATTLE_TYPE_HERO_ACTION_COMPLETE, currAction)
	elseif currAction.action_type == Const.BattleAction_Type_Wait then
		--等待
	elseif currAction.action_type == Const.BattleAction_Type_End then
		--结束
		self:doNextAction()
	end
end

function BattleController:isLeft(teamId)
	--test
	if teamId == 1 then
		return true
	else
		return false
	end
end


function BattleController:isWin()
	
	if self:isLeft(1) == true then
		return api.BattleData.winner == 1
	else
		return api.BattleData.winner == 2
	end
end

function BattleController:getLeftTeam()	
	
	if self:isLeft(1) == true then
		return api.BattleData.team1
	else
		return api.BattleData.team2
	end
end

function BattleController:getRightTeam()	
	if self:isLeft(1) == true then
		return api.BattleData.team2
	else
		return api.BattleData.team1
	end
end

function BattleController:getCellCoordinate(pos, direction)
	local x, y
	if pos == 1 then
		x, y = 12,8
	elseif pos == 2 then
		x, y = 14,6
	elseif pos == 3 then
		x, y = 7,10
	elseif pos == 4 then
		x, y = 4,8
	elseif pos == 5 then
		x, y = 5,6
	elseif pos == 6 then
		x, y = 8,4
	end

	if direction == Const.Direction_Type_Right then
		x = Const.BattleGrid_Max_Col - x + 1		
	end
	
	return x, y
end

function BattleController:getCellPosition(pos, direction)
	local x, y = self:getCellCoordinate(pos, direction)
	return self:_getCellPosition(x, y, direction)
end

function BattleController:_getCellPosition(x, y, direction)
	if direction == Const.Direction_Type_Left then
		x = x - Const.BattleGrid_Max_Col / 2 - 1
		y = Const.BattleGrid_Max_Row / 2 - y
	else
		x = x - Const.BattleGrid_Max_Col / 2 - 1
		y = Const.BattleGrid_Max_Row / 2 - y
	end
	x = Const.BattleGrid_Cell_Width / 2 + x * Const.BattleGrid_Cell_Width
	y = Const.BattleGrid_Cell_Height / 2 + y * Const.BattleGrid_Cell_Height
	return x  / 100, y / 100
end

function BattleController:getCenterPos(direction)
	--test
	local x, y
	if direction == Const.Direction_Type_Left then
		x = 10
		y = 6
	else
		x = 31
		y = 6
	end
	return x, y
end

function BattleController:getDefeatJump()
	
	local cfg = {}
	
	for k,v in pairs(api.ItemData.JumpTypeConfig) do
		
		if v.type == 4 then
			table.insert(cfg, v)
		end
		
	end
	return cfg
	
end

function BattleController:getSkillActionList(skillId)
	log("getSkillActionList:",api.HeroData.HeroSkillConfig[skillId].ani_config)
	return clone(api.BattleData:getSkillConfig(api.HeroData.HeroSkillConfig[skillId].ani_config))
end

function BattleController:replay()

	api.BattleData.round = 0
	api.BattleData.roundList = clone(api.BattleData.roundListClone)
	
	self:doNextRound()	
	
end

function BattleController:getSkillActionList_test(skillId)
	--test
	local skillActionList = {}	
	if skillId == 0 then
		
		local skillAction1 = {}
		skillAction1.type_id = 1
		skillAction1.offset = {0, -3, 0}
		skillAction1.target_pos_type = 1		
		skillAction1.end_type = 0
		skillAction1.ease = 1
		
		local skillAction2 = {}
		skillAction2.type_id = 4
		skillAction2.ani_action_name = "attack"
		skillAction2.ani_action_type = 1
		skillAction2.ani_action_value = 1
		skillAction2.end_type = 1

		local skillAction3 = {}		
		skillAction3.type_id = 8
		skillAction3.time = 0.3
		
		local skillAction4 = {}
		skillAction4.type_id = 7
		skillAction4.offset = {0, 0, 0}
		skillAction4.target_pos_type = 1
		skillAction4.ani_path = "aisi_attack_hit_1"
		skillAction4.ani_action_type = 1
		skillAction4.ani_action_value = 1
		skillAction4.layer_type = 0
		skillAction4.end_type = 0
		skillAction4.is_listen_gethit = 1	
		
		table.insert(skillActionList, skillAction1)
		table.insert(skillActionList, skillAction2)
		table.insert(skillActionList, skillAction3)
		table.insert(skillActionList, skillAction4)
		
	elseif skillId == 1 then
		
		local skillAction1 = {}
		skillAction1.type_id = 4
		skillAction1.ani_action_name = "skill_1"
		skillAction1.ani_action_type = 1
		skillAction1.ani_action_value = 1
		skillAction1.end_type = 0

		local skillAction2 = {}
		skillAction2.type_id = 7
		skillAction2.offset = {0, 0, 0}
		skillAction2.target_pos_type = 1
		skillAction2.ani_path = "nvfashi_skill1_hit_1"
		skillAction2.ani_action_type = 1
		skillAction2.ani_action_value = 1
		skillAction2.layer_type = 0
		skillAction2.end_type = 1
		skillAction2.is_listen_gethit = 1
		
		local skillAction3 = {}
		skillAction3.type_id = 9
		skillAction3.time = 0.5
		skillAction3.strength = 0.5
		skillAction3.vibrato = 10
		skillAction3.randomness = 30
		skillAction3.end_type = 0
		
		table.insert(skillActionList, skillAction1)
		table.insert(skillActionList, skillAction2)
		table.insert(skillActionList, skillAction3)
		
	elseif skillId == 2 then
		local skillAction1 = {}
		skillAction1.type_id = 4
		skillAction1.ani_action_name = "attack"
		skillAction1.ani_action_type = 1
		skillAction1.ani_action_value = 1
		skillAction1.end_type = 0
		
		local skillAction2 = {}
		skillAction2.type_id = 5
		skillAction2.ani_path = "nvfashi_attack_hit_1"
		skillAction2.ani_play_time = 0.5
		skillAction2.start_pos_type = 0
		skillAction2.start_pos_offset = {0, 6, 3}
		skillAction2.end_pos_type = 1
		skillAction2.end_pos_offset = {0, 0, 1}
		skillAction2.end_type = 0
		skillAction2.layer_type = 0
		skillAction2.is_listen_gethit = 0
		skillAction2.ease = 5
		
		local skillAction3 = {}
		skillAction3.type_id = 7
		skillAction3.ani_path = "nvfashi_attack_hit_2"	
		skillAction3.target_pos_type = 1
		skillAction3.offset = {1, 0, 0}
		skillAction3.ani_action_type = 1
		skillAction3.ani_action_value = 1
		skillAction3.layer_type = 0
		skillAction3.is_listen_gethit = 1
		skillAction3.end_type = 1
		
		local skillAction4 = {}
		skillAction4.type_id = 9
		skillAction4.time = 0.5
		skillAction4.strength = 0.5
		skillAction4.vibrato = 10
		skillAction4.randomness = 30
		skillAction4.end_type = 0
		
		table.insert(skillActionList, skillAction1)
		table.insert(skillActionList, skillAction2)
		table.insert(skillActionList, skillAction3)
		table.insert(skillActionList, skillAction4)
	end
	return skillActionList
end

return BattleController