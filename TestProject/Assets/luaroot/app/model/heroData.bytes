local HeroData = class("HeroData", import("cframework.mvc.BaseData"))

----------------
--- 英雄Data ---
----------------

---------------- 配置 ----------------

HeroData.HeroAttributeTypeConfig = nil					-- 英雄属性类型配置
HeroData.HeroCampAuraConfig = nil						-- 英雄阵营光环配置

HeroData.HeroCampAuraLvConfig = nil						-- 阵营光环等级配置
HeroData.HeroCampConfig = nil							-- 英雄阵营配置

HeroData.HeroConfig = nil								-- 英雄配置
HeroData.HeroLvUpConfig = nil							-- 英雄升级配置
HeroData.HeroQualityConfig = nil						-- 英雄品质配置

HeroData.HeroSkillBuffConfig = nil						-- 技能buff配置
HeroData.HeroSkillConfig = nil							-- 技能配置
HeroData.HeroSkillLvupConfig = nil						-- 技能升级配置表

HeroData.HeroTierupConfig = nil							-- 英雄品阶配置
HeroData.HeroLvLimitConfig = nil						-- 英雄等级限制

HeroData.HeroProfessionConfig = nil						-- 英雄职业配置

---------------- 变量 ----------------

-- 仓库相关

HeroData.hero_list = nil								-- 英雄仓库列表
HeroData.curr_cell_count = nil							-- 英雄仓库当前格子数量
HeroData.cell_price = nil								-- 英雄仓库当前购买格子的价格	
HeroData.cell_buy_num = nil								-- 单次购买格子的数量

HeroData.curr_depotData = nil							-- 当前的背包过滤好的列表数据
HeroData.curr_trainHero = nil							-- 当前培养界面打开的英雄对象

-- 图鉴相关

HeroData.hide_list = nil								-- 图鉴屏蔽的英雄Id列表
HeroData.owned_list = nil								-- 已经拥有的英雄Id列表

HeroData.curr_manualData = nil							-- 当前的图鉴数据列表
HeroData.curr_manualHero = nil							-- 当前图鉴详细页面的英雄对象

-- 升级相关

HeroData.currLevelUpSN = nil							-- 当前升级的英雄SN

-- 升阶相关

HeroData.currTierUpSN = nil								-- 当前升阶的英雄sn

HeroData.currTierUpPreview = nil						-- 当前升阶预览
HeroData.currSelectHeroIndex = nil                      -- 当前选择消耗的英雄格子编号

--- <summary>
--- data初始化
--- </summary>
function HeroData:onInit()

	-- 加载配置

	self.HeroAttributeTypeConfig = api:loadConfig("HeroAttributeTypeConfig")
	self.HeroCampAuraConfig = api:loadConfig("HeroCampAuraConfig")
	self.HeroCampAuraLvConfig = api:loadConfig("HeroCampAuraLvConfig")
	self.HeroCampConfig = api:loadConfig("HeroCampConfig")
	self.HeroConfig = api:loadConfig("HeroConfig")
	self.HeroLvUpConfig = api:loadConfig("HeroLvUpConfig")
	self.HeroQualityConfig = api:loadConfig("HeroQualityConfig")
	self.HeroSkillBuffConfig = api:loadConfig("HeroSkillBuffConfig")
	self.HeroSkillConfig = api:loadConfig("HeroSkillConfig")
	self.HeroSkillLvupConfig = api:loadConfig("HeroSkillLvupConfig")						-- 英雄技能升级配置表
	self.HeroTierupConfig = api:loadConfig("HeroTierupConfig")
	self.HeroLvLimitConfig = api:loadConfig("HeroLvLimitConfig")						-- 英雄等级限制
	self.HeroProfessionConfig = api:loadConfig("HeroProfessionConfig")						-- 英雄职业配置

	-- 初始化变量
	
	-- 仓库相关

	self.hero_list = {}								-- 英雄仓库列表
	self.curr_cell_count = 0							-- 英雄仓库当前格子数量
	self.cell_price = 0								-- 英雄仓库当前购买格子的价格
	self.cell_buy_num = 0								-- 单次购买格子的数量	

	-- 图鉴相关

	self.hide_list = {}								-- 图鉴屏蔽的英雄Id列表
	self.owned_list = {}								-- 已经拥有的英雄Id列表

	-- 升级相关

	self.currLevelUpSN = 0							-- 当前升级的英雄SN

	-- 升阶相关

	self.currTierUpSN = 0								-- 当前升阶的英雄sn

	---- 测试代码
