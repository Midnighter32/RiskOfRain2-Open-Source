using System;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Bell.BellWeapon
{
	// Token: 0x020008E2 RID: 2274
	public class BuffBeam : BaseState
	{
		// Token: 0x060032E2 RID: 13026 RVA: 0x000DC838 File Offset: 0x000DAA38
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
				this.targetBody.AddBuff(BuffIndex.HiddenInvincibility);
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

		// Token: 0x060032E3 RID: 13027 RVA: 0x000DC9C0 File Offset: 0x000DABC0
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

		// Token: 0x060032E4 RID: 13028 RVA: 0x000DCA5D File Offset: 0x000DAC5D
		public override void Update()
		{
			base.Update();
			this.UpdateHealBeamVisuals();
		}

		// Token: 0x060032E5 RID: 13029 RVA: 0x000DCA6B File Offset: 0x000DAC6B
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if ((base.fixedAge >= BuffBeam.duration || this.target == null) && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060032E6 RID: 13030 RVA: 0x000DCAA2 File Offset: 0x000DACA2
		public override void OnExit()
		{
			Util.PlaySound(BuffBeam.stopBeamSoundString, base.gameObject);
			EntityState.Destroy(this.buffBeamInstance);
			if (this.targetBody)
			{
				this.targetBody.RemoveBuff(BuffIndex.HiddenInvincibility);
			}
			base.OnExit();
		}

		// Token: 0x060032E7 RID: 13031 RVA: 0x0000AC89 File Offset: 0x00008E89
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Any;
		}

		// Token: 0x060032E8 RID: 13032 RVA: 0x000DCAE0 File Offset: 0x000DACE0
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			HurtBoxReference.FromHurtBox(this.target).Write(writer);
		}

		// Token: 0x060032E9 RID: 13033 RVA: 0x000DCB08 File Offset: 0x000DAD08
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			HurtBoxReference hurtBoxReference = default(HurtBoxReference);
			hurtBoxReference.Read(reader);
			GameObject gameObject = hurtBoxReference.ResolveGameObject();
			this.target = ((gameObject != null) ? gameObject.GetComponent<HurtBox>() : null);
		}

		// Token: 0x0400322F RID: 12847
		public static float duration;

		// Token: 0x04003230 RID: 12848
		public static GameObject buffBeamPrefab;

		// Token: 0x04003231 RID: 12849
		public static AnimationCurve beamWidthCurve;

		// Token: 0x04003232 RID: 12850
		public static string playBeamSoundString;

		// Token: 0x04003233 RID: 12851
		public static string stopBeamSoundString;

		// Token: 0x04003234 RID: 12852
		public HurtBox target;

		// Token: 0x04003235 RID: 12853
		private float healTimer;

		// Token: 0x04003236 RID: 12854
		private float healInterval;

		// Token: 0x04003237 RID: 12855
		private float healChunk;

		// Token: 0x04003238 RID: 12856
		private CharacterBody targetBody;

		// Token: 0x04003239 RID: 12857
		private GameObject buffBeamInstance;

		// Token: 0x0400323A RID: 12858
		private BezierCurveLine healBeamCurve;

		// Token: 0x0400323B RID: 12859
		private Transform muzzleTransform;

		// Token: 0x0400323C RID: 12860
		private Transform beamTipTransform;
	}
}
