using System;
using System.Collections.Generic;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002B1 RID: 689
	public class OrbFireZone : MonoBehaviour
	{
		// Token: 0x06000F9E RID: 3998 RVA: 0x0000409B File Offset: 0x0000229B
		private void Awake()
		{
		}

		// Token: 0x06000F9F RID: 3999 RVA: 0x0004472C File Offset: 0x0004292C
		private void FixedUpdate()
		{
			if (this.previousColliderList.Count > 0)
			{
				this.resetStopwatch += Time.fixedDeltaTime;
				this.removeFromBottomOfListStopwatch += Time.fixedDeltaTime;
				if (this.removeFromBottomOfListStopwatch > 1f / this.orbRemoveFromBottomOfListFrequency)
				{
					this.removeFromBottomOfListStopwatch -= 1f / this.orbRemoveFromBottomOfListFrequency;
					this.previousColliderList.RemoveAt(this.previousColliderList.Count - 1);
				}
				if (this.resetStopwatch > 1f / this.orbResetListFrequency)
				{
					this.resetStopwatch -= 1f / this.orbResetListFrequency;
					this.previousColliderList.Clear();
				}
			}
		}

		// Token: 0x06000FA0 RID: 4000 RVA: 0x000447EC File Offset: 0x000429EC
		private void OnTriggerStay(Collider other)
		{
			if (NetworkServer.active)
			{
				if (this.previousColliderList.Contains(other))
				{
					return;
				}
				this.previousColliderList.Add(other);
				CharacterBody component = other.GetComponent<CharacterBody>();
				if (component && component.mainHurtBox)
				{
					DamageOrb damageOrb = new DamageOrb();
					damageOrb.attacker = null;
					damageOrb.damageOrbType = DamageOrb.DamageOrbType.ClayGooOrb;
					damageOrb.procCoefficient = this.procCoefficient;
					damageOrb.damageValue = this.baseDamage * Run.instance.teamlessDamageCoefficient;
					damageOrb.target = component.mainHurtBox;
					damageOrb.teamIndex = TeamIndex.None;
					RaycastHit raycastHit;
					if (Physics.Raycast(damageOrb.target.transform.position + UnityEngine.Random.insideUnitSphere * 3f, Vector3.down, out raycastHit, 1000f, LayerIndex.world.mask))
					{
						damageOrb.origin = raycastHit.point;
						OrbManager.instance.AddOrb(damageOrb);
					}
				}
			}
		}

		// Token: 0x04000F07 RID: 3847
		public float baseDamage;

		// Token: 0x04000F08 RID: 3848
		public float procCoefficient;

		// Token: 0x04000F09 RID: 3849
		public float orbRemoveFromBottomOfListFrequency;

		// Token: 0x04000F0A RID: 3850
		public float orbResetListFrequency;

		// Token: 0x04000F0B RID: 3851
		private List<Collider> previousColliderList = new List<Collider>();

		// Token: 0x04000F0C RID: 3852
		private float resetStopwatch;

		// Token: 0x04000F0D RID: 3853
		private float removeFromBottomOfListStopwatch;
	}
}
