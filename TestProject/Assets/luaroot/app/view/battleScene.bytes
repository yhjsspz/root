local BattleScene = class("BattleScene", import("cframework.mvc.BaseWindow"))

function BattleScene:Start()

	self.sceneCamera = UnityEngine.GameObject.Find("SceneCamera")
	self.leftHeroList = {}
	self.rightHeroList = {}
	self.gridLayer = nil
	self.currSkillActionList = {}
	self.additionTxtList = {}
	self.additionTxtNum = 0
	self.subtractionTxtList = {}
	self.subtractionTxtNum = 0
	self.longPressId = 0
	self.longPressTime = 0
	self.longPressIsLeft = true

	self:initUI()
	self:initHero(Const.Direction_Type_Left)
	self:initHero(Const.Direction_Type_Right)
	
	api.BattleController:startBattle()
	
	api:openSimpleWindow("BattleWindow")
end

function BattleScene:Dispose()	
	api:removeTimer(Const.TIMER_ID_BATTLE)
end

function BattleScene:listNotificationInterests()
	return {Message.BATTLE}
end

function BattleScene:handleNotification(id, sid, data)
	if sid == Message.BATTLE_TYPE_START then	
		-- 开始	
	elseif sid == Message.BATTLE_TYPE_ATTACK then
		-- 普攻
		self:doSkillAction(data.param1, data)
	elseif sid == Message.BATTLE_TYPE_CASTSKILL then
		-- 技能
		self:doSkillAction(data.param1, data)
	elseif sid == Message.BATTLE_TYPE_DAMAGE then
		-- 伤害
		self:doDamageAction(data)
	elseif sid == Message.BATTLE_TYPE_PARRYING then
		-- 格挡
		self:doParryingAction(data)
	elseif sid == Message.BATTLE_TYPE_HERO_ACTION_COMPLETE then
		-- 出手结束
		self:doHeroActionComplete(data)
	elseif sid == Message.BATTLE_TYPE_NEXT_ROUND then
		-- 更新回合
		self:updateRound()
	elseif sid == Message.BATTLE_TYPE_RECOVER then
		-- 回血
		self:doRecoverAction(data)
	elseif sid == Message.BATTLE_TYPE_BUFF_ADD then
		-- 添加buff
		self:doBuffAddAction(data)
	elseif sid == Message.BATTLE_TYPE_BUFF_CD then
		-- 更新buff
		self:doBuffCdAction(data)
	elseif sid == Message.BATTLE_TYPE_RAGE_UPDATE then
		-- 能量更新
		self:doRageUpdateAction(data)
	elseif sid == Message.BATTLE_TYPE_COUNTERATTACK then
		-- 反击
		self:doCounterAttackAction(data)
	elseif sid == Message.BATTLE_TYPE_REVIVE then
		-- 复活
		self:doReviveAction(data)
	elseif sid == Message.BATTLE_TYPE_MISS then
		-- 闪避
		self:doMissAction(data)
		
	end
end

----------------------------------------------------------------------------------

function BattleScene:initUI()
	
	self.gridLayer = UnityEngine.GameObject.Find("MainLayer").transform
	api:addTimer(Const.TIMER_ID_BATTLE, 1, handler(self, self.onTimer))
	
end

function BattleScene:onTimer()
	
end

function BattleScene:updateRound()
	if self.Txt_round == nil then
		--第一次进来的时候还没初始化游戏
		return
	end
end


--- <summary>
--- 获取伤害数字
--- 1.普通伤害 2.加血 3.压制 4.暴击 5. 毒
--- </summary>
function BattleScene:getBattleNumText(index)
	
	if self.txtDic == nil then
		self.txtDic = {}
	end
	
	if self.txtDic[index] == nil then
		self.txtDic[index] = {}
	end
	

	if #self.txtDic[index] > 0 then
		local txt = self.txtDic[index][1]
		table.remove(self.txtDic[index], 1)
		txt:GetComponent("Text").color = Color(1,1,1,1)
		return txt
	end
	
	if self.txtNum == nil then
		self.txtNum = {}
	end
	if self.txtNum[index] == nil then
		self.txtNum[index] = 0
	end
	self.txtNum[index] = self.txtNum[index] + 1	
	
	local Txt_num = self:addChild(api:getCurrScene().TipLayer.transform, api:getAsset("prefabs/txt_battlenum_"..index, "Txt_Battlenum_"..index).transform,self.txtNum[index])

	return Txt_num
