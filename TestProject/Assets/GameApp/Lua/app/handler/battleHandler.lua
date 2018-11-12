local BattleHandler = class("BattleHandler", import("cframework.mvc.BaseHandler"))

function BattleHandler:HANDLER_10401(data)	
	
	dump(data)
	
	api.BattleData.battle_type = data.battle_type
	api.BattleData.battle_setting = data.battle_setting
	api.BattleData.battle_scene = data.battle_scene
	api.BattleData.roundList = data.round
	api.BattleData.winner = data.winner
	api.BattleData.team1 = data.team1
	api.BattleData.team2 = data.team2
	api.BattleData.show_type = data.show_type
	api.BattleData.text = data.text
	api.BattleData.item_list = data.item_list
	
	
	local preloadList = {}
	
	------------------------------------添加其他预加载-------------------------------------------
	preloadList["prefabs/txt_battlenum_1"] = 0
	preloadList["prefabs/txt_battlenum_2"] = 0
	preloadList["prefabs/txt_battlenum_3"] = 0
	preloadList["prefabs/txt_battlenum_4"] = 0
	preloadList["prefabs/txt_battlenum_5"] = 0
	preloadList["prefabs/common_img"] = 0
	preloadList["prefabs/common_btn"] = 0
	preloadList["prefabs/battleui"] = 0
	preloadList["prefabs/battlehero_canvas"] = 0
	preloadList["prefabs/battleherobuff_item"] = 0
	preloadList["ui/common/herocamp"] = 0
	
	------------------------------------背景-------------------------------------------
	local bgName = api.BattleData.BattleBgConfig[data.battle_setting].image
	local sceneName = api.BattleData.BattleGroundConfig[data.battle_scene].image
	preloadList["dongtai/battlebg/"..bgName] = 0
	preloadList["dongtai/battlebg2/"..sceneName] = 0
	
	------------------------------------添加阵营光辉预加载---------------------------------------
	for i=1,#data.team1.camp_halo do

		local campId = data.team1.camp_halo[i]
		preloadList["dongtai/battlecampauraicon/"..api.BattleData:getCampHaloImg(campId)] = 0

	end
	for i=1,#data.team2.camp_halo do

		local campId = data.team2.camp_halo[i]
		preloadList["dongtai/battlecampauraicon/"..api.BattleData:getCampHaloImg(campId)] = 0
		
	end
	------------------------------------添加人物预加载-------------------------------------------
	for	i = 1,#data.team1.hero_list do
		
		local heroId = data.team1.hero_list[i].id
		local heroAniName = api.HeroData.HeroConfig[heroId].default_ani
		
		preloadList["spineani/battleani/"..heroAniName] = 0;
		
	end
	
	for	i = 1,#data.team2.hero_list do
		
		local heroId = data.team2.hero_list[i].id
		local heroAniName = api.HeroData.HeroConfig[heroId].default_ani
		
		preloadList["spineani/battleani/"..heroAniName] = 0;
		
	end
	
	------------------------------------添加技能预加载-------------------------------------------
	for	i = 1,#data.round do
		
		local action_list = data.round[i].action_list
		
		for	j = 1,#action_list do
			
			local action = action_list[j]
			--技能
			if action.action_type == 2 or action_type == 1 then
				
				if action.param1 == 0 then 

					log("error",i,j,action.action_type, action.param1)
					break 
					
				end
				
				local skillAni = api.HeroData.HeroSkillConfig[action.param1].ani_config
				
				local skillList = api.BattleData:getSkillConfig(skillAni)
				
				for k = 1,#skillList do
					
					local skillItem = skillList[k]
					
					if skillItem.type_id == 5 or skillItem.type_id == 7 then
						
						preloadList["spineani/battleskill/"..skillItem.ani_path] = 0;
						
					end
					
				end
				
			--buff
			elseif action.action_type == 8 then
				
				log(action.param2)
				local cfgBuff = api.HeroData.HeroSkillBuffConfig[action.param2]
				if cfgBuff.buffani_pos ~= 0 then
					--动画buff
					preloadList["spineani/battlebuff/"..cfgBuff.buffani] = 0;
				else
					--头顶buff
					preloadList["dongtai/bufficon/"..cfgBuff.bufficon] = 0;
				end
			-- 格挡技能
			elseif action.action_type == 5 then	
				preloadList["spineani/battleskill/fya"] = 0;
			end
		end
		
	end	
	
	dump(table.keys(preloadList))
	
	api.BattleData.round = 0
	api:enterScene("Battle", table.keys(preloadList), "battleScene")
	
	
end


return BattleHandler
