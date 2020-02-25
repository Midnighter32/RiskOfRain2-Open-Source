using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Drone
{
	// Token: 0x02000899 RID: 2201
	public class MegaDroneDeathState : GenericCharacterDeath
	{
		// Token: 0x0600315B RID: 12635 RVA: 0x000D4772 File Offset: 0x000D2972
		public override void OnEnter()
		{
			if (NetworkServer.active)
			{
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x0600315C RID: 12636 RVA: 0x000D4788 File Offset: 0x000D2988
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
						EffectManager.SpawnEffect(MegaDroneDeathState.initialEffect, new EffectData
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

		// Token: 0x04002FB0 RID: 12208
		public static string initialSoundString;

		// Token: 0x04002FB1 RID: 12209
		public static GameObject initialEffect;

		// Token: 0x04002FB2 RID: 12210
		public static float initialEffectScale;

		// Token: 0x04002FB3 RID: 12211
		public static float velocityMagnitude;

		// Token: 0x04002FB4 RID: 12212
		public static float explosionForce;
	}
}
