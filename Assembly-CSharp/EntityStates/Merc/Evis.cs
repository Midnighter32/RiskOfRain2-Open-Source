using System;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Merc
{
	// Token: 0x020007C2 RID: 1986
	public class Evis : BaseState
	{
		// Token: 0x06002D5E RID: 11614 RVA: 0x000BF9CC File Offset: 0x000BDBCC
		public override void OnEnter()
		{
			base.OnEnter();
			this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
			Util.PlayScaledSound(Evis.beginSoundString, base.gameObject, 1.2f);
			this.crit = Util.CheckRoll(this.critStat, base.characterBody.master);
			this.modelTransform = base.GetModelTransform();
			if (this.modelTransform)
			{
				this.animator = this.modelTransform.GetComponent<Animator>();
				this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
			}
			if (this.characterModel)
			{
				this.characterModel.invisibilityCount++;
			}
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
			}
			if (NetworkServer.active)
			{
				base.characterBody.AddBuff(BuffIndex.HiddenInvincibility);
			}
		}

		// Token: 0x06002D5F RID: 11615 RVA: 0x000BFAAC File Offset: 0x000BDCAC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			this.attackStopwatch += Time.fixedDeltaTime;
			float num = 1f / Evis.damageFrequency / this.attackSpeedStat;
			if (this.attackStopwatch >= num)
			{
				this.attackStopwatch -= num;
				HurtBox hurtBox = this.SearchForTarget();
				if (hurtBox)
				{
					Util.PlayScaledSound(Evis.slashSoundString, base.gameObject, Evis.slashPitch);
					Util.PlaySound(Evis.dashSoundString, base.gameObject);
					Util.PlaySound(Evis.impactSoundString, base.gameObject);
					HurtBoxGroup hurtBoxGroup = hurtBox.hurtBoxGroup;
					HurtBox hurtBox2 = hurtBoxGroup.hurtBoxes[UnityEngine.Random.Range(0, hurtBoxGroup.hurtBoxes.Length - 1)];
					if (hurtBox2)
					{
						DamageInfo damageInfo = new DamageInfo();
						damageInfo.damage = Evis.damageCoefficient * this.damageStat;
						damageInfo.attacker = base.gameObject;
						damageInfo.procCoefficient = Evis.procCoefficient;
						damageInfo.position = hurtBox2.transform.position;
						damageInfo.crit = this.crit;
						hurtBox2.healthComponent.TakeDamage(damageInfo);
						GlobalEventManager.instance.OnHitEnemy(damageInfo, hurtBox2.healthComponent.gameObject);
						GlobalEventManager.instance.OnHitAll(damageInfo, hurtBox2.healthComponent.gameObject);
						Vector3 position = hurtBox2.transform.position;
						Vector2 normalized = UnityEngine.Random.insideUnitCircle.normalized;
						Vector3 normal = new Vector3(normalized.x, 0f, normalized.y);
						EffectManager.SimpleImpactEffect(Evis.hitEffectPrefab, position, normal, false);
						Transform transform = hurtBox.hurtBoxGroup.transform;
						TemporaryOverlay temporaryOverlay = transform.gameObject.AddComponent<TemporaryOverlay>();
						temporaryOverlay.duration = num;
						temporaryOverlay.animateShaderAlpha = true;
						temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
						temporaryOverlay.destroyComponentOnEnd = true;
						temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matMercEvisTarget");
						temporaryOverlay.AddToCharacerModel(transform.GetComponent<CharacterModel>());
					}
				}
				else if (base.isAuthority && this.stopwatch > Evis.minimumDuration)
				{
					this.outer.SetNextStateToMain();
				}
			}
			if (base.characterMotor)
			{
				base.characterMotor.velocity = Vector3.zero;
			}
			if (this.stopwatch >= Evis.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002D60 RID: 11616 RVA: 0x000BFD20 File Offset: 0x000BDF20
		private HurtBox SearchForTarget()
		{
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = base.transform.position;
			bullseyeSearch.searchDirection = UnityEngine.Random.onUnitSphere;
			bullseyeSearch.maxDistanceFilter = Evis.maxRadius;
			bullseyeSearch.teamMaskFilter = TeamMask.all;
			bullseyeSearch.teamMaskFilter.RemoveTeam(TeamComponent.GetObjectTeam(base.gameObject));
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.RefreshCandidates();
			return bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
		}

		// Token: 0x06002D61 RID: 11617 RVA: 0x000BFD94 File Offset: 0x000BDF94
		private void CreateBlinkEffect(Vector3 origin)
		{
			EffectData effectData = new EffectData();
			effectData.rotation = Util.QuaternionSafeLookRotation(Vector3.up);
			effectData.origin = origin;
			EffectManager.SpawnEffect(Evis.blinkPrefab, effectData, false);
		}

		// Token: 0x06002D62 RID: 11618 RVA: 0x000BFDCC File Offset: 0x000BDFCC
		public override void OnExit()
		{
			Util.PlaySound(Evis.endSoundString, base.gameObject);
			this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
			this.modelTransform = base.GetModelTransform();
			if (this.modelTransform)
			{
				TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
				temporaryOverlay.duration = 0.6f;
				temporaryOverlay.animateShaderAlpha = true;
				temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
				temporaryOverlay.destroyComponentOnEnd = true;
				temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matMercEvisTarget");
				temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
				TemporaryOverlay temporaryOverlay2 = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
				temporaryOverlay2.duration = 0.7f;
				temporaryOverlay2.animateShaderAlpha = true;
				temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
				temporaryOverlay2.destroyComponentOnEnd = true;
				temporaryOverlay2.originalMaterial = Resources.Load<Material>("Materials/matHuntressFlashExpanded");
				temporaryOverlay2.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
			}
			if (this.characterModel)
			{
				this.characterModel.invisibilityCount--;
			}
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
			}
			if (NetworkServer.active)
			{
				base.characterBody.RemoveBuff(BuffIndex.HiddenInvincibility);
				base.characterBody.AddTimedBuff(BuffIndex.HiddenInvincibility, Evis.lingeringInvincibilityDuration);
			}
			Util.PlaySound(Evis.endSoundString, base.gameObject);
			base.SmallHop(base.characterMotor, Evis.smallHopVelocity);
			base.OnExit();
		}

		// Token: 0x040029A3 RID: 10659
		private Transform modelTransform;

		// Token: 0x040029A4 RID: 10660
		public static GameObject blinkPrefab;

		// Token: 0x040029A5 RID: 10661
		public static float duration = 2f;

		// Token: 0x040029A6 RID: 10662
		public static float damageCoefficient;

		// Token: 0x040029A7 RID: 10663
		public static float damageFrequency;

		// Token: 0x040029A8 RID: 10664
		public static float procCoefficient;

		// Token: 0x040029A9 RID: 10665
		public static string beginSoundString;

		// Token: 0x040029AA RID: 10666
		public static string endSoundString;

		// Token: 0x040029AB RID: 10667
		public static float maxRadius;

		// Token: 0x040029AC RID: 10668
		public static GameObject hitEffectPrefab;

		// Token: 0x040029AD RID: 10669
		public static string slashSoundString;

		// Token: 0x040029AE RID: 10670
		public static string impactSoundString;

		// Token: 0x040029AF RID: 10671
		public static string dashSoundString;

		// Token: 0x040029B0 RID: 10672
		public static float slashPitch;

		// Token: 0x040029B1 RID: 10673
		public static float smallHopVelocity;

		// Token: 0x040029B2 RID: 10674
		public static float lingeringInvincibilityDuration;

		// Token: 0x040029B3 RID: 10675
		private Animator animator;

		// Token: 0x040029B4 RID: 10676
		private CharacterModel characterModel;

		// Token: 0x040029B5 RID: 10677
		private float stopwatch;

		// Token: 0x040029B6 RID: 10678
		private float attackStopwatch;

		// Token: 0x040029B7 RID: 10679
		private bool crit;

		// Token: 0x040029B8 RID: 10680
		private static float minimumDuration = 0.5f;
	}
}
