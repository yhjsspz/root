local BattleHero = class("BattleHero")

function BattleHero:init(id, pos, direction)
	
	self.id = id
	self.pos = pos
	self.posX, self.posY = api.BattleController:getCellCoordinate(pos, direction)
	self.currPosX = nil
	self.currPosY = nil
	self.direction = direction
	self.lastState = nil
	self.buffAniList = {}
	self.isFreeze = false	--是否被定住
	
	---------------------------
	
	self.heroAni = nil
	
	local ani_name = api.HeroData.HeroConfig[id].default_ani
	local battle_aniscale = api.HeroData.HeroConfig[id].battle_aniscale
	
	self.heroAni = api:newSpineSync("BattleScene", nil, "spineani/battleani/"..ani_name..".unity3d", ani_name, "stand", true)

	self.heroAni.gameObject.name = "hero_"..tostring(direction).."_"..tostring(pos)
	self.heroAni.gameObject.transform.localScale = Vector3(battle_aniscale,battle_aniscale,battle_aniscale);
	
	if direction == Const.Direction_Type_Right then
		self.heroAni.skeleton.FlipX = true
	end
			
	CFunc:SetSpineEvent(self.heroAni.state, 0, handler(self, self.onChangeStateEnd))
	CFunc:SetSpineEvent(self.heroAni.state, 1, handler(self, self.onEvent))
	
end

function BattleHero:dispose()
	self.changeStateCallback = nil
	if self.heroAni ~= nil then
		GameObject.Destroy(self.heroAni.gameObject)
	end	
	self.heroAni = nil
end

function BattleHero:clearCallback()
	log("-------------------clearCallback")
	self.changeStateCallback = nil
	self.eventCallback = nil
	self.actionName = nil
end

function BattleHero:changeState(actionName, isLoop, endCallback, eventCallback)	
	log("changeState",self:getView().name, self.id,actionName)
	self.changeStateCallback = endCallback
	self.eventCallback = eventCallback
	self.actionName = actionName
	
	self.heroAni.skeleton:SetToSetupPose();
	self.heroAni.state:ClearTracks();
	self.heroAni.state:SetAnimation(0,actionName, isLoop)
end

function BattleHero:onChangeStateEnd(trackEntry)
	if self.changeStateCallback ~= nil and self.actionName == tostring(trackEntry) then
		log("onChangeStateEnd", self.id, self.actionName , tostring(trackEntry))
		self.changeStateCallback()
	end
end

function BattleHero:onEvent(eventName)
	log("BattleHero:onEvent:", self.id, eventName, (self.eventCallback == nil))
	if self.eventCallback ~= nil then
		self.eventCallback(eventName)
	end
end

function BattleHero:runToPos(tileX, tileY, arrivedCallback, ease, isArriveStand)	
	log("runToPos",tileX, tileY)
	self:changeState(Const.BattleHero_Action_Run, true)
	self.lastState = Const.BattleHero_Action_Run
	
	local heroView = self:getView()
	local x, y = api.BattleController:_getCellPosition(tileX, tileY, self.direction)
	local targetPos = Vector3(x, y, 0)
	local distance = Vector3.Distance(heroView.transform.localPosition, targetPos)	
	local duration = distance / Const.BattleHero_Move_Speed
	
	local function __callback()
		log("runToPos end")
		if isArriveStand ~= false then
			self:changeState(Const.BattleHero_Action_Stand, true)
		end
		self.currPosX = tileX
		self.currPosY = tileY
		if arrivedCallback ~= nil then
			arrivedCallback()
		end	
	end

	if ease == nil then
		ease = 1
	end
	log("runToPos start", self.id, duration)
	
	heroView.transform:DOLocalMove(targetPos, duration):SetEase(CFunc:ConvertToEaseType(ease)):OnComplete(__callback)
end

function BattleHero:runToBack(arrivedCallback, ease)	
	log("runToBack ease:",ease)
	self:runToPos(self.posX, self.posY, arrivedCallback, ease)
end

function BattleHero:flashToPos(tileX, tileY, arrivedCallback)
	self.lastState = Const.BattleHero_Action_Flash
	
	local heroView = self:getView()
	local x, y = api.BattleController:_getCellPosition(tileX, tileY, self.direction)
	local targetPos = Vector3(x, y, 0)

	heroView.transform.localPosition = targetPos
	
	self:changeState(Const.BattleHero_Action_Flash, false, arrivedCallback)	
end

