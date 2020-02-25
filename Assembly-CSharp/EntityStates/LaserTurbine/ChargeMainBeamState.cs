using System;
using RoR2;
using UnityEngine;

namespace EntityStates.LaserTurbine
{
	// Token: 0x020007FC RID: 2044
	public class ChargeMainBeamState : LaserTurbineBaseState
	{
		// Token: 0x06002E7D RID: 11901 RVA: 0x000C58A4 File Offset: 0x000C3AA4
		public override void OnEnter()
		{
			base.OnEnter();
			this.beamIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeMainBeamState.beamIndicatorPrefab, base.GetMuzzleTransform(), false);
			this.beamIndicatorChildLocator = this.beamIndicatorInstance.GetComponent<ChildLocator>();
			if (this.beamIndicatorChildLocator)
			{
				this.beamIndicatorEndTransform = this.beamIndicatorChildLocator.FindChild("End");
			}
		}

		// Token: 0x06002E7E RID: 11902 RVA: 0x000C5902 File Offset: 0x000C3B02
		public override void OnExit()
		{
			EntityState.Destroy(this.beamIndicatorInstance);
			base.OnExit();
		}

		// Token: 0x06002E7F RID: 11903 RVA: 0x000C5915 File Offset: 0x000C3B15
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= ChargeMainBeamState.baseDuration)
			{
				this.outer.SetNextState(new FireMainBeamState());
			}
		}

		// Token: 0x06002E80 RID: 11904 RVA: 0x000C5944 File Offset: 0x000C3B44
		public override void Update()
		{
			base.Update();
			if (this.beamIndicatorInstance && this.beamIndicatorEndTransform)
			{
				float num = 1000f;
				Ray aimRay = base.GetAimRay();
				Vector3 position = this.beamIndicatorInstance.transform.parent.position;
				Vector3 point = aimRay.GetPoint(num);
				RaycastHit raycastHit;
				if (Util.CharacterRaycast(base.ownerBody.gameObject, aimRay, out raycastHit, num, LayerIndex.entityPrecise.mask | LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal))
				{
					point = raycastHit.point;
				}
				this.beamIndicatorEndTransform.transform.position = point;
			}
		}

		// Token: 0x06002E81 RID: 11905 RVA: 0x000C5713 File Offset: 0x000C3913
		public override float GetChargeFraction()
		{
			return 1f;
		}

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x06002E82 RID: 11906 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected override bool shouldFollow
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04002B90 RID: 11152
		public static float baseDuration;

		// Token: 0x04002B91 RID: 11153
		public static GameObject beamIndicatorPrefab;

		// Token: 0x04002B92 RID: 11154
		private GameObject beamIndicatorInstance;

		// Token: 0x04002B93 RID: 11155
		private ChildLocator beamIndicatorChildLocator;

		// Token: 0x04002B94 RID: 11156
		private Transform beamIndicatorEndTransform;
	}
}
