using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Treebot.Pounder
{
	// Token: 0x0200075A RID: 1882
	public class Spawn : BaseState
	{
		// Token: 0x06002B89 RID: 11145 RVA: 0x000B7DFC File Offset: 0x000B5FFC
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = Spawn.baseDuration / this.attackSpeedStat;
			Util.PlaySound(Spawn.spawnSoundString, base.gameObject);
			EffectManager.SimpleMuzzleFlash(Spawn.spawnPrefab, base.gameObject, "Feet", false);
			base.PlayAnimation("Base", "Spawn", "Spawn.playbackRate", this.duration);
		}

		// Token: 0x06002B8A RID: 11146 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002B8B RID: 11147 RVA: 0x000B7E63 File Offset: 0x000B6063
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002B8C RID: 11148 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400279F RID: 10143
		public static GameObject spawnPrefab;

		// Token: 0x040027A0 RID: 10144
		public static float baseDuration;

		// Token: 0x040027A1 RID: 10145
		public static string spawnSoundString;

		// Token: 0x040027A2 RID: 10146
		private float duration;
	}
}