function BattleHero:flashToBack(arrivedCallback)
	self:flashToPos(self.posX, self.posY, arrivedCallback)
end

function BattleHero:jumpToPos(tileX, tileY, arrivedCallback, ease, gravity)
	if gravity == nil then
		gravity = 0.8
	end
	
	self:changeState(Const.BattleHero_Action_Jump, false)
	self.lastState = Const.BattleHero_Action_Jump
	
	local heroView = self:getView()
	local x, y = api.BattleController:_getCellPosition(tileX, tileY, self.direction)
	local targetPos = Vector3(x, y, 0)
	local distance = Vector3.Distance(heroView.transform.localPosition, targetPos)	
	local duration = distance / 8
	
	local function __callback()
		self:changeState(Const.BattleHero_Action_Stand, true)
		self.currPosX = tileX
		self.currPosY = tileY
		if arrivedCallback ~= nil then
			arrivedCallback()
		end	
	end

	heroView.transform:DOLocalJump(targetPos, gravity, 1, duration):SetEase(CFunc:ConvertToEaseType(ease)):OnComplete(__callback)
end

function BattleHero:jumpToBack(arrivedCallback, ease)
	self:jumpToPos(self.posX, self.posY, arrivedCallback, ease)
end

function BattleHero:goToBack(arrivedCallback, ease)

	if self.lastState == Const.BattleHero_Action_Run then
		self:runToBack(arrivedCallback, ease)
	elseif self.lastState == Const.BattleHero_Action_Flash then
		self:flashToBack(arrivedCallback)
	elseif self.lastState == Const.BattleHero_Action_Jump then
		self:jumpToBack(arrivedCallback, ease)
	end

	self.lastState = nil
end

function BattleHero:isInPos()
	if self.currPosX == nil and self.currPosY == nil then
		log("isInPos true")
		return true
	end
	log("isInPos", self.posX , self.currPosX , self.posY , self.currPosY)
	
	if self.posX ~= self.currPosX or self.posY ~= self.currPosY then
		return false
	end
	return true
end

function BattleHero:changeFlipX(flipX)
	self.heroAni.skeleton.FlipX = flipX
end

function BattleHero:initInfo(infoView,Sli_blood,Sli_yellow,Pnl_buff,Img_camp, Img_bg, hp, totalHp, ap, totalAp)
	
	self.infoView = infoView
	self.Sli_blood = Sli_blood:GetComponent("Slider")
	self.Sli_yellow = Sli_yellow:GetComponent("Slider")
	self.hp = hp
	self.ap = ap
	self.totalHp = totalHp
	self.totalAp = totalAp
	self.Pnl_buff = Pnl_buff
	self.Img_camp = Img_camp
	self.Img_bg = Img_bg
	
	self.infoView.localScale = Vector3(0.02,0.02,0.02)
	self.infoView.localPosition = Vector3(0, 3.5, 1)
	
	log("ap", ap, totalAp)
	self.Sli_blood.value = hp/totalHp
	self.Sli_yellow.value = ap/totalAp
	
	--加载种族图标
	local cfgHero = api.HeroData.HeroConfig[self.id]
	local imgCampName = api.HeroData.HeroCampConfig[cfgHero.camp].icon
	
	log("imgCampName", imgCampName)
	
	api:setImage("BattleScene", Img_camp, "ui/common/herocamp", imgCampName)
end

function BattleHero:changeHp(changeHp, endCallback)
	
	self.hp = self.hp + changeHp
	
	if self.hp < 0 then
		self.hp = 0
	end
	
	local percent = self.hp / self.totalHp
	if percent < 0 then
		percent = 0
	end
	self.Sli_blood:DOValue(percent, 0.5, false):OnComplete(function ()
		if endCallback ~= nil then
			endCallback()
		end
	end)
	
end

function BattleHero:isDead()
	return self.hp <= 0
end

function BattleHero:dead(endCallback)
	self:changeState(Const.BattleHero_Action_Dead, false,endCallback)
	
	local sequence = DOTween.Sequence()
	
	sequence:Join(self.Img_camp:GetComponent("Image"):DOColor(Color(0,0,0,0), 0.2))
	sequence:Join(self.Img_bg:GetComponent("Image"):DOColor(Color(0,0,0,0), 0.2))
	sequence:PrependCallback(function ()
		self.Sli_yellow.value = 0
		--移除buff
		for k,v in pairs(self.buffAniList) do 
			
			log("dead remove buff",k,v, self.buffAniList[k])
			if tostring(v) ~= "null" then
				UnityEngine.GameObject.Destroy(v.gameObject)
			end
		end
		
		
	end)
