using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Bell
{
	// Token: 0x020008E0 RID: 2272
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x060032D9 RID: 13017 RVA: 0x000DC6DC File Offset: 0x000DA8DC
		public override void OnEnter()
		{
			base.OnEnter();
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform && modelTransform.GetComponent<ChildLocator>() && DeathState.initialEffect)
			{
				EffectManager.SpawnEffect(DeathState.initialEffect, new EffectData
				{
					origin = base.transform.position,
					scale = DeathState.initialEffectScale
				}, false);
			}
			if (modelTransform)
			{
				RagdollController component = modelTransform.GetComponent<RagdollController>();
				Rigidbody component2 = base.GetComponent<Rigidbody>();
				if (component && component2)
				{
					component.BeginRagdoll(component2.velocity * DeathState.velocityMagnitude);
				}
			}
			ExplodeRigidbodiesOnStart component3 = modelTransform.GetComponent<ExplodeRigidbodiesOnStart>();
			if (component3)
			{
				component3.force = DeathState.explosionForce;
				component3.enabled = true;
			}
		}

		// Token: 0x060032DA RID: 13018 RVA: 0x000DC7A0 File Offset: 0x000DA9A0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && base.fixedAge > 0.1f)
			{
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x060032DB RID: 13019 RVA: 0x000DC7C7 File Offset: 0x000DA9C7
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x04003229 RID: 12841
		public static GameObject initialEffect;

		// Token: 0x0400322A RID: 12842
		public static float initialEffectScale;

		// Token: 0x0400322B RID: 12843
		public static float velocityMagnitude;

		// Token: 0x0400322C RID: 12844
		public static float explosionForce;
	}
}