end

function BattleScene:initHero(direction)
	
	local team = nil
	if direction == Const.Direction_Type_Left then
		team = api.BattleController:getLeftTeam()
	else
		team = api.BattleController:getRightTeam()
	end
	
	dump(team.hero_list)
	
	for i = 1, #team.hero_list do
		local heroInfo = team.hero_list[i]
		
		local hero = require("app.view.entity.BattleHero").new()
		hero:init(heroInfo.id, heroInfo.pos, direction)
		
		self:addToCellPos(hero:getView(), heroInfo.pos, direction)
		self:setHeroSorting(hero)

		if direction == Const.Direction_Type_Left then
			self.leftHeroList[heroInfo.pos] = hero
		else
			self.rightHeroList[heroInfo.pos] = hero
		end
		
		--添加血条
		local preStr = "cvs_"..direction.."_"..i
		local heroCanvas = self:addChild(hero:getView().transform,api:getAsset("prefabs/battlehero_canvas", "BattleHero_Canvas").transform,preStr)
		
		hero:initInfo(heroCanvas.transform,self.uiRoot[preStr.."_Sli_blood"], self.uiRoot[preStr.."_Sli_yellow"], 
			self.uiRoot[preStr.."_Pnl_buff"], self.uiRoot[preStr.."_Img_camp"], self.uiRoot[preStr.."_Img_bg"], heroInfo.hp, heroInfo.total_hp, heroInfo.ap, heroInfo.total_ap)
				
	end
end

function BattleScene:addToCellPos(go, pos, direction, offsetX, offsetY)
	local x, y = api.BattleController:getCellCoordinate(pos, direction)
	self:addToCellPosXY(go, x, y, direction, offsetX, offsetY)
end

function BattleScene:addToCellPosXY(go, x, y, direction, offsetX, offsetY)
--	api:logE("addToCellPosXY", x, y, direction, offsetX, offsetY)
	x, y = api.BattleController:_getCellPosition(x, y, direction)
	if offsetX == nil then
		offsetX = 0
	end
	if offsetY == nil then
		offsetY = 0
	end	
	go.transform.localPosition = Vector3(x + offsetX / 100, y + offsetY / 100, 0)
	self:addChild(self.gridLayer, go.transform)
end

function BattleScene:setHeroSorting(hero, order)
	if order == nil then
		local x, y = api.BattleController:getCellCoordinate(hero.pos, hero.direction)
		order = y * 100 + x
	end
	
	api:setSortingLayer(hero:getView(), "Element", order)
end

function BattleScene:getTeamHeroList(teamId)
	if api.BattleController:isLeft(teamId) == true then
		return self.leftHeroList
	else
		return self.rightHeroList
	end
end

function BattleScene:getEnemyHeroList(teamId)
	if api.BattleController:isLeft(teamId) == true then
		return self.rightHeroList
	else
		return self.leftHeroList
	end
end

function BattleScene:getHero(teamId, pos)
	return self:getTeamHeroList(teamId)[pos]
end

----------------------------------------------------------------------------------------

