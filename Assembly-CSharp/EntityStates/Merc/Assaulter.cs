using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Merc
{
	// Token: 0x020007C1 RID: 1985
	public class Assaulter : BaseState
	{
		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x06002D52 RID: 11602 RVA: 0x000BF40A File Offset: 0x000BD60A
		// (set) Token: 0x06002D53 RID: 11603 RVA: 0x000BF412 File Offset: 0x000BD612
		public bool hasHit { get; private set; }

		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x06002D54 RID: 11604 RVA: 0x000BF41B File Offset: 0x000BD61B
		// (set) Token: 0x06002D55 RID: 11605 RVA: 0x000BF423 File Offset: 0x000BD623
		public int dashIndex { private get; set; }

		// Token: 0x06002D56 RID: 11606 RVA: 0x000BF42C File Offset: 0x000BD62C
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
			if (NetworkServer.active)
			{
				base.characterBody.AddBuff(BuffIndex.HiddenInvincibility);
			}
		}

		// Token: 0x06002D57 RID: 11607 RVA: 0x000BF56C File Offset: 0x000BD76C
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

		// Token: 0x06002D58 RID: 11608 RVA: 0x000BF5E4 File Offset: 0x000BD7E4
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
						if (!this.hasHit)
						{
							this.hasHit = true;
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

		// Token: 0x06002D59 RID: 11609 RVA: 0x000BF894 File Offset: 0x000BDA94
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
			if (this.childLocator)
			{
				this.childLocator.FindChild("PreDashEffect").gameObject.SetActive(false);
			}
			base.PlayAnimation("FullBody, Override", "EvisLoopExit");
			if (NetworkServer.active)
			{
				base.characterBody.RemoveBuff(BuffIndex.HiddenInvincibility);
			}
			base.OnExit();
		}

		// Token: 0x06002D5A RID: 11610 RVA: 0x000BF975 File Offset: 0x000BDB75
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write((byte)this.dashIndex);
		}

		// Token: 0x06002D5B RID: 11611 RVA: 0x000BF98B File Offset: 0x000BDB8B
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.dashIndex = (int)reader.ReadByte();
		}

		// Token: 0x0400298B RID: 10635
		private Transform modelTransform;

		// Token: 0x0400298C RID: 10636
		public static GameObject dashPrefab;

		// Token: 0x0400298D RID: 10637
		public static float smallHopVelocity;

		// Token: 0x0400298E RID: 10638
		public static float dashPrepDuration;

		// Token: 0x0400298F RID: 10639
		public static float dashDuration = 0.3f;

		// Token: 0x04002990 RID: 10640
		public static float speedCoefficient = 25f;

		// Token: 0x04002991 RID: 10641
		public static string beginSoundString;

		// Token: 0x04002992 RID: 10642
		public static string endSoundString;

		// Token: 0x04002993 RID: 10643
		public static float damageCoefficient;

		// Token: 0x04002994 RID: 10644
		public static float procCoefficient;

		// Token: 0x04002995 RID: 10645
		public static GameObject hitEffectPrefab;

		// Token: 0x04002996 RID: 10646
		public static float hitPauseDuration;

		// Token: 0x04002997 RID: 10647
		private float stopwatch;

		// Token: 0x04002998 RID: 10648
		private Vector3 dashVector = Vector3.zero;

		// Token: 0x04002999 RID: 10649
		private Animator animator;

		// Token: 0x0400299A RID: 10650
		private CharacterModel characterModel;

		// Token: 0x0400299B RID: 10651
		private HurtBoxGroup hurtboxGroup;

		// Token: 0x0400299C RID: 10652
		private OverlapAttack overlapAttack;

		// Token: 0x0400299D RID: 10653
		private ChildLocator childLocator;

		// Token: 0x0400299E RID: 10654
		private bool isDashing;

		// Token: 0x0400299F RID: 10655
		private bool inHitPause;

		// Token: 0x040029A0 RID: 10656
		private float hitPauseTimer;
	}
}