--[[
	local ids = {11001, 22001, 31001, 43001, 55001, 62001}
	self.owned_list = {11001, 22001, 31001, 43001, 55001, 62001}

	for i = 1, 40 do

		local heroInfo = {}
		heroInfo.sn = i
		heroInfo.id = ids[i % 6 + 1]
		heroInfo.power = api:getRandomNumber(100, 1000000)
		heroInfo.lv = api:getRandomNumber(1, 100)
		heroInfo.tier = api:getRandomNumber(1, 8)
        heroInfo.lock = 0
        heroInfo.status = 0
        heroInfo.hp = api:getRandomNumber(100, 100000)
        heroInfo.attack = api:getRandomNumber(100, 10000)
        heroInfo.armor = api:getRandomNumber(100, 10000)
        heroInfo.speed = api:getRandomNumber(100, 10000)
        heroInfo.power = api:getRandomNumber(100, 100000)
        heroInfo.hit = api:getRandomNumber(100, 10000)
        heroInfo.dodge = api:getRandomNumber(100, 10000)
        heroInfo.crit = api:getRandomNumber(100, 10000)
        heroInfo.critdamage = api:getRandomNumber(100, 10000)
        heroInfo.armorbreak = api:getRandomNumber(100, 10000)
        heroInfo.controlimmune = api:getRandomNumber(100, 10000)
        heroInfo.reducedamage = api:getRandomNumber(100, 10000)
        heroInfo.holydamage = api:getRandomNumber(100, 10000)
        heroInfo.sp = api:getRandomNumber(0, 100)

		table.insert(self.hero_list, heroInfo)

	end

    self.currTierUpPreview = {}

    self.currTierUpPreview.lv_max = 200
    self.currTierUpPreview.hp = 200000
    self.currTierUpPreview.attack = 20000
    self.currTierUpPreview.armor = 10000
    self.currTierUpPreview.speed = 2000
    self.currTierUpPreview.power = 500000
    self.currTierUpPreview.skill_id = 111001
    self.currTierUpPreview.cur_skill_lv = 1
    self.currTierUpPreview.next_skill_lv = 2

    local comsume_list = {
        {hero_id=11001, camp=1, num=2, quality=5, select_list={}},
        {hero_id=0, camp=1, num=2, quality=6, select_list={}},
        {hero_id=0, camp=1, num=3, quality=6, select_list={}}
    }

    self.currTierUpPreview.comsume_hero = comsume_list
--]]

	---- 测试代码

end

--- <summary>
--- 获取属性类型配置
--- </summary>
function HeroData:getHeroAttributeTypeConfig(id)

    return self.HeroAttributeTypeConfig[id]

end

--- <summary>
--- 根据阵营配置
--- </summary>
function HeroData:getHeroCampConfig(id)

    return self.HeroCampConfig[id]

end

--- <summary>
--- 根据品质获取品质配置
--- </summary>
function HeroData:getHeroQualityConfig(quality)

	return self.HeroQualityConfig[quality]

end

--- <summary>
--- 获取英雄职业配置
--- </summary>
function HeroData:getProfessionConfig(profession)

	return self.HeroProfessionConfig[profession]

end

--- <summary>
--- 根据配置Id获取英雄配置
--- </summary>
function HeroData:getHeroConfigById(heroId)

	for key, info in pairs(self.HeroConfig) do

		if info.id == heroId then

			return info

		end

	end

	return nil

end

--- <summary>
--- 根据阵营获取英雄配置列表
--- </summary>
function HeroData:getHeroConfigByCamp(camp)

	local list = {}

	for key, info in pairs(self.HeroConfig) do

		if info.camp == camp then

			table.insert(list, info)

		end

	end

	self:doHeroConfigSort(list)

	return list

end

--- <summary>
--- 根据配置Id获取技能配置
--- </summary>
function HeroData:getHeroSkillConfig(skill_id)

	for key, info in pairs(self.HeroSkillConfig) do

		if info.id == skill_id then

			return info

		end

	end

	return nil

end

--- <summary>
--- 根据主键Id获取英雄数据
--- </summary>
function HeroData:getMyHeroInfoBySN(sn)

	for key, info in ipairs(self.hero_list) do

		if info.sn == sn then
			return info
		end

	end

	return nil

end

--- <summary>
--- 根据阵营获取英雄列表
--- </summary>
function HeroData:getMyHeroListByCamp(camp)

	if camp == nil or camp == 0 then

		-- 不传参数、0 都表示全阵营，返回所有列表

		local list = api.HeroController:copyList(self.hero_list)
		self:doMyHeroSortByLevel(list)
		return list

	end

	local list = {}

	for key, info in ipairs(self.hero_list) do

		local baseInfo = self:getHeroConfigById(info.id)

		if baseInfo ~= nil and baseInfo.camp == camp then

			table.insert(list, info)

		end

	end

	--self:doMyHeroSortByLevel(list)
    self:doMyHeroSortByQuality(list)

	return list

end

--- <summary>
--- 英雄配置列表排序
--- </summary>
function HeroData:doHeroConfigSort(list)

	local function __sort(a, b)

		if a.quality == b.quality then

			-- 同品阶的，判断战力
			-- 同战力的，判断Id
			return a.id < b.id

		else

			-- 品阶越低，排序越靠前
			return a.quality < b.quality

		end

	end

	table.sort(list, __sort)

end