function BattleScene:doDamageAction(data)	
	local hero = self:getHero(data.team_id, data.param1)
	
	hero:doColorHit()
	
	local Common_txt = nil
	
	if data.param2 == 0 then
		Common_txt = self:getBattleNumText(1)
	elseif data.param2 == 1 then
		Common_txt = self:getBattleNumText(4)
	elseif data.param2 == 2 then
		Common_txt = self:getBattleNumText(3)
	elseif data.param2 == 3 then
		Common_txt = self:getBattleNumText(5)
	end
	
	log("param2",data.param2)
	Common_txt:GetComponent("Text").text = "-"..data.param3
	Common_txt.position = Vector3(hero:getView().transform.position.x,
																	hero:getView().transform.position.y+2,0)
																	
	local position = Common_txt.transform.localPosition
	local sequence = DOTween.Sequence()
	
	if data.param2 == 1 then
		Common_txt.localScale = Vector3(1,1,1)
		Common_txt.localPosition = Vector3(position.x,position.y,0)
		
		sequence:Append(Common_txt.transform:DOScale(2,0.1))
		sequence:Append(Common_txt.transform:DOScale(1,0.1))
		sequence:Append(Common_txt.transform:DOScale(1.5,0.1))
		sequence:Append(Common_txt.transform:DOScale(1,0.1))
		sequence:Join(Common_txt:GetComponent("Text"):DOColor(Color(1,1,1,0), 0.5):SetEase(CFunc:ConvertToEaseType(18)))
	else
		
		Common_txt.localScale = Vector3(0.6,0.6,1)
		
		sequence:Append(Common_txt.transform:DOLocalJump(Vector3(position.x + 50,position.y,0), 50, 1,0.2))
		sequence:Append(Common_txt.transform:DOLocalJump(Vector3(position.x + 80,position.y,0), 10, 1,0.1))
		sequence:Append(Common_txt:GetComponent("Text"):DOColor(Color(1,0,0,0), 0.2))
		
	end
	
	
	sequence:AppendCallback(function ()
			
		table.insert(self.subtractionTxtList, Common_txt)
		
	end)

	hero:changeHp(-1*data.param3)
	
	if data.param4 ~= "1" then
		
		hero:changeState(Const.BattleHero_Action_Hit, false, function ()
			
			if hero:isDead() then
				hero:dead()
			else
				if hero.isFreeze == false then
					hero:changeState(Const.BattleHero_Action_Stand, true)
				end
			end 
			
			if data.param4 == "2" then
				
				log("*********************doNextAction","doDamageAction")
				api.BattleController:doNextAction()
				
			end
			
		end)
		
		
	end 
end
----------------------------------------------------------------------------------------
function BattleScene:doParryingAction(data)
	
	local hero = self:getHero(data.team_id, data.param1)
	local battleSkillEffect = self:_addHeroEffect(battleSkillEffect, hero, "fya", 1, {0,0,2}, 0, 0.4)
	battleSkillEffect:play(false, function ()
	end,function ()
		battleSkillEffect:dispose()
	end)
	
end
----------------------------------------------------------------------------------------
function BattleScene:doMissAction(data)
	
	local hero = self:getHero(data.team_id, data.param1)
	local battleSkillEffect = self:_addSkilEffect(battleSkillEffect, hero, "miss", 1, {0,0,2}, 0, 0.6,1)
	
	battleSkillEffect:getView():GetComponent("Renderer").sortingOrder = 10
	
	battleSkillEffect:play(false, function ()
	end,function ()
		battleSkillEffect:dispose()
	end)
	
end
----------------------------------------------------------------------------------------

function BattleScene:doRecoverAction(data)	
	local hero = self:getHero(data.team_id, data.param1)
	
	self:doRecoverAni(hero, data.param3, data.param2, function ()
		
	end)

	hero:changeHp(data.param3)
end

