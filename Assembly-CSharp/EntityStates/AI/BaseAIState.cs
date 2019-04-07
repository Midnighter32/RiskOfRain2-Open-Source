using System;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;

namespace EntityStates.AI
{
	// Token: 0x020001E3 RID: 483
	public class BaseAIState : EntityState
	{
		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000960 RID: 2400 RVA: 0x0002F1B9 File Offset: 0x0002D3B9
		// (set) Token: 0x06000961 RID: 2401 RVA: 0x0002F1C1 File Offset: 0x0002D3C1
		private protected CharacterMaster characterMaster { protected get; private set; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000962 RID: 2402 RVA: 0x0002F1CA File Offset: 0x0002D3CA
		// (set) Token: 0x06000963 RID: 2403 RVA: 0x0002F1D2 File Offset: 0x0002D3D2
		private protected BaseAI ai { protected get; private set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000964 RID: 2404 RVA: 0x0002F1DB File Offset: 0x0002D3DB
		// (set) Token: 0x06000965 RID: 2405 RVA: 0x0002F1E3 File Offset: 0x0002D3E3
		private protected CharacterBody body { protected get; private set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000966 RID: 2406 RVA: 0x0002F1EC File Offset: 0x0002D3EC
		// (set) Token: 0x06000967 RID: 2407 RVA: 0x0002F1F4 File Offset: 0x0002D3F4
		private protected Transform bodyTransform { protected get; private set; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000968 RID: 2408 RVA: 0x0002F1FD File Offset: 0x0002D3FD
		// (set) Token: 0x06000969 RID: 2409 RVA: 0x0002F205 File Offset: 0x0002D405
		private protected InputBankTest bodyInputBank { protected get; private set; }

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x0600096A RID: 2410 RVA: 0x0002F20E File Offset: 0x0002D40E
		// (set) Token: 0x0600096B RID: 2411 RVA: 0x0002F216 File Offset: 0x0002D416
		private protected CharacterMotor bodyCharacterMotor { protected get; private set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x0600096C RID: 2412 RVA: 0x0002F21F File Offset: 0x0002D41F
		// (set) Token: 0x0600096D RID: 2413 RVA: 0x0002F227 File Offset: 0x0002D427
		private protected SkillLocator bodySkillLocator { protected get; private set; }

		// Token: 0x0600096E RID: 2414 RVA: 0x0002F230 File Offset: 0x0002D430
		public override void OnEnter()
		{
			base.OnEnter();
			this.characterMaster = base.GetComponent<CharacterMaster>();
			this.ai = base.GetComponent<BaseAI>();
			if (this.characterMaster)
			{
				GameObject bodyObject = this.characterMaster.GetBodyObject();
				if (bodyObject)
				{
					this.body = bodyObject.GetComponent<CharacterBody>();
					if (this.body)
					{
						this.bodyTransform = this.body.transform;
						this.bodyInputBank = this.body.GetComponent<InputBankTest>();
						this.bodyCharacterMotor = this.body.GetComponent<CharacterMotor>();
						this.bodySkillLocator = this.body.GetComponent<SkillLocator>();
					}
				}
			}
		}

		// Token: 0x0600096F RID: 2415 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000970 RID: 2416 RVA: 0x0000F633 File Offset: 0x0000D833
		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}
	}
}
