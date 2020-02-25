using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008B3 RID: 2227
	public class CastSmokescreen : BaseState
	{
		// Token: 0x060031ED RID: 12781 RVA: 0x000D7694 File Offset: 0x000D5894
		private void CastSmoke()
		{
			if (!this.hasCastSmoke)
			{
				Util.PlaySound(CastSmokescreen.startCloakSoundString, base.gameObject);
			}
			else
			{
				Util.PlaySound(CastSmokescreen.stopCloakSoundString, base.gameObject);
			}
			EffectManager.SpawnEffect(CastSmokescreen.smokescreenEffectPrefab, new EffectData
			{
				origin = base.transform.position
			}, false);
			int layerIndex = this.animator.GetLayerIndex("Impact");
			if (layerIndex >= 0)
			{
				this.animator.SetLayerWeight(layerIndex, 2f);
				this.animator.PlayInFixedTime("LightImpact", layerIndex, 0f);
			}
			if (NetworkServer.active)
			{
				new BlastAttack
				{
					attacker = base.gameObject,
					inflictor = base.gameObject,
					teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
					baseDamage = this.damageStat * CastSmokescreen.damageCoefficient,
					baseForce = CastSmokescreen.forceMagnitude,
					position = base.transform.position,
					radius = CastSmokescreen.radius,
					falloffModel = BlastAttack.FalloffModel.None,
					damageType = DamageType.Stun1s
				}.Fire();
			}
		}

		// Token: 0x060031EE RID: 12782 RVA: 0x000D77B0 File Offset: 0x000D59B0
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = CastSmokescreen.baseDuration / this.attackSpeedStat;
			this.totalDuration = CastSmokescreen.stealthDuration + this.totalDuration;
			base.PlayCrossfade("Gesture, Smokescreen", "CastSmokescreen", "CastSmokescreen.playbackRate", this.duration, 0.2f);
			this.animator = base.GetModelAnimator();
			Util.PlaySound(CastSmokescreen.jumpSoundString, base.gameObject);
			EffectManager.SpawnEffect(CastSmokescreen.initialEffectPrefab, new EffectData
			{
				origin = base.transform.position
			}, true);
			if (base.characterBody && NetworkServer.active)
			{
				base.characterBody.AddBuff(BuffIndex.CloakSpeed);
			}
		}

		// Token: 0x060031EF RID: 12783 RVA: 0x000D7868 File Offset: 0x000D5A68
		public override void OnExit()
		{
			if (base.characterBody && NetworkServer.active)
			{
				if (base.characterBody.HasBuff(BuffIndex.Cloak))
				{
					base.characterBody.RemoveBuff(BuffIndex.Cloak);
				}
				if (base.characterBody.HasBuff(BuffIndex.CloakSpeed))
				{
					base.characterBody.RemoveBuff(BuffIndex.CloakSpeed);
				}
			}
			if (!this.outer.destroying)
			{
				this.CastSmoke();
			}
			base.OnExit();
		}

		// Token: 0x060031F0 RID: 12784 RVA: 0x000D78D8 File Offset: 0x000D5AD8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && !this.hasCastSmoke)
			{
				this.CastSmoke();
				if (base.characterBody && NetworkServer.active)
				{
					base.characterBody.AddBuff(BuffIndex.Cloak);
				}
				this.hasCastSmoke = true;
			}
			if (base.fixedAge >= this.totalDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060031F1 RID: 12785 RVA: 0x000D7950 File Offset: 0x000D5B50
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (!this.hasCastSmoke)
			{
				return InterruptPriority.PrioritySkill;
			}
			return InterruptPriority.Any;
		}

		// Token: 0x04003086 RID: 12422
		public static float baseDuration;

		// Token: 0x04003087 RID: 12423
		public static float stealthDuration = 3f;

		// Token: 0x04003088 RID: 12424
		public static string jumpSoundString;

		// Token: 0x04003089 RID: 12425
		public static string startCloakSoundString;

		// Token: 0x0400308A RID: 12426
		public static string stopCloakSoundString;

		// Token: 0x0400308B RID: 12427
		public static GameObject initialEffectPrefab;

		// Token: 0x0400308C RID: 12428
		public static GameObject smokescreenEffectPrefab;

		// Token: 0x0400308D RID: 12429
		public static float damageCoefficient = 1.3f;

		// Token: 0x0400308E RID: 12430
		public static float radius = 4f;

		// Token: 0x0400308F RID: 12431
		public static float forceMagnitude = 100f;

		// Token: 0x04003090 RID: 12432
		private float duration;

		// Token: 0x04003091 RID: 12433
		private float totalDuration;

		// Token: 0x04003092 RID: 12434
		private bool hasCastSmoke;

		// Token: 0x04003093 RID: 12435
		private Animator animator;
	}
}
