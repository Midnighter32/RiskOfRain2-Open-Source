using System;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Drone.DroneWeapon
{
	// Token: 0x0200019A RID: 410
	internal class HealBeam : BaseState
	{
		// Token: 0x060007E9 RID: 2025 RVA: 0x00027338 File Offset: 0x00025538
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(HealBeam.playHealSoundString, base.gameObject);
			base.PlayCrossfade("Gesture", "Heal", 0.2f);
			this.healInterval = HealBeam.duration / (float)HealBeam.healCount;
			this.healChunk = HealBeam.healCoefficient / (float)HealBeam.healCount;
			Ray aimRay = base.GetAimRay();
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.teamMaskFilter = TeamMask.none;
			if (base.teamComponent)
			{
				bullseyeSearch.teamMaskFilter.AddTeam(base.teamComponent.teamIndex);
			}
			bullseyeSearch.filterByLoS = false;
			bullseyeSearch.maxDistanceFilter = 50f;
			bullseyeSearch.maxAngleFilter = 180f;
			bullseyeSearch.searchOrigin = aimRay.origin;
			bullseyeSearch.searchDirection = aimRay.direction;
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
			bullseyeSearch.RefreshCandidates();
			bullseyeSearch.FilterOutGameObject(base.gameObject);
			this.target = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
			Debug.LogFormat("Healing target {0}", new object[]
			{
				this.target
			});
			if (this.target)
			{
				this.targetHealthComponent = this.target.healthComponent;
			}
			string childName = "Muzzle";
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					this.muzzleTransform = component.FindChild(childName);
					this.healBeam = UnityEngine.Object.Instantiate<GameObject>(HealBeam.healBeamPrefab, this.muzzleTransform);
					this.healBeamCurve = this.healBeam.GetComponent<BezierCurveLine>();
				}
			}
		}

		// Token: 0x060007EA RID: 2026 RVA: 0x000274C4 File Offset: 0x000256C4
		private void UpdateHealBeamVisuals()
		{
			float widthMultiplier = Mathf.SmoothDamp(this.healBeamCurve.lineRenderer.widthMultiplier, this.targetLineWidth, ref this.lineWidthRefVelocity, this.smoothTime);
			this.healBeamCurve.lineRenderer.widthMultiplier = widthMultiplier;
			this.healBeamCurve.v0 = this.muzzleTransform.forward * 8f;
			if (this.target)
			{
				this.healBeamCurve.p1 = this.target.transform.position;
			}
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x00027552 File Offset: 0x00025752
		public override void Update()
		{
			base.Update();
			this.UpdateHealBeamVisuals();
		}

		// Token: 0x060007EC RID: 2028 RVA: 0x00027560 File Offset: 0x00025760
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.healTimer += Time.fixedDeltaTime;
			if ((base.fixedAge >= HealBeam.duration || this.target == null) && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
			if (base.fixedAge >= HealBeam.duration - 0.6f)
			{
				this.targetLineWidth = 0f;
			}
			else
			{
				this.targetLineWidth = this.maxLineWidth;
			}
			if (this.healTimer >= this.healInterval)
			{
				this.healTimer -= this.healInterval;
				if (this.targetHealthComponent)
				{
					this.targetHealthComponent.Heal(this.healChunk * base.characterBody.damage, default(ProcChainMask), true);
				}
			}
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x00027636 File Offset: 0x00025836
		public override void OnExit()
		{
			base.PlayCrossfade("Gesture", "Empty", 0.2f);
			Util.PlaySound(HealBeam.stopHealSoundString, base.gameObject);
			EntityState.Destroy(this.healBeam);
			base.OnExit();
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x0000A1ED File Offset: 0x000083ED
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Any;
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x00027670 File Offset: 0x00025870
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			HurtBoxReference.FromHurtBox(this.target).Write(writer);
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x00027698 File Offset: 0x00025898
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			HurtBoxReference hurtBoxReference = default(HurtBoxReference);
			hurtBoxReference.Read(reader);
			GameObject gameObject = hurtBoxReference.ResolveGameObject();
			this.target = ((gameObject != null) ? gameObject.GetComponent<HurtBox>() : null);
		}

		// Token: 0x04000A56 RID: 2646
		public static float duration;

		// Token: 0x04000A57 RID: 2647
		public static float healCoefficient = 5f;

		// Token: 0x04000A58 RID: 2648
		public static int healCount = 5;

		// Token: 0x04000A59 RID: 2649
		public static GameObject healBeamPrefab;

		// Token: 0x04000A5A RID: 2650
		public static string playHealSoundString;

		// Token: 0x04000A5B RID: 2651
		public static string stopHealSoundString;

		// Token: 0x04000A5C RID: 2652
		public HurtBox target;

		// Token: 0x04000A5D RID: 2653
		private float healTimer;

		// Token: 0x04000A5E RID: 2654
		private float healInterval;

		// Token: 0x04000A5F RID: 2655
		private float healChunk;

		// Token: 0x04000A60 RID: 2656
		private HealthComponent targetHealthComponent;

		// Token: 0x04000A61 RID: 2657
		private GameObject healBeam;

		// Token: 0x04000A62 RID: 2658
		private BezierCurveLine healBeamCurve;

		// Token: 0x04000A63 RID: 2659
		private Transform muzzleTransform;

		// Token: 0x04000A64 RID: 2660
		private float lineWidthRefVelocity;

		// Token: 0x04000A65 RID: 2661
		private float maxLineWidth = 0.3f;

		// Token: 0x04000A66 RID: 2662
		private float targetLineWidth;

		// Token: 0x04000A67 RID: 2663
		private float smoothTime = 0.1f;
	}
}