--- <summary>
--- 英雄数据品阶排序
--- </summary>
function HeroData:doMyHeroSortByLevel(list)

	local function __sort(a, b)

		if a.lv == b.lv then

			if a.power == b.power then

				-- 战力也相同的英雄，按照id排序
				return a.id < b.id 

			else

				-- 战力越高排序越靠前
				return a.power > b.power

			end

		else

			-- 等级越高，排序越靠前
			return a.lv > b.lv

		end

	end

	table.sort(list, __sort)

end

--- <summary>
--- 英雄数据品质排序
--- </summary>
function HeroData:doMyHeroSortByQuality(list)

	local function __sort(a, b)

        local a_base = self:getHeroConfigById(a.id)
        local b_base = self:getHeroConfigById(b.id)

        print("英雄数据品质排序", a_base.quality, b_base.quality)

        if a_base.quality == b_base.quality then

            -- 同品质比等级

			if a.lv == b.lv then

                -- 同等级比战力

                if a.power == b.power then

                    -- 战力也相同的英雄，按照id排序
                    return a.id < b.id 

                else

                    -- 战力越高排序越靠前
                    return a.power > b.power

                end

            else

                -- 等级越高，排序越靠前
                return a.lv > b.lv

            end

		else

			-- 品质越高，排序越靠前
			return a_base.quality > b_base.quality

		end

	end

	table.sort(list, __sort)

end

--- <summary>
--- 新增英雄
--- </summary>
function HeroData:addHero(info)

	table.insert(self.hero_list, info)

	--self:doMyHeroSortByLevel(self.hero_list)

end

--- <summary>
--- 删除英雄
--- </summary>
function HeroData:removeHero(sn)

	for index, info in ipairs(self.hero_list) do

		if info.sn == sn then

			table.remove(self.hero_list, index)
			break

		end

	end

end

--- <summary>
--- 获取技能等级
--- skill_id 技能Id
--- hero_tier 英雄品阶
--- </summary>
function HeroData:getSkillLevelConfig(skill_id, hero_tier)

	local config = nil

	for key, info in ipairs(self.HeroSkillLvupConfig) do

		if config == nil then

			config = info

		elseif tonumber(info.skillid) == skill_id and tonumber(info.unlock_tier) <= hero_tier and tonumber(info.level) > tonumber(config.level) then

			config = info

		end

	end

	return config

end

--- <summary>
--- 获取技能升级配置
--- skill_id 技能Id
--- hero_tier 英雄品阶
--- </summary>
function HeroData:getSkillLvupConfig(skill_id, hero_tier)

	for key, info in ipairs(self.HeroSkillLvupConfig) do

		if info.skillid == skill_id and info.unlock_tier == hero_tier then

			return info

		end

	end

	return nil

end

--- <summary>
--- 获取英雄升级配置
--- lv 英雄等级
--- </summary>
function HeroData:getHeroLevelupConfig(lv)

	return self.HeroLvUpConfig[lv]

end

--- <summary>
--- 获取英雄升阶配置
--- tier 英雄品阶
--- </summary>
function HeroData:getHeroTierupConfig(tier)

	for index, config in pairs(self.HeroTierupConfig) do

		if config.tier == tier then return config end

	end

	return nil

end

--- <summary>
--- 根据英雄品质获取英雄
--- quality 英雄初始品质
--- camp 英雄阵营，为0 或者 为nil 视为全阵营
--- </summary>
function HeroData:getMyHeroByQuality(quality, camp)

	local heroList = self:getMyHeroListByCamp(camp)

	local list = {}

	for index, info in ipairs(heroList) do

		local baseInfo = self:getHeroConfigById(info.id)

		if baseInfo.quality == quality then

			table.insert(list, info)

		end

	end

	return list

end

--- <summary>
--- 获取英雄每阶的等级限制
--- quality 英雄初始品质
--- tier 英雄的品阶
--- </summary>
function HeroData:getHeroLvLimitConfig(quality, tier)

	for index, config in ipairs(self.HeroLvLimitConfig) do

		if config.quality == quality and config.tier == tier then 

			return config

		end

	end

	return config

end

--- <summary>
--- 获取英雄升阶消耗可选列表
--- camp 英雄阵营
--- quality 英雄初始品质
--- tier 英雄的品阶
--- hero_id 选择同名卡时，需要传递此参数，其他条件不需要
--- </summary>
function HeroData:getHeroTierConsumeList(camp, quality, hero_id)

    local temp = {}

    if hero_id ~= nil and hero_id ~= 0 then

        -- 同名卡

        for index, info in ipairs(self.hero_list) do

            if info.id == hero_id then

                table.insert(temp, info)

            end

        end

    else

        -- 非同名卡

        temp = self:getMyHeroByQuality(quality, camp)

    end

    local list = {}

    -- 过滤已经被选中的

    for key, heroInfo in ipairs(temp) do

        if api.HeroController:isInTierChoose(heroInfo.sn, self.currSelectHeroIndex) == false
            and heroInfo.sn ~= self.curr_trainHero.sn then

            table.insert(list, heroInfo)

        end

    end

    return list

end

return HeroData