
local BattleSkillEffect = class("BattleSkillEffect")

function BattleSkillEffect:init(id, path, direction)

	self.id = id
	self.direction = direction
	
	---------------------------
			
	self.skillAni = api:newSpineSync("BattleScene", nil, "spineani/battleskill/"..path..".unity3d", path)
	self.skillAni.gameObject.name = "skill_"..path.."_"..tostring(id)
	
	if direction == Const.Direction_Type_Right then
		self.skillAni.skeleton.FlipX = true
	end

	CFunc:SetSpineEvent(self.skillAni.state, 0, handler(self, self.onPlayEnd))	
	CFunc:SetSpineEvent(self.skillAni.state, 1, handler(self, self.onEvent))
	
	------------------------------
end

function BattleSkillEffect:dispose()
	self.eventCallback = nil
	self.endCallback = nil
	if self.skillAni ~= nil then
		UnityEngine.GameObject.Destroy(self.skillAni.gameObject)
	end	
	self.skillAni = nil
end

function BattleSkillEffect:play(isLoop, eventCallback, endCallback)
	self.eventCallback = eventCallback
	self.endCallback = endCallback
	self.skillAni.state:SetAnimation(0, "hit", isLoop)	
end

function BattleSkillEffect:onEvent(eventName)
	if self.eventCallback ~= nil then
		self.eventCallback(eventName)
	end
end

function BattleSkillEffect:onPlayEnd()
	if self.endCallback ~= nil then
		self.endCallback()
	end
end

function BattleSkillEffect:changeFlipX(flipX)
	self.skillAni.skeleton.FlipX = flipX
end

function BattleSkillEffect:getView()
	return self.skillAni.gameObject
end

return BattleSkillEffect