function BattleScene:doRecoverAni(hero, hp, recoverType, endCallback)
	
	local Common_txt = self:getBattleNumText(2)
	Common_txt:GetComponent("Text").text = "+"..hp
	Common_txt:GetComponent("RectTransform").anchoredPosition = Vector3(hero:getView().transform.position.x*100,
																		hero:getView().transform.position.y*100+200,0)
																		
	if recoverType == 1 then
		Common_txt:GetComponent("RectTransform").localScale = Vector3(1,1,1)
	else
		Common_txt:GetComponent("RectTransform").localScale = Vector3(0.6,0.6,1)
	end

	local position = Common_txt.transform.localPosition
	local sequence = DOTween.Sequence()
	sequence:Append(Common_txt.transform:DOScale(0.1,0.1))
	sequence:Append(Common_txt.transform:DOScale(1,0.1))
	sequence:Append(Common_txt.transform:DOScale(0.6,0.1))
	sequence:Append(Common_txt.transform:DOLocalMoveY(position.y + 70, 0.6))
	sequence:Join(Common_txt:GetComponent("Text"):DOColor(Color(0,1,0,0), 0.6):SetEase(CFunc:ConvertToEaseType(8)))
	
	sequence:AppendCallback(function ()
			
		log("doRecoverAni end")
		table.insert(self.additionTxtList, Common_txt)
		if endCallback ~= nil then
			endCallback()
		end
		
	end)

end
----------------------------------------------------------------------------------------

function BattleScene:doBuffAddAction(data)
	
	local hero = self:getHero(data.team_id, data.param1)
	local cfgBuff = api.HeroData.HeroSkillBuffConfig[data.param2]
	
	log("add buff", data.param2, cfgBuff.buffani_pos, data.param4)
	if cfgBuff.buffani_pos ~= 0 then
		hero:addBuff(data.param2,nil, data.param4)
	else
		local preStr = "Buff_"..data.param2
		local BattleHeroBuff_Item = self:addChild(hero.Pnl_buff, api:getAsset("prefabs/battleherobuff_item","BattleHeroBuff_Item"), preStr)
		
		hero:addBuff(data.param2, data.param3, BattleHeroBuff_Item, data.param4)
	end
	
end
----------------------------------------------------------------------------------------

function BattleScene:doBuffCdAction(data)
	
	local hero = self:getHero(data.team_id, data.param1)
	hero:updateBuff(data.param2, data.param3, data.param4)
	
end
----------------------------------------------------------------------------------------

function BattleScene:doRageUpdateAction(data)
	
	local hero = self:getHero(data.team_id, data.param1)
	hero:changeAp(data.param3)
end
----------------------------------------------------------------------------------------

function BattleScene:doReviveAction(data)
	
	local hero = self:getHero(data.team_id, data.param1)
	self:doRecoverAni(hero, data.param2, 0)
	hero:doReviveState(data.param2, data.param3)
	
end
----------------------------------------------------------------------------------------

function BattleScene:doCounterAttackAction(data)
	
		local hero = self:getHero(data.team_id, data.param2)
		local battleSkillEffect = self:_addSkilEffect(battleSkillEffect, hero, "nuqi", 1, {0,0,0}, 0, 0.6)
		
		battleSkillEffect:getView().transform.position = hero.infoView.position
		
		battleSkillEffect:play(false, function ()
		end,function ()
			self:doSkillAction(data.param1, data)
			battleSkillEffect:dispose()
		end)
		
end

----------------------------------------------------------------------------------------

function BattleScene:doHeroActionComplete(data)
	
	if self.currSkillSourceHero == nil or self.currSkillSourceHero:isInPos() == true then
		
		--放完技能回来后变为站立动作
		if self.currSkillSourceHero ~= nil and self.currSkillSourceHero.isFreeze == false then
			self.currSkillSourceHero:changeState(Const.BattleHero_Action_Stand, true)
		end
		log("*********************doNextAction","HeroActionComplete")
		api.BattleController:doNextAction()
		
		return
	end
	
	self.currSkillSourceHero:goToBack(function ()
		log("*********************doNextAction","goToBack end")
		api.BattleController:doNextAction()
	end, 18)
	
	self.currSkillId = 0
	self.currSkillSourceHero = nil
	self.currSkillTargetHeroList = nil
	self.currSkillActionList = nil
end
----------------------------------------------------------------------------------------

