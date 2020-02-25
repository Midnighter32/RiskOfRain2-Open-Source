using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000346 RID: 838
	public class SummonMasterBehavior : NetworkBehaviour
	{
		// Token: 0x060013EF RID: 5103 RVA: 0x00019B5A File Offset: 0x00017D5A
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x060013F0 RID: 5104 RVA: 0x000555F8 File Offset: 0x000537F8
		[Server]
		public void OpenSummon(Interactor activator)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.SummonMasterBehavior::OpenSummon(RoR2.Interactor)' called on client");
				return;
			}
			this.OpenSummonReturnMaster(activator);
		}

		// Token: 0x060013F1 RID: 5105 RVA: 0x00055618 File Offset: 0x00053818
		[Server]
		public CharacterMaster OpenSummonReturnMaster(Interactor activator)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'RoR2.CharacterMaster RoR2.SummonMasterBehavior::OpenSummonReturnMaster(RoR2.Interactor)' called on client");
				return null;
			}
			float d = 0f;
			CharacterMaster characterMaster = new MasterSummon
			{
				masterPrefab = this.masterPrefab,
				position = base.transform.position + Vector3.up * d,
				rotation = base.transform.rotation,
				summonerBodyObject = ((activator != null) ? activator.gameObject : null),
				ignoreTeamMemberLimit = true
			}.Perform();
			if (characterMaster)
			{
				GameObject bodyObject = characterMaster.GetBodyObject();
				if (bodyObject)
				{
					ModelLocator component = bodyObject.GetComponent<ModelLocator>();
					if (component && component.modelTransform)
					{
						TemporaryOverlay temporaryOverlay = component.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
						temporaryOverlay.duration = 0.5f;
						temporaryOverlay.animateShaderAlpha = true;
						temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
						temporaryOverlay.destroyComponentOnEnd = true;
						temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matSummonDrone");
						temporaryOverlay.AddToCharacerModel(component.modelTransform.GetComponent<CharacterModel>());
					}
				}
			}
			if (this.destroyAfterSummoning)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			return characterMaster;
		}

		// Token: 0x060013F2 RID: 5106 RVA: 0x00055763 File Offset: 0x00053963
		public void OnEnable()
		{
			if (this.callOnEquipmentSpentOnPurchase)
			{
				PurchaseInteraction.onEquipmentSpentOnPurchase += this.OnEquipmentSpentOnPurchase;
			}
		}

		// Token: 0x060013F3 RID: 5107 RVA: 0x0005577E File Offset: 0x0005397E
		public void OnDisable()
		{
			if (this.callOnEquipmentSpentOnPurchase)
			{
				PurchaseInteraction.onEquipmentSpentOnPurchase -= this.OnEquipmentSpentOnPurchase;
			}
		}

		// Token: 0x060013F4 RID: 5108 RVA: 0x0005579C File Offset: 0x0005399C
		[Server]
		private void OnEquipmentSpentOnPurchase(PurchaseInteraction purchaseInteraction, Interactor interactor, EquipmentIndex equipmentIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.SummonMasterBehavior::OnEquipmentSpentOnPurchase(RoR2.PurchaseInteraction,RoR2.Interactor,RoR2.EquipmentIndex)' called on client");
				return;
			}
			if (purchaseInteraction == base.GetComponent<PurchaseInteraction>())
			{
				CharacterMaster characterMaster = this.OpenSummonReturnMaster(interactor);
				if (characterMaster)
				{
					characterMaster.inventory.SetEquipmentIndex(equipmentIndex);
				}
			}
		}

		// Token: 0x060013F6 RID: 5110 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x060013F7 RID: 5111 RVA: 0x000557F8 File Offset: 0x000539F8
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060013F8 RID: 5112 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040012C7 RID: 4807
		[Tooltip("The master to spawn")]
		public GameObject masterPrefab;

		// Token: 0x040012C8 RID: 4808
		public bool callOnEquipmentSpentOnPurchase;

		// Token: 0x040012C9 RID: 4809
		public bool destroyAfterSummoning = true;
	}
}
