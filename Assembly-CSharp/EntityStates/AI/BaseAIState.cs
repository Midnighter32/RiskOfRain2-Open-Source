using System;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;

namespace EntityStates.AI
{
	// Token: 0x020008FE RID: 2302
	public class BaseAIState : EntityState
	{
		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x06003360 RID: 13152 RVA: 0x000DEDD9 File Offset: 0x000DCFD9
		// (set) Token: 0x06003361 RID: 13153 RVA: 0x000DEDE1 File Offset: 0x000DCFE1
		private protected CharacterMaster characterMaster { protected get; private set; }

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x06003362 RID: 13154 RVA: 0x000DEDEA File Offset: 0x000DCFEA
		// (set) Token: 0x06003363 RID: 13155 RVA: 0x000DEDF2 File Offset: 0x000DCFF2
		private protected BaseAI ai { protected get; private set; }

		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x06003364 RID: 13156 RVA: 0x000DEDFB File Offset: 0x000DCFFB
		// (set) Token: 0x06003365 RID: 13157 RVA: 0x000DEE03 File Offset: 0x000DD003
		private protected CharacterBody body { protected get; private set; }

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x06003366 RID: 13158 RVA: 0x000DEE0C File Offset: 0x000DD00C
		// (set) Token: 0x06003367 RID: 13159 RVA: 0x000DEE14 File Offset: 0x000DD014
		private protected Transform bodyTransform { protected get; private set; }

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x06003368 RID: 13160 RVA: 0x000DEE1D File Offset: 0x000DD01D
		// (set) Token: 0x06003369 RID: 13161 RVA: 0x000DEE25 File Offset: 0x000DD025
		private protected InputBankTest bodyInputBank { protected get; private set; }

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x0600336A RID: 13162 RVA: 0x000DEE2E File Offset: 0x000DD02E
		// (set) Token: 0x0600336B RID: 13163 RVA: 0x000DEE36 File Offset: 0x000DD036
		private protected CharacterMotor bodyCharacterMotor { protected get; private set; }

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x0600336C RID: 13164 RVA: 0x000DEE3F File Offset: 0x000DD03F
		// (set) Token: 0x0600336D RID: 13165 RVA: 0x000DEE47 File Offset: 0x000DD047
		private protected SkillLocator bodySkillLocator { protected get; private set; }

		// Token: 0x0600336E RID: 13166 RVA: 0x000DEE50 File Offset: 0x000DD050
		public override void OnEnter()
		{
			base.OnEnter();
			this.characterMaster = base.GetComponent<CharacterMaster>();
			this.ai = base.GetComponent<BaseAI>();
			if (this.ai)
			{
				this.body = this.ai.body;
				this.bodyTransform = this.ai.bodyTransform;
				this.bodyInputBank = this.ai.bodyInputBank;
				this.bodyCharacterMotor = this.ai.bodyCharacterMotor;
				this.bodySkillLocator = this.ai.bodySkillLocator;
			}
		}

		// Token: 0x0600336F RID: 13167 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06003370 RID: 13168 RVA: 0x000B23CF File Offset: 0x000B05CF
		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}
	}
}
