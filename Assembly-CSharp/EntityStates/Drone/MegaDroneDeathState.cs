using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Drone
{
	// Token: 0x02000194 RID: 404
	public class MegaDroneDeathState : GenericCharacterDeath
	{
		// Token: 0x060007C6 RID: 1990 RVA: 0x0002677F File Offset: 0x0002497F
		public override void OnEnter()
		{
			if (NetworkServer.active)
			{
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x060007C7 RID: 1991 RVA: 0x00026794 File Offset: 0x00024994
		public override void OnExit()
		{
			base.OnExit();
			Util.PlaySound(MegaDroneDeathState.initialSoundString, base.gameObject);
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform && NetworkServer.active)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					component.FindChild("LeftJet").gameObject.SetActive(false);
					component.FindChild("RightJet").gameObject.SetActive(false);
					if (MegaDroneDeathState.initialEffect)
					{
						EffectManager.instance.SpawnEffect(MegaDroneDeathState.initialEffect, new EffectData
						{
							origin = base.transform.position,
							scale = MegaDroneDeathState.initialEffectScale
						}, true);
					}
				}
			}
			Rigidbody component2 = base.GetComponent<Rigidbody>();
			RagdollController component3 = modelTransform.GetComponent<RagdollController>();
			if (component3 && component2)
			{
				component3.BeginRagdoll(component2.velocity * MegaDroneDeathState.velocityMagnitude);
			}
			ExplodeRigidbodiesOnStart component4 = modelTransform.GetComponent<ExplodeRigidbodiesOnStart>();
			if (component4)
			{
				component4.force = MegaDroneDeathState.explosionForce;
				component4.enabled = true;
			}
		}

		// Token: 0x04000A11 RID: 2577
		public static string initialSoundString;

		// Token: 0x04000A12 RID: 2578
		public static GameObject initialEffect;

		// Token: 0x04000A13 RID: 2579
		public static float initialEffectScale;

		// Token: 0x04000A14 RID: 2580
		public static float velocityMagnitude;

		// Token: 0x04000A15 RID: 2581
		public static float explosionForce;
	}
}
