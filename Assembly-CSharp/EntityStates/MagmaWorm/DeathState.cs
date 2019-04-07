using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.MagmaWorm
{
	// Token: 0x0200010E RID: 270
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x0600052F RID: 1327 RVA: 0x00016D0C File Offset: 0x00014F0C
		public override void OnEnter()
		{
			base.OnEnter();
			WormBodyPositions2 component = base.GetComponent<WormBodyPositions2>();
			if (component)
			{
				component.yDamperConstant = 0f;
				component.ySpringConstant = 0f;
				component.maxTurnSpeed = 0f;
				component.meatballCount = 0;
				Util.PlaySound(DeathState.deathSoundString, component.bones[0].gameObject);
			}
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				PrintController printController = modelTransform.gameObject.AddComponent<PrintController>();
				printController.printTime = DeathState.duration;
				printController.enabled = true;
				printController.startingPrintHeight = 99999f;
				printController.maxPrintHeight = 99999f;
				printController.startingPrintBias = 1f;
				printController.maxPrintBias = 3.5f;
				printController.animateFlowmapPower = true;
				printController.startingFlowmapPower = 1.14f;
				printController.maxFlowmapPower = 30f;
				printController.disableWhenFinished = false;
				printController.printCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
				ParticleSystem[] componentsInChildren = modelTransform.GetComponentsInChildren<ParticleSystem>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].Stop();
				}
				ChildLocator component2 = modelTransform.GetComponent<ChildLocator>();
				if (component2)
				{
					Transform transform = component2.FindChild("PP");
					if (transform)
					{
						PostProcessDuration component3 = transform.GetComponent<PostProcessDuration>();
						if (component3)
						{
							component3.enabled = true;
							component3.maxDuration = DeathState.duration;
						}
					}
				}
				if (NetworkServer.active)
				{
					EffectManager.instance.SimpleMuzzleFlash(DeathState.initialDeathExplosionEffect, base.gameObject, "HeadCenter", true);
				}
			}
		}

		// Token: 0x06000530 RID: 1328 RVA: 0x00016E9A File Offset: 0x0001509A
		public override void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			if (NetworkServer.active && this.stopwatch > DeathState.duration)
			{
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x04000561 RID: 1377
		public static GameObject initialDeathExplosionEffect;

		// Token: 0x04000562 RID: 1378
		public static string deathSoundString;

		// Token: 0x04000563 RID: 1379
		public static float duration;

		// Token: 0x04000564 RID: 1380
		private float stopwatch;
	}
}
