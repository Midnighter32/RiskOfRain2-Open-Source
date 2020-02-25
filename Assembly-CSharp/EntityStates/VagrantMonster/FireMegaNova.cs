using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.VagrantMonster
{
	// Token: 0x02000803 RID: 2051
	public class FireMegaNova : BaseState
	{
		// Token: 0x06002EA2 RID: 11938 RVA: 0x000C64D0 File Offset: 0x000C46D0
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.duration = FireMegaNova.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture, Override", "FireMegaNova", "FireMegaNova.playbackRate", this.duration);
			this.Detonate();
		}

		// Token: 0x06002EA3 RID: 11939 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002EA4 RID: 11940 RVA: 0x000C6521 File Offset: 0x000C4721
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002EA5 RID: 11941 RVA: 0x000C6560 File Offset: 0x000C4760
		private void Detonate()
		{
			Vector3 position = base.transform.position;
			Util.PlaySound(FireMegaNova.novaSoundString, base.gameObject);
			if (FireMegaNova.novaEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireMegaNova.novaEffectPrefab, base.gameObject, "NovaCenter", false);
			}
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
				temporaryOverlay.duration = 3f;
				temporaryOverlay.animateShaderAlpha = true;
				temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
				temporaryOverlay.destroyComponentOnEnd = true;
				temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matVagrantEnergized");
				temporaryOverlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
			}
			if (NetworkServer.active)
			{
				new BlastAttack
				{
					attacker = base.gameObject,
					baseDamage = this.damageStat * FireMegaNova.novaDamageCoefficient,
					baseForce = FireMegaNova.novaForce,
					bonusForce = Vector3.zero,
					canHurtAttacker = false,
					crit = base.characterBody.RollCrit(),
					damageColorIndex = DamageColorIndex.Default,
					damageType = DamageType.Generic,
					falloffModel = BlastAttack.FalloffModel.None,
					inflictor = base.gameObject,
					position = position,
					procChainMask = default(ProcChainMask),
					procCoefficient = 3f,
					radius = this.novaRadius,
					losType = BlastAttack.LoSType.NearestHit,
					teamIndex = base.teamComponent.teamIndex,
					impactEffect = EffectCatalog.FindEffectIndexFromPrefab(FireMegaNova.novaImpactEffectPrefab)
				}.Fire();
			}
		}

		// Token: 0x06002EA6 RID: 11942 RVA: 0x0000C5D3 File Offset: 0x0000A7D3
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}

		// Token: 0x04002BC6 RID: 11206
		public static float baseDuration = 3f;

		// Token: 0x04002BC7 RID: 11207
		public static GameObject novaEffectPrefab;

		// Token: 0x04002BC8 RID: 11208
		public static GameObject novaImpactEffectPrefab;

		// Token: 0x04002BC9 RID: 11209
		public static string novaSoundString;

		// Token: 0x04002BCA RID: 11210
		public static float novaDamageCoefficient;

		// Token: 0x04002BCB RID: 11211
		public static float novaForce;

		// Token: 0x04002BCC RID: 11212
		public float novaRadius;

		// Token: 0x04002BCD RID: 11213
		private float duration;

		// Token: 0x04002BCE RID: 11214
		private float stopwatch;
	}
}