function BattleScene:doSkillAction(skillId, data)
	
	local sourceHero = self:getTeamHeroList(data.team_id)[data.param2]
	local targetArr = string.split(data.param4,',')
	local targetHeroList = {}
	for i = 1, #targetArr do
		local targetHero = nil
		if data.param3 == 0 then
			--对敌方施法
			targetHero = self:getEnemyHeroList(data.team_id)[tonumber(targetArr[i])]
		else
			--对已方施法
			targetHero = self:getTeamHeroList(data.team_id)[tonumber(targetArr[i])]
		end
		table.insert(targetHeroList, targetHero)
	end
	
	log("sourceHero:", data.team_id, data.param2,sourceHero)
	
	self.currSkillId = skillId
	self.currSkillSourceHero = sourceHero
	self.currSkillTarget = data.param3
	self.currSkillTargetHeroList = targetHeroList
	self.currSkillActionList = api.BattleController:getSkillActionList(skillId)
	
	--用来标识技能是否播放完成，现在只记录两个地方，一是人物动作完成，二是技能action完成，每次完成时该值加一
	self.endNum = 0	
	self.heroActNum = 0	
	
	
--	self:setHeroSorting(self.currSkillSourceHero, 10000)
	
	self:doNextSkillAction()
end

function BattleScene:doNextSkillAction()
	if #self.currSkillActionList == 0 then
		log("*********************doNextAction","skill end")
		self:setHeroSorting(self.currSkillSourceHero)
		
		self.endNum = self.endNum + 1
		if self.endNum == self.heroActNum + 1 then
			api.BattleController:doNextAction()
		end
		
		return
	end

	local skillAction = self.currSkillActionList[1]
	table.remove(self.currSkillActionList, 1)

	log("doSkillAction:"..skillAction.type_id)

	if skillAction.type_id == Const.BattleSkillAction_Type_RunToPos then
		self:doSkillAction_runToPos(self.currSkillSourceHero, skillAction)
	elseif skillAction.type_id == Const.BattleSkillAction_Type_JumpToPos then
		self:doSkillAction_jumpToPos(self.currSkillSourceHero, skillAction)
	elseif skillAction.type_id == Const.BattleSkillAction_Type_FlashToPos then
		self:doSkillAction_flashToPos(self.currSkillSourceHero, skillAction)
	elseif skillAction.type_id == Const.BattleSkillAction_Type_ChangeAction then
--		dump(skillAction)
		self:doSkillAction_changeAction(self.currSkillSourceHero, skillAction.ani_action_name, skillAction.ani_action_type, skillAction.ani_action_value, skillAction.end_type, skillAction.is_listen_gethit, skillAction.order_type)
	elseif skillAction.type_id == Const.BattleSkillAction_Type_PlayFlyEffect then
		self:doSkillAction_playFlyEffect(skillAction)
	elseif skillAction.type_id == Const.BattleSkillAction_Type_PlayRayEffect then
	elseif skillAction.type_id == Const.BattleSkillAction_Type_PlayEffect then
		self:doSkillAction_playEffect(skillAction)
	elseif skillAction.type_id == Const.BattleSkillAction_Type_Wait then		
		self:doSkillAction_wait(skillAction)
	elseif skillAction.type_id == Const.BattleSkillAction_Type_Shake then
		self:doSkillAction_shake(skillAction)
	end
end

