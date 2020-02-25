﻿using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Barrel
{
	// Token: 0x020008F7 RID: 2295
	public class ActivateFan : EntityState
	{
		// Token: 0x0600334C RID: 13132 RVA: 0x000DE84C File Offset: 0x000DCA4C
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
