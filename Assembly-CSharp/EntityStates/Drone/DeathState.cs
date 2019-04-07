using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Drone
{
	// Token: 0x02000193 RID: 403
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x060007C1 RID: 1985 RVA: 0x000265EC File Offset: 0x000247EC
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(DeathState.initialSoundString, base.gameObject);
			if (base.rigidbodyMotor)
			{
				base.rigidbodyMotor.forcePID.enabled = false;
				base.rigidbodyMotor.rigid.useGravity = true;
				base.rigidbodyMotor.rigid.AddForce(Vector3.up * DeathState.forceAmount, ForceMode.Force);
				base.rigidbodyMotor.rigid.collisionDetectionMode = CollisionDetectionMode.Continuous;
			}
			if (base.rigidbodyDirection)
			{
				base.rigidbodyDirection.enabled = false;
			}
			if (DeathState.initialExplosionEffect)
			{
				EffectManager.instance.SpawnEffect(DeathState.deathExplosionEffect, new EffectData
				{
					origin = base.characterBody.corePosition,
					scale = base.characterBody.radius + DeathState.deathEffectRadius
				}, false);
			}
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x000266D2 File Offset: 0x000248D2
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && base.fixedAge > DeathState.deathDuration)
			{
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x000266FC File Offset: 0x000248FC
		public override void OnExit()
		{
			if (DeathState.deathExplosionEffect)
			{
				EffectManager.instance.SpawnEffect(DeathState.deathExplosionEffect, new EffectData
				{
					origin = base.characterBody.corePosition,
					scale = base.characterBody.radius + DeathState.deathEffectRadius
				}, false);
			}
			Util.PlaySound(DeathState.deathSoundString, base.gameObject);
			base.OnExit();
		}

		// Token: 0x04000A0A RID: 2570
		public static GameObject initialExplosionEffect;

		// Token: 0x04000A0B RID: 2571
		public static GameObject deathExplosionEffect;

		// Token: 0x04000A0C RID: 2572
		public static string initialSoundString;

		// Token: 0x04000A0D RID: 2573
		public static string deathSoundString;

		// Token: 0x04000A0E RID: 2574
		public static float deathEffectRadius;

		// Token: 0x04000A0F RID: 2575
		public static float forceAmount = 20f;

		// Token: 0x04000A10 RID: 2576
		public static float deathDuration = 2f;
	}
}