function BattleScene:doSkillAction_changeAction(hero, actionName, ani_action_type, ani_action_value, end_type, is_listen_gethit, order_type)
	
	if order_type == 1 then
		self:setHeroSorting(hero, self.currSkillTargetHeroList[1]:getView():GetComponent("Renderer").sortingOrder+100)
	elseif order_type == 2 then
		self:setHeroSorting(hero, 10000)
	end
	
	if ani_action_type == nil or ani_action_type == 0 then
		hero:changeState(actionName, true)
	elseif ani_action_type == 1 then
		local times = 0
		self.heroActNum = self.heroActNum + 1
		local function __callback()
			times = times + 1
			if times == ani_action_value then
				
				self:doNextSkillAction() 
				if end_type == 0 then
					hero:clearCallback()
					
					self.endNum = self.endNum + 1
					if self.endNum == self.heroActNum + 1 then
						log("*********************doNextAction","end_type end")
						api.BattleController:doNextAction()
					end
					
				end
			else
				hero:changeState(actionName, false, __callback)
			end
		end
		local function __eventCallback(eventName)
			log("hero:changeState event", eventName)
			
			if is_listen_gethit == 1 and eventName == "gethit" then	
				log("*********************doNextAction","changeAction gethit")
				api.BattleController:doNextAction()
			end
				
			if end_type == 2 and eventName == "next_play" then
				self:doNextSkillAction()
			end
		end
		hero:changeState(actionName, false, __callback, __eventCallback)
	elseif ani_action_type == 2 then
		hero:changeState(actionName, true)
		api:setTimerout(Const.Timer_Battle_Skill_Timeout, ani_action_value, function ()
			if end_type == 0 then
				self:doNextSkillAction()
			end
		end)
	end
	if end_type == 1 then
		self:doNextSkillAction()
	end
end

function BattleScene:_getSkillTargetPos(skillAction)
	local targetHero = self.currSkillTargetHeroList[1]	
	local x, y = api.BattleController:getCellCoordinate(targetHero.pos, targetHero.direction)
	if skillAction.offset[1] == 0 then
		log("direction",targetHero.direction, x,y, skillAction.offset[2], skillAction.offset[3])
		if targetHero.direction == Const.Direction_Type_Left then
			x = x - skillAction.offset[2]
			y = y - skillAction.offset[3]
		else
			x = x + skillAction.offset[2]
			y = y + skillAction.offset[3]
		end
	end
--	log("TargetPos",x, y)
	return x, y
end

function BattleScene:doSkillAction_runToPos(hero, skillAction)		
	if skillAction.target_pos_type == 1 then
		local x, y = self:_getSkillTargetPos(skillAction)
		hero:runToPos(x, y, function ()
			self:doNextSkillAction()
		end, skillAction.ease, false)--skillAction.ease
	else		
		hero:goToBack(function ()
			self:doNextSkillAction()
		end, 18)
	end
end

function BattleScene:doSkillAction_flashToPos(hero, skillAction)
	if skillAction.target_pos_type == 1 then
		local x, y = self:_getSkillTargetPos(skillAction)
		hero:flashToPos(x, y, function ()
			self:doNextSkillAction()
		end)
	else
		hero:flashToBack(function ()
			self:doNextSkillAction()
		end)
	end	
end

function BattleScene:doSkillAction_jumpToPos(hero, skillAction)
	if skillAction.target_pos_type == 1 then
		local x, y = self:_getSkillTargetPos(skillAction)
		hero:jumpToPos(x, y, function ()
			self:doNextSkillAction()
		end, skillAction.ease, skillAction.gravity)
	else		
		hero:jumpToBack(function ()
			self:doNextSkillAction()
		end)
	end
end

