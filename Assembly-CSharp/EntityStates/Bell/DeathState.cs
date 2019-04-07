using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Bell
{
	// Token: 0x020001C5 RID: 453
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x060008DA RID: 2266 RVA: 0x0002677F File Offset: 0x0002497F
		public override void OnEnter()
		{
			if (NetworkServer.active)
			{
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x060008DB RID: 2267 RVA: 0x0002CA80 File Offset: 0x0002AC80
		public override void OnExit()
		{
			base.OnExit();
			Util.PlaySound(DeathState.initialSoundString, base.gameObject);
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform && NetworkServer.active && modelTransform.GetComponent<ChildLocator>() && DeathState.initialEffect)
			{
				EffectManager.instance.SpawnEffect(DeathState.initialEffect, new EffectData
				{
					origin = base.transform.position,
					scale = DeathState.initialEffectScale
				}, true);
			}
			Rigidbody component = base.GetComponent<Rigidbody>();
			RagdollController component2 = modelTransform.GetComponent<RagdollController>();
			if (component2 && component)
			{
				component2.BeginRagdoll(component.velocity * DeathState.velocityMagnitude);
			}
			ExplodeRigidbodiesOnStart component3 = modelTransform.GetComponent<ExplodeRigidbodiesOnStart>();
			if (component3)
			{
				component3.force = DeathState.explosionForce;
				component3.enabled = true;
			}
		}

		// Token: 0x04000C00 RID: 3072
		public static string initialSoundString;

		// Token: 0x04000C01 RID: 3073
		public static GameObject initialEffect;

		// Token: 0x04000C02 RID: 3074
		public static float initialEffectScale;

		// Token: 0x04000C03 RID: 3075
		public static float velocityMagnitude;

		// Token: 0x04000C04 RID: 3076
		public static float explosionForce;
	}
}
