using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Barrel
{
	// Token: 0x020001DC RID: 476
	public class ActivateFan : EntityState
	{
		// Token: 0x0600094C RID: 2380 RVA: 0x0002EC28 File Offset: 0x0002CE28
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Base", "IdleToActive");
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					component.FindChild("JumpVolume").gameObject.SetActive(true);
					component.FindChild("LightBack").gameObject.SetActive(true);
					component.FindChild("LightFront").gameObject.SetActive(true);
				}
			}
			if (base.sfxLocator)
			{
				Util.PlaySound(base.sfxLocator.openSound, base.gameObject);
			}
		}
	}
}