function BattleScene:doSkillAction_playEffect(skillAction)
	local nextSkillActionFlag = false
	log("doSkillAction_playEffect", skillAction.ani_path, #self.currSkillTargetHeroList)
	for i = 1, #self.currSkillTargetHeroList do
		
		local targetHero = self.currSkillTargetHeroList[i]
		local battleSkillEffect = self:_addSkilEffect(battleSkillEffect, targetHero, skillAction.ani_path, skillAction.target_pos_type, skillAction.offset, skillAction.layer_type, skillAction.scale)

		if skillAction.rgba ~= nil and skillAction.rgba ~= "" then
			local rgba = string.split(skillAction.rgba, ",")
			local skeleton = battleSkillEffect:getView():GetComponent("SkeletonAnimation").skeleton
			
			skeleton.R = rgba[1];
			skeleton.G = rgba[2];
			skeleton.B = rgba[3];
			skeleton.A = rgba[4];
			
		end

		---------------------------------------------------------------------------------------------------------

		if skillAction.end_type == 1 and nextSkillActionFlag == false then
			nextSkillActionFlag = true
			self:doNextSkillAction()
		end
		
		if skillAction.ani_action_type == 0 then
			battleSkillEffect:play(true)			
		elseif skillAction.ani_action_type == 1 then
			local times = 0
			battleSkillEffect:play(true,
			function (eventName)
				log("event",eventName)
				if skillAction.is_listen_gethit == 1 and eventName == "gethit" then	
					log("*********************doNextAction","playEffect gethit", skillAction.ani_path)
					api.BattleController:doNextAction()
				end
				
				if skillAction.end_type == 2 and eventName == "next_play" then	
					nextSkillActionFlag = true			
					self:doNextSkillAction()
				end
				
			end,
			function ()
				times = times + 1
				if times == skillAction.ani_action_value then
					battleSkillEffect:dispose()
					if skillAction.end_type == 0 and nextSkillActionFlag == false then	
						nextSkillActionFlag = true			
						log("doSkillAction_playEffect  complete", skillAction.ani_path)
						self:doNextSkillAction()
					end					
				end
			end)
		elseif skillAction.ani_action_type == 2 then
			api:setTimerout(Const.Timer_Battle_Skill_Timeout, skillAction.endValue, function ()
				battleSkillEffect:dispose()
				if skillAction.end_type == 0 and nextSkillActionFlag == false then
					nextSkillActionFlag = true
					self:doNextSkillAction()
				end
			end)
		end
		
		-- 目标位置是中心点的即为全屏大招，只执行一次
		if skillAction.target_pos_type == 2 then
			break
		end
		
	end

end
function BattleScene:doSkillAction_playFlyEffect(skillAction)

	local nextSkillActionFlag = false	

	for i = 1, #self.currSkillTargetHeroList do

		local targetHero = self.currSkillTargetHeroList[i]
		local battleSkillEffect = self:_addSkilEffect(battleSkillEffect, self.currSkillSourceHero, skillAction.ani_path, skillAction.start_pos_type, skillAction.start_pos_offset, skillAction.layer_type, skillAction.scale)
		local effectView = battleSkillEffect:getView()
		
		------------------------------------------------------------------------------------------------

		local posX, posY, direction, offsetX, offsetY = self:_getSkillEffectPos(targetHero, skillAction.end_pos_type, skillAction.end_pos_offset)
		
		local x, y = api.BattleController:_getCellPosition(posX, posY, direction)
		local targetPos = Vector3(x + offsetX / 100, y + offsetY / 100, 0)
--		local angle = Vector3.Angle(effectView.transform.localPosition, targetPos)
		local angle = self:getFlyAngle(effectView.transform.localPosition.x,effectView.transform.localPosition.y,targetPos.x,targetPos.y)
		
		
		local function __callback()			
			battleSkillEffect:dispose()
			if nextSkillActionFlag == false then
				self:doNextSkillAction()
				nextSkillActionFlag = true
			end
			
		end

		effectView.transform.eulerAngles  = Vector3(0, 0, angle)
		effectView.transform:DOLocalMove(targetPos, skillAction.ani_play_time):SetEase(CFunc:ConvertToEaseType(skillAction.ease)):OnComplete(__callback)
		
	end
	
end

--计算平面两点叫
function BattleScene:getFlyAngle(x1,y1,x2,y2)
	local x = math.abs(x1-x2);
    local y = math.abs(y1-y2);
    local z=math.sqrt(x*x+y*y);
    local angle=math.round((math.asin(y/z)/math.pi*180));
	
	if y2 - y1 < 0 then
		angle = angle * -1
	end
	
	if x2 - x1 < 0 then
		angle = angle * -1
	end
	
	return angle
end

function BattleScene:_addSkilEffect(battleSkillEffect, targetHero, ani_path, pos_type, pos_offset, layer_type, scale, outDirection)
	
	if scale == nil then
		scale = 1
	end

	local posX, posY, direction, offsetX, offsetY = self:_getSkillEffectPos(targetHero, pos_type, pos_offset)
	
	local battleSkillEffect = require("app.view.entity.BattleSkillEffect").new()
	battleSkillEffect:init(tostring(posX).."_"..tostring(posY), ani_path, outDirection == nil and direction or outDirection)

	self:addToCellPosXY(battleSkillEffect:getView(), posX, posY, direction, offsetX, offsetY)

	if layer_type == 0 then
		api:setSortingLayer(battleSkillEffect:getView(), "FrontEffect", 0)
	else
		api:setSortingLayer(battleSkillEffect:getView(), "BackEffect", 0)
	end
	
	battleSkillEffect:getView().transform.localScale = Vector3(scale,scale,0)
	
	return battleSkillEffect
	
end

function BattleScene:_addHeroEffect(battleSkillEffect, targetHero, ani_path, pos_type, pos_offset, layer_type, scale, outDirection)

	if scale == nil then
		scale = 1
	end

	local posX, posY, direction, offsetX, offsetY = self:_getSkillEffectPos(targetHero, pos_type, pos_offset)
	
	local battleSkillEffect = require("app.view.entity.BattleSkillEffect").new()
	battleSkillEffect:init(tostring(posX).."_"..tostring(posY), ani_path, outDirection == nil and direction or outDirection)


	local aniView = self:addChild(targetHero:getView(), battleSkillEffect:getView())
	aniView.transform.position = Vector3(aniView.transform.position.x, aniView.transform.position.y+1, 0)

	if layer_type == 0 then
		api:setSortingLayer(battleSkillEffect:getView(), "FrontEffect", 0)
	else
		api:setSortingLayer(battleSkillEffect:getView(), "BackEffect", 0)
	end
	
	battleSkillEffect:getView().transform.localScale = Vector3(scale,scale,0)
	
	return battleSkillEffect
	
end

function BattleScene:_getSkillEffectPos(targetHero, pos_type, pos_offset)
	
	local posX, posY, direction

	if pos_type == 0 then
		--已方
		posX = self.currSkillSourceHero.posX
		posY = self.currSkillSourceHero.posY
		direction = self.currSkillSourceHero.direction
		
	elseif pos_type == 1 then
		--对方
		posX = targetHero.posX
		posY = targetHero.posY
		direction = targetHero.direction
	elseif pos_type == 2 then
		--中心点
		if self.currSkillTarget == 0 then
			--敌方
			posX, posY = api.BattleController:getCenterPos(targetHero.direction)
			direction = targetHero.direction
		else
			--已方
			posX, posY = api.BattleController:getCenterPos(self.currSkillSourceHero.direction)	
			direction = self.currSkillSourceHero.direction
		end
	else
		logError("doSkillAction_playEffect Error:target_pos_type无效")
		return
	end	

	local offsetX, offsetY
	if pos_offset[1] == 0 then
		offsetX = pos_offset[2] * Const.BattleGrid_Cell_Width
		offsetY = pos_offset[3] * Const.BattleGrid_Cell_Height
	else
		offsetX = pos_offset[2]
		offsetY = pos_offset[3]
	end
	
	if direction == Const.Direction_Type_Right then
		offsetX = offsetX * -1
	end
	
	return posX, posY, direction, offsetX, offsetY
end

function BattleScene:doSkillAction_wait(skillAction)	
	api:setTimerout(Const.Timer_Battle_Skill_Wait_Timeout, skillAction.time, function ()
		self:doNextSkillAction()
	end)
end

function BattleScene:doSkillAction_shake(skillAction)
	if skillAction.end_type == 1 then
		self:doNextSkillAction()
	end
	self.sceneCamera:Shake(skillAction.time, skillAction.strength, skillAction.vibrato, skillAction.randomness, false, true, function ()
		if skillAction.end_type == 0 then
			self:doNextSkillAction()
		end
	end)
end

function BattleScene:doSkillAction_timeScale(skillAction)
	--Time.timeScale = 0.2
end

function BattleScene:doSkillAction_cameraZoom()	
end

function BattleScene:doSkillAction_cameraLockAt()	
end

--------------------------------------------------------------------------------------------

return BattleScene