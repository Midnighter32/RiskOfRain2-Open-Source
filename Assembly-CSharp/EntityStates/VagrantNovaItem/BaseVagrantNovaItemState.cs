using System;
using RoR2;
using UnityEngine;

namespace EntityStates.VagrantNovaItem
{
	// Token: 0x0200073F RID: 1855
	public class BaseVagrantNovaItemState : BaseBodyAttachmentState
	{
		// Token: 0x06002B0C RID: 11020 RVA: 0x000B53DB File Offset: 0x000B35DB
		protected int GetItemStack()
		{
			if (!base.attachedBody || !base.attachedBody.inventory)
			{
				return 1;
			}
			return base.attachedBody.inventory.GetItemCount(ItemIndex.NovaOnLowHealth);
		}

		// Token: 0x06002B0D RID: 11021 RVA: 0x000B5410 File Offset: 0x000B3610
		public override void OnEnter()
		{
			base.OnEnter();
			ChildLocator component = base.GetComponent<ChildLocator>();
			if (component)
			{
				Transform transform = component.FindChild("ChargeSparks");
				this.chargeSparks = ((transform != null) ? transform.GetComponent<ParticleSystem>() : null);
				if (this.chargeSparks)
				{
					this.chargeSparks.shape.skinnedMeshRenderer = this.FindAttachedBodyMainRenderer();
				}
			}
		}

		// Token: 0x06002B0E RID: 11022 RVA: 0x000B5478 File Offset: 0x000B3678
		protected void SetChargeSparkEmissionRateMultiplier(float multiplier)
		{
			if (this.chargeSparks)
			{
				this.chargeSparks.emission.rateOverTimeMultiplier = multiplier * 20f;
			}
		}

		// Token: 0x06002B0F RID: 11023 RVA: 0x000B54AC File Offset: 0x000B36AC
		private SkinnedMeshRenderer FindAttachedBodyMainRenderer()
		{
			if (!base.attachedBody)
			{
				return null;
			}
			ModelLocator modelLocator = base.attachedBody.modelLocator;
			CharacterModel.RendererInfo[] array;
			if (modelLocator == null)
			{
				array = null;
			}
			else
			{
				CharacterModel component = modelLocator.modelTransform.GetComponent<CharacterModel>();
				array = ((component != null) ? component.baseRendererInfos : null);
			}
			CharacterModel.RendererInfo[] array2 = array;
			if (array2 == null)
			{
				return null;
			}
			for (int i = 0; i < array2.Length; i++)
			{
				SkinnedMeshRenderer result;
				if ((result = (array2[i].renderer as SkinnedMeshRenderer)) != null)
				{
					return result;
				}
			}
			return null;
		}

		// Token: 0x040026E9 RID: 9961
		protected ParticleSystem chargeSparks;
	}
}
