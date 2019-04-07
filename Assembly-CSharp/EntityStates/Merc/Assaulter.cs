using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Merc
{
	// Token: 0x02000104 RID: 260
	public class Assaulter : BaseState
	{
		// Token: 0x06000506 RID: 1286 RVA: 0x000153EC File Offset: 0x000135EC
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(Assaulter.beginSoundString, base.gameObject);
			this.modelTransform = base.GetModelTransform();
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
			}
			if (this.modelTransform)
			{
				this.animator = this.modelTransform.GetComponent<Animator>();
				this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
				this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
				this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
				if (this.childLocator)
				{
					this.childLocator.FindChild("PreDashEffect").gameObject.SetActive(true);
				}
			}
			base.SmallHop(base.characterMotor, Assaulter.smallHopVelocity);
			base.PlayAnimation("FullBody, Override", "AssaulterPrep", "AssaulterPrep.playbackRate", Assaulter.dashPrepDuration);
			this.dashVector = base.inputBank.aimDirection;
			this.overlapAttack = base.InitMeleeOverlap(Assaulter.damageCoefficient, Assaulter.hitEffectPrefab, this.modelTransform, "Assaulter");
			this.overlapAttack.damageType = DamageType.Stun1s;
			this.dashSkill = base.GetComponent<MercDashSkill>();
			if (this.dashSkill && base.isAuthority)
			{
				this.dashIndex = this.dashSkill.currentDashIndex;
			}
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x0001554C File Offset: 0x0001374C
		private void CreateDashEffect()
		{
			Transform transform = this.childLocator.FindChild("DashCenter");
			if (transform && Assaulter.dashPrefab)
			{
				UnityEngine.Object.Instantiate<GameObject>(Assaulter.dashPrefab, transform.position, Util.QuaternionSafeLookRotation(this.dashVector), transform);
			}
			if (this.childLocator)
			{
				this.childLocator.FindChild("PreDashEffect").gameObject.SetActive(false);
			}
		}

		// Token: 0x06000508 RID: 1288 RVA: 0x000155C4 File Offset: 0x000137C4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			base.characterDirection.forward = this.dashVector;
			if (this.stopwatch > Assaulter.dashPrepDuration / this.attackSpeedStat && !this.isDashing)
			{
				this.isDashing = true;
				this.dashVector = base.inputBank.aimDirection;
				this.CreateDashEffect();
				base.PlayCrossfade("FullBody, Override", "AssaulterLoop", 0.1f);
				base.gameObject.layer = LayerIndex.fakeActor.intVal;
				base.characterMotor.Motor.RebuildCollidableLayers();
				if (this.hurtboxGroup)
				{
					HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
					int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
					hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
				}
				if (this.modelTransform)
				{
					TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
					temporaryOverlay.duration = 0.7f;
					temporaryOverlay.animateShaderAlpha = true;
					temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
					temporaryOverlay.destroyComponentOnEnd = true;
					temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matMercEnergized");
					temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
				}
			}
			if (!this.isDashing)
			{
				this.stopwatch += Time.fixedDeltaTime;
			}
			else if (base.isAuthority)
			{
				base.characterMotor.velocity = Vector3.zero;
				if (!this.inHitPause)
				{
					bool flag = this.overlapAttack.Fire(null);
					this.stopwatch += Time.fixedDeltaTime;
					if (flag)
					{
						Util.PlaySound(Assaulter.impactSoundString, base.gameObject);
						if (!this.hasHit)
						{
							this.hasHit = true;
							if (this.dashSkill)
							{
								this.dashSkill.AddHit();
							}
						}
						this.inHitPause = true;
						this.hitPauseTimer = Assaulter.hitPauseDuration / this.attackSpeedStat;
						if (this.modelTransform)
						{
							TemporaryOverlay temporaryOverlay2 = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
							temporaryOverlay2.duration = Assaulter.hitPauseDuration / this.attackSpeedStat;
							temporaryOverlay2.animateShaderAlpha = true;
							temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
							temporaryOverlay2.destroyComponentOnEnd = true;
							temporaryOverlay2.originalMaterial = Resources.Load<Material>("Materials/matMercEvisTarget");
							temporaryOverlay2.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
						}
					}
					base.characterMotor.rootMotion += this.dashVector * this.moveSpeedStat * Assaulter.speedCoefficient * Time.fixedDeltaTime;
				}
				else
				{
					this.hitPauseTimer -= Time.fixedDeltaTime;
					if (this.hitPauseTimer < 0f)
					{
						this.inHitPause = false;
					}
				}
			}
			if (this.stopwatch >= Assaulter.dashDuration + Assaulter.dashPrepDuration / this.attackSpeedStat && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x000158C0 File Offset: 0x00013AC0
		public override void OnExit()
		{
			base.gameObject.layer = LayerIndex.defaultLayer.intVal;
			base.characterMotor.Motor.RebuildCollidableLayers();
			Util.PlaySound(Assaulter.endSoundString, base.gameObject);
			if (base.isAuthority)
			{
				base.characterMotor.velocity *= 0.1f;
				base.SmallHop(base.characterMotor, Assaulter.smallHopVelocity);
			}
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
			}
			if (this.hurtboxGroup)
			{
				HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
				int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
				hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
			}
			if (this.childLocator)
			{
				this.childLocator.FindChild("PreDashEffect").gameObject.SetActive(false);
			}
			base.PlayAnimation("FullBody, Override", "EvisLoopExit");
			base.OnExit();
		}

		// Token: 0x0600050A RID: 1290 RVA: 0x000159B0 File Offset: 0x00013BB0
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write((byte)this.dashIndex);
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x000159C6 File Offset: 0x00013BC6
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.dashIndex = (int)reader.ReadByte();
		}

		// Token: 0x040004E5 RID: 1253
		private Transform modelTransform;

		// Token: 0x040004E6 RID: 1254
		public static GameObject dashPrefab;

		// Token: 0x040004E7 RID: 1255
		public static float smallHopVelocity;

		// Token: 0x040004E8 RID: 1256
		public static float dashPrepDuration;

		// Token: 0x040004E9 RID: 1257
		public static float dashDuration = 0.3f;

		// Token: 0x040004EA RID: 1258
		public static float speedCoefficient = 25f;

		// Token: 0x040004EB RID: 1259
		public static string beginSoundString;

		// Token: 0x040004EC RID: 1260
		public static string endSoundString;

		// Token: 0x040004ED RID: 1261
		public static float damageCoefficient;

		// Token: 0x040004EE RID: 1262
		public static float procCoefficient;

		// Token: 0x040004EF RID: 1263
		public static GameObject hitEffectPrefab;

		// Token: 0x040004F0 RID: 1264
		public static float hitPauseDuration;

		// Token: 0x040004F1 RID: 1265
		public static string impactSoundString;

		// Token: 0x040004F2 RID: 1266
		private float stopwatch;

		// Token: 0x040004F3 RID: 1267
		private Vector3 dashVector = Vector3.zero;

		// Token: 0x040004F4 RID: 1268
		private Animator animator;

		// Token: 0x040004F5 RID: 1269
		private CharacterModel characterModel;

		// Token: 0x040004F6 RID: 1270
		private HurtBoxGroup hurtboxGroup;

		// Token: 0x040004F7 RID: 1271
		private OverlapAttack overlapAttack;

		// Token: 0x040004F8 RID: 1272
		private ChildLocator childLocator;

		// Token: 0x040004F9 RID: 1273
		private bool isDashing;

		// Token: 0x040004FA RID: 1274
		private bool inHitPause;

		// Token: 0x040004FB RID: 1275
		private float hitPauseTimer;

		// Token: 0x040004FC RID: 1276
		private bool hasHit;

		// Token: 0x040004FD RID: 1277
		private MercDashSkill dashSkill;

		// Token: 0x040004FE RID: 1278
		private int dashIndex;
	}
}