end

function BattleHero:changeAp(changeAp, endCallback)
	
	self.ap = changeAp
	log("changeAp", self.id, self.ap, self.totalAp, self.ap / self.totalAp)
	if changeAp == 0 then
		self.Sli_yellow:DOKill()
		self.Sli_yellow.value = 0
	else
		
		self.Sli_yellow:DOValue(self.ap / self.totalAp, 0.3, false):OnComplete(function ()
			
			if endCallback ~= nil then
				endCallback()
			end
		end)
	
	end
	
	
end

function BattleHero:addBuff(bufferId, num, itemView, buffSn)
	
	local cfgBuff = api.HeroData.HeroSkillBuffConfig[bufferId]
	
	if cfgBuff.buffani_pos == 1 or cfgBuff.buffani_pos == 2 then
		--挂血条上的动画buff
		local buffAni = api:newSpineSync("BattleScene", self.heroAni, "spineani/battlebuff/"..cfgBuff.buffani..".unity3d", cfgBuff.buffani.."_SkeletonData", "hit", true)
		buffAni.name = "spn_buff"..bufferId
		
		local pos = cfgBuff.buffani_pos == 1 and self.infoView.localPosition or Vector3(0,0,0)
		
		buffAni.transform.localPosition = Vector3(pos.x + cfgBuff.buffani_offset_x, pos.y + cfgBuff.buffani_offset_y,0)
		buffAni.transform.localScale = Vector3(cfgBuff.buffani_scale,cfgBuff.buffani_scale,1)
		
		api:setSortingLayer(buffAni, "FrontEffect", 0)
		
		self.buffAniList[bufferId] = buffAni
		
		if cfgBuff.isfreeze == 1 then
			self.isFreeze = true
			self.heroAni.state:GetCurrent(0).Loop = false
--[[			self.heroAni.state:GetCurrent(0).AnimationStart = 0;
			self.heroAni.state:GetCurrent(0).AnimationEnd = 0;--]]
		end
		
		self.heroAni.state:ClearTrack(1)
		api:logE(self.heroAni)
	else
		--头顶buff
		local preStr = "Buff_"..bufferId.."_"
		local Img_buff = itemView.transform:Find(preStr.."Img_buff")
		local Txt_buff = itemView.transform:Find(preStr.."Txt_num")
		
		api:setImageByAsset(Img_buff, "dongtai/bufficon/"..cfgBuff.bufficon, cfgBuff.bufficon, true)
		api:setText(Txt_buff, num == 1 and "" or num)
		
		self.buffAniList[bufferId] = itemView
		
	end
	
end

function BattleHero:updateBuff(bufferId, buffCd, buffSn)
	
	if buffCd == 0 then
		log( "remove buff", "bufferId", self.buffAniList[bufferId] == nil)
		
		local cfgBuff = api.HeroData.HeroSkillBuffConfig[bufferId]
		if cfgBuff.isfreeze == 1 then
			self.isFreeze = false
			self:changeState(Const.BattleHero_Action_Stand, true)
		end
		UnityEngine.GameObject.Destroy(self.buffAniList[bufferId].gameObject)
	else
		
		local cfgBuff = api.HeroData.HeroSkillBuffConfig[bufferId]
		
		if cfgBuff.buffani_pos == 0 then
		
			local preStr = "Buff_"..bufferId.."_"
			local Txt_buff = self.buffAniList[bufferId].transform:Find(preStr.."Txt_num")
			
			api:setText(Txt_buff, buffCd == 1 and "" or buffCd)	
			
		end
	end
	
end

function BattleHero:doReviveState(hp, ap)
	
	log("复活",hp, ap)
	self:changeHp(hp)
	self:changeAp(ap)
	self:changeState(Const.BattleHero_Action_Stand, true)
	
	local sequence = DOTween.Sequence()
	
	sequence:Join(self.Img_camp:GetComponent("Image"):DOColor(Color(1,1,1,1), 0.2))
	sequence:Join(self.Img_bg:GetComponent("Image"):DOColor(Color(1,1,1,1), 0.2))

end

function BattleHero:doColorHit()
	
--[[	local setter = function(x)
		log(x)
	end

	DOTween.To(setter, 1, 5, 1)--]]
	
end

function BattleHero:getView()
	return self.heroAni.gameObject
end

return BattleHero