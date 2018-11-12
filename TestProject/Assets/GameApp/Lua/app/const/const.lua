local Const = {}

--合局
Const.Direction_Type_Left = 1
Const.Direction_Type_Right = 2

--Timer
Const.TIMER_ID_BATTLE = "TIMER_ID_BATTLE"
Const.Timer_Battle_Skill_Timeout = "Timer_Battle_Skill_Timeout"
Const.Timer_Battle_Skill_Wait_Timeout = "Timer_Battle_Skill_Wait_Timeout"

--战斗
Const.BattleAction_Type_Start = 0   --开始
Const.BattleAction_Type_Attack = 1 --普通攻击
Const.BattleAction_Type_CastSkill = 2   --施法
Const.BattleAction_Type_Damage = 3  --伤害
Const.BattleAction_Type_Recover = 4  --回血
Const.BattleAction_Type_Parrying = 5    --格挡
Const.BattleAction_Type_Counterattack = 6 --反击
Const.BattleAction_Type_Imprison = 7   --禁锢
Const.BattleAction_Type_Buffer_Add = 8   --新增bufer
Const.BattleAction_Type_Buffer_CD = 9  --更新bufer CD，0表示移除
Const.BattleAction_Type_Rage_Update = 10	--怒气更新
Const.BattleAction_Type_Revive = 11	--复活
Const.BattleAction_Type_Miss = 12    --闪避
Const.BattleAction_Type_Hero_Action_Complete = 15 --英雄行动回合结束 
Const.BattleAction_Type_Wait = 16 --等待
Const.BattleAction_Type_End = 17  --结束

Const.BattleSkillAction_Type_RunToPos = 1
Const.BattleSkillAction_Type_JumpToPos = 2
Const.BattleSkillAction_Type_FlashToPos = 3
Const.BattleSkillAction_Type_ChangeAction = 4
Const.BattleSkillAction_Type_PlayFlyEffect = 5
Const.BattleSkillAction_Type_PlayRayEffect = 6
Const.BattleSkillAction_Type_PlayEffect = 7
Const.BattleSkillAction_Type_Wait = 8
Const.BattleSkillAction_Type_Shake = 9

Const.BattleGrid_Max_Row = 10	--网络行数
Const.BattleGrid_Max_Col = 40	--网络列数
Const.BattleGrid_Cell_Width = 33	--单元格宽度
Const.BattleGrid_Cell_Height = 50	--单元格高度

Const.BattleHero_Action_Stand = "stand"
Const.BattleHero_Action_Run = "run"
Const.BattleHero_Action_Hit = "hit"
Const.BattleHero_Action_Flash = "flash"
Const.BattleHero_Action_Jump = "jump"
Const.BattleHero_Action_Dead = "dead"
Const.BattleHero_Move_Speed = 16

return Const