using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;

namespace EntityStates.VagrantMonster
{
	// Token: 0x0200012D RID: 301
	internal class FireMegaNova : BaseState
	{
		// Token: 0x060005CB RID: 1483 RVA: 0x0001A794 File Offset: 0x00018994
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.duration = FireMegaNova.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture, Override", "FireMegaNova", "FireMegaNova.playbackRate", this.duration);
			this.Detonate();
		}

		// Token: 0x060005CC RID: 1484 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x0001A7E5 File Offset: 0x000189E5
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

		// Token: 0x060005CE RID: 1486 RVA: 0x0001A824 File Offset: 0x00018A24
		private void Detonate()
		{
			Vector3 position = base.transform.position;
			Util.PlaySound(FireMegaNova.novaSoundString, base.gameObject);
			if (FireMegaNova.novaEffectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireMegaNova.novaEffectPrefab, base.gameObject, "NovaCenter", false);
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
			if (base.isAuthority)
			{
				BullseyeSearch bullseyeSearch = new BullseyeSearch();
				bullseyeSearch.filterByLoS = true;
				bullseyeSearch.maxDistanceFilter = this.novaRadius;
				bullseyeSearch.searchOrigin = position;
				bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
				bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
				bullseyeSearch.teamMaskFilter.RemoveTeam(base.teamComponent.teamIndex);
				bullseyeSearch.RefreshCandidates();
				bullseyeSearch.queryTriggerInteraction = QueryTriggerInteraction.Collide;
				List<HurtBox> list = bullseyeSearch.GetResults().ToList<HurtBox>();
				if (list.Count > 0)
				{
					DamageInfo damageInfo = new DamageInfo();
					damageInfo.damage = this.damageStat * FireMegaNova.novaDamageCoefficient;
					damageInfo.attacker = base.gameObject;
					damageInfo.procCoefficient = 1f;
					damageInfo.crit = Util.CheckRoll(this.critStat, base.characterBody.master);
					for (int i = 0; i < list.Count; i++)
					{
						HurtBox hurtBox = list[i];
						HealthComponent healthComponent = hurtBox.healthComponent;
						if (healthComponent)
						{
							damageInfo.force = FireMegaNova.novaForce * (healthComponent.transform.position - position).normalized;
							damageInfo.position = hurtBox.transform.position;
							EffectManager.instance.SimpleImpactEffect(FireMegaNova.novaImpactEffectPrefab, hurtBox.transform.position, Vector3.up, true);
							healthComponent.TakeDamage(damageInfo);
						}
					}
				}
			}
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x0000BB2B File Offset: 0x00009D2B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}

		// Token: 0x040006A2 RID: 1698
		public static float baseDuration = 3f;

		// Token: 0x040006A3 RID: 1699
		public static GameObject novaEffectPrefab;

		// Token: 0x040006A4 RID: 1700
		public static GameObject novaImpactEffectPrefab;

		// Token: 0x040006A5 RID: 1701
		public static string novaSoundString;

		// Token: 0x040006A6 RID: 1702
		public static float novaDamageCoefficient;

		// Token: 0x040006A7 RID: 1703
		public static float novaForce;

		// Token: 0x040006A8 RID: 1704
		public float novaRadius;

		// Token: 0x040006A9 RID: 1705
		private float duration;

		// Token: 0x040006AA RID: 1706
		private float stopwatch;
	}
}
