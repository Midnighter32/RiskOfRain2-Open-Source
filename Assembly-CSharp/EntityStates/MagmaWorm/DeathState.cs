using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.MagmaWorm
{
	// Token: 0x02000815 RID: 2069
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x06002EEE RID: 12014 RVA: 0x000C7B2C File Offset: 0x000C5D2C
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
					EffectManager.SimpleMuzzleFlash(DeathState.initialDeathExplosionEffect, base.gameObject, "HeadCenter", true);
				}
			}
		}

		// Token: 0x06002EEF RID: 12015 RVA: 0x000C7CB5 File Offset: 0x000C5EB5
		public override void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			if (NetworkServer.active && this.stopwatch > DeathState.duration)
			{
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x04002C2F RID: 11311
		public static GameObject initialDeathExplosionEffect;

		// Token: 0x04002C30 RID: 11312
		public static string deathSoundString;

		// Token: 0x04002C31 RID: 11313
		public static float duration;

		// Token: 0x04002C32 RID: 11314
		private float stopwatch;
	}
}
