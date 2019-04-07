using System;
using System.Linq;
using RoR2;
using UnityEngine;

namespace EntityStates.Merc
{
	// Token: 0x02000105 RID: 261
	public class Evis : BaseState
	{
		// Token: 0x0600050E RID: 1294 RVA: 0x00015A04 File Offset: 0x00013C04
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
				this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
			}
			if (this.characterModel)
			{
				this.characterModel.invisibilityCount++;
			}
			if (this.hurtboxGroup)
			{
				HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
				int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
				hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
			}
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
			}
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x00015B04 File Offset: 0x00013D04
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
						EffectManager.instance.SimpleImpactEffect(Evis.hitEffectPrefab, position, normal, false);
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

		// Token: 0x06000510 RID: 1296 RVA: 0x00015D7C File Offset: 0x00013F7C
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

		// Token: 0x06000511 RID: 1297 RVA: 0x00015DF0 File Offset: 0x00013FF0
		private void CreateBlinkEffect(Vector3 origin)
		{
			EffectData effectData = new EffectData();
			effectData.rotation = Util.QuaternionSafeLookRotation(Vector3.up);
			effectData.origin = origin;
			EffectManager.instance.SpawnEffect(Evis.blinkPrefab, effectData, false);
		}

		// Token: 0x06000512 RID: 1298 RVA: 0x00015E2C File Offset: 0x0001402C
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
			if (this.hurtboxGroup)
			{
				HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
				int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
				hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
			}
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
			}
			Util.PlaySound(Evis.endSoundString, base.gameObject);
			base.SmallHop(base.characterMotor, Evis.smallHopVelocity);
			base.OnExit();
		}

		// Token: 0x040004FF RID: 1279
		private Transform modelTransform;

		// Token: 0x04000500 RID: 1280
		public static GameObject blinkPrefab;

		// Token: 0x04000501 RID: 1281
		public static float duration = 2f;

		// Token: 0x04000502 RID: 1282
		public static float damageCoefficient;

		// Token: 0x04000503 RID: 1283
		public static float damageFrequency;

		// Token: 0x04000504 RID: 1284
		public static float procCoefficient;

		// Token: 0x04000505 RID: 1285
		public static string beginSoundString;

		// Token: 0x04000506 RID: 1286
		public static string endSoundString;

		// Token: 0x04000507 RID: 1287
		public static float maxRadius;

		// Token: 0x04000508 RID: 1288
		public static GameObject hitEffectPrefab;

		// Token: 0x04000509 RID: 1289
		public static string slashSoundString;

		// Token: 0x0400050A RID: 1290
		public static string impactSoundString;

		// Token: 0x0400050B RID: 1291
		public static string dashSoundString;

		// Token: 0x0400050C RID: 1292
		public static float slashPitch;

		// Token: 0x0400050D RID: 1293
		public static float smallHopVelocity;

		// Token: 0x0400050E RID: 1294
		private Animator animator;

		// Token: 0x0400050F RID: 1295
		private CharacterModel characterModel;

		// Token: 0x04000510 RID: 1296
		private HurtBoxGroup hurtboxGroup;

		// Token: 0x04000511 RID: 1297
		private float stopwatch;

		// Token: 0x04000512 RID: 1298
		private float attackStopwatch;

		// Token: 0x04000513 RID: 1299
		private bool crit;

		// Token: 0x04000514 RID: 1300
		private static float minimumDuration = 0.5f;
	}
}
