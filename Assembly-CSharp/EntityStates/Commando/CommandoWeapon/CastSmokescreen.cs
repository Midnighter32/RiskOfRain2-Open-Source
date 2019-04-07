using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020001A2 RID: 418
	public class CastSmokescreen : BaseState
	{
		// Token: 0x0600081B RID: 2075 RVA: 0x00028728 File Offset: 0x00026928
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
			EffectManager.instance.SpawnEffect(CastSmokescreen.smokescreenEffectPrefab, new EffectData
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

		// Token: 0x0600081C RID: 2076 RVA: 0x00028848 File Offset: 0x00026A48
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = CastSmokescreen.baseDuration / this.attackSpeedStat;
			this.totalDuration = CastSmokescreen.stealthDuration + this.totalDuration;
			base.PlayCrossfade("Gesture, Smokescreen", "CastSmokescreen", "CastSmokescreen.playbackRate", this.duration, 0.2f);
			this.animator = base.GetModelAnimator();
			Util.PlaySound(CastSmokescreen.jumpSoundString, base.gameObject);
			EffectManager.instance.SpawnEffect(CastSmokescreen.initialEffectPrefab, new EffectData
			{
				origin = base.transform.position
			}, true);
			if (base.characterBody && NetworkServer.active)
			{
				base.characterBody.AddBuff(BuffIndex.CloakSpeed);
			}
		}

		// Token: 0x0600081D RID: 2077 RVA: 0x00028904 File Offset: 0x00026B04
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

		// Token: 0x0600081E RID: 2078 RVA: 0x00028974 File Offset: 0x00026B74
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

		// Token: 0x0600081F RID: 2079 RVA: 0x000289EC File Offset: 0x00026BEC
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (!this.hasCastSmoke)
			{
				return InterruptPriority.PrioritySkill;
			}
			return InterruptPriority.Any;
		}

		// Token: 0x04000A9E RID: 2718
		public static float baseDuration;

		// Token: 0x04000A9F RID: 2719
		public static float stealthDuration = 3f;

		// Token: 0x04000AA0 RID: 2720
		public static string jumpSoundString;

		// Token: 0x04000AA1 RID: 2721
		public static string startCloakSoundString;

		// Token: 0x04000AA2 RID: 2722
		public static string stopCloakSoundString;

		// Token: 0x04000AA3 RID: 2723
		public static GameObject initialEffectPrefab;

		// Token: 0x04000AA4 RID: 2724
		public static GameObject smokescreenEffectPrefab;

		// Token: 0x04000AA5 RID: 2725
		public static float damageCoefficient = 1.3f;

		// Token: 0x04000AA6 RID: 2726
		public static float radius = 4f;

		// Token: 0x04000AA7 RID: 2727
		public static float forceMagnitude = 100f;

		// Token: 0x04000AA8 RID: 2728
		private float duration;

		// Token: 0x04000AA9 RID: 2729
		private float totalDuration;

		// Token: 0x04000AAA RID: 2730
		private bool hasCastSmoke;

		// Token: 0x04000AAB RID: 2731
		private Animator animator;
	}
}
