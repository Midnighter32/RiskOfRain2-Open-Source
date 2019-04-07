using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Barrel
{
	// Token: 0x020001DF RID: 479
	public class OpeningLunar : BaseState
	{
		// Token: 0x06000954 RID: 2388 RVA: 0x0002ED6C File Offset: 0x0002CF6C
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

		// Token: 0x06000955 RID: 2389 RVA: 0x0002EDC3 File Offset: 0x0002CFC3
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= OpeningLunar.duration)
			{
				this.outer.SetNextState(new Opened());
				return;
			}
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x0002EDEC File Offset: 0x0002CFEC
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

		// Token: 0x04000CA5 RID: 3237
		public static float duration = 1f;
	}
}
