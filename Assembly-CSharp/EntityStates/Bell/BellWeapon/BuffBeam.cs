using System;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Bell.BellWeapon
{
	// Token: 0x020001C7 RID: 455
	internal class BuffBeam : BaseState
	{
		// Token: 0x060008E2 RID: 2274 RVA: 0x0002CBC0 File Offset: 0x0002ADC0
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(BuffBeam.playBeamSoundString, base.gameObject);
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
			Debug.LogFormat("Buffing target {0}", new object[]
			{
				this.target
			});
			if (this.target)
			{
				this.targetBody = this.target.healthComponent.body;
				this.targetBody.AddBuff(BuffIndex.Invincibility);
			}
			string childName = "Muzzle";
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					this.muzzleTransform = component.FindChild(childName);
					this.buffBeamInstance = UnityEngine.Object.Instantiate<GameObject>(BuffBeam.buffBeamPrefab);
					ChildLocator component2 = this.buffBeamInstance.GetComponent<ChildLocator>();
					if (component2)
					{
						this.beamTipTransform = component2.FindChild("BeamTip");
					}
					this.healBeamCurve = this.buffBeamInstance.GetComponentInChildren<BezierCurveLine>();
				}
			}
		}

		// Token: 0x060008E3 RID: 2275 RVA: 0x0002CD48 File Offset: 0x0002AF48
		private void UpdateHealBeamVisuals()
		{
			float widthMultiplier = BuffBeam.beamWidthCurve.Evaluate(base.age / BuffBeam.duration);
			this.healBeamCurve.lineRenderer.widthMultiplier = widthMultiplier;
			this.healBeamCurve.v0 = this.muzzleTransform.forward * 3f;
			this.healBeamCurve.transform.position = this.muzzleTransform.position;
			if (this.target)
			{
				this.beamTipTransform.position = this.targetBody.mainHurtBox.transform.position;
			}
		}

		// Token: 0x060008E4 RID: 2276 RVA: 0x0002CDE5 File Offset: 0x0002AFE5
		public override void Update()
		{
			base.Update();
			this.UpdateHealBeamVisuals();
		}

		// Token: 0x060008E5 RID: 2277 RVA: 0x0002CDF3 File Offset: 0x0002AFF3
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if ((base.fixedAge >= BuffBeam.duration || this.target == null) && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060008E6 RID: 2278 RVA: 0x0002CE2A File Offset: 0x0002B02A
		public override void OnExit()
		{
			Util.PlaySound(BuffBeam.stopBeamSoundString, base.gameObject);
			EntityState.Destroy(this.buffBeamInstance);
			if (this.targetBody)
			{
				this.targetBody.RemoveBuff(BuffIndex.Invincibility);
			}
			base.OnExit();
		}

		// Token: 0x060008E7 RID: 2279 RVA: 0x0000A1ED File Offset: 0x000083ED
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Any;
		}

		// Token: 0x060008E8 RID: 2280 RVA: 0x0002CE68 File Offset: 0x0002B068
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			HurtBoxReference.FromHurtBox(this.target).Write(writer);
		}

		// Token: 0x060008E9 RID: 2281 RVA: 0x0002CE90 File Offset: 0x0002B090
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			HurtBoxReference hurtBoxReference = default(HurtBoxReference);
			hurtBoxReference.Read(reader);
			GameObject gameObject = hurtBoxReference.ResolveGameObject();
			this.target = ((gameObject != null) ? gameObject.GetComponent<HurtBox>() : null);
		}

		// Token: 0x04000C07 RID: 3079
		public static float duration;

		// Token: 0x04000C08 RID: 3080
		public static GameObject buffBeamPrefab;

		// Token: 0x04000C09 RID: 3081
		public static AnimationCurve beamWidthCurve;

		// Token: 0x04000C0A RID: 3082
		public static string playBeamSoundString;

		// Token: 0x04000C0B RID: 3083
		public static string stopBeamSoundString;

		// Token: 0x04000C0C RID: 3084
		public HurtBox target;

		// Token: 0x04000C0D RID: 3085
		private float healTimer;

		// Token: 0x04000C0E RID: 3086
		private float healInterval;

		// Token: 0x04000C0F RID: 3087
		private float healChunk;

		// Token: 0x04000C10 RID: 3088
		private CharacterBody targetBody;

		// Token: 0x04000C11 RID: 3089
		private GameObject buffBeamInstance;

		// Token: 0x04000C12 RID: 3090
		private BezierCurveLine healBeamCurve;

		// Token: 0x04000C13 RID: 3091
		private Transform muzzleTransform;

		// Token: 0x04000C14 RID: 3092
		private Transform beamTipTransform;
	}
}
