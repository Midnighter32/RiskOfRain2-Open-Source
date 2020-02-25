using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Barrel
{
	// Token: 0x020008FA RID: 2298
	public class OpeningLunar : BaseState
	{
		// Token: 0x06003354 RID: 13140 RVA: 0x000DE990 File Offset: 0x000DCB90
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Opening", "Opening.playbackRate", OpeningLunar.duration);
			if (base.sfxLocator)
			{
				Util.PlaySound(base.sfxLocator.openSound, base.gameObject);
			}
			this.StopSteamEffect();
		}

		// Token: 0x06003355 RID: 13141 RVA: 0x000DE9E7 File Offset: 0x000DCBE7
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= OpeningLunar.duration)
			{
				this.outer.SetNextState(new Opened());
				return;
			}
		}

		// Token: 0x06003356 RID: 13142 RVA: 0x000DEA10 File Offset: 0x000DCC10
		private void StopSteamEffect()
		{
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("SteamEffect");
					if (transform)
					{
						ParticleSystem component2 = transform.GetComponent<ParticleSystem>();
						if (component2)
						{
							component2.main.loop = false;
						}
					}
				}
			}
		}

		// Token: 0x040032CE RID: 13006
		public static float duration = 1f;
	}
}
