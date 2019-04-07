using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002B9 RID: 697
	public class ConvertPlayerMoneyToExperience : MonoBehaviour
	{
		// Token: 0x06000E2A RID: 3626 RVA: 0x00045C41 File Offset: 0x00043E41
		private void Start()
		{
			if (!NetworkServer.active)
			{
				Debug.LogErrorFormat("Component {0} can only be added on the server!", new object[]
				{
					base.GetType().Name
				});
				UnityEngine.Object.Destroy(this);
				return;
			}
			this.burstTimer = 0f;
		}

		// Token: 0x06000E2B RID: 3627 RVA: 0x00045C7C File Offset: 0x00043E7C
		private void FixedUpdate()
		{
			this.burstTimer -= Time.fixedDeltaTime;
			if (this.burstTimer <= 0f)
			{
				bool flag = false;
				ReadOnlyCollection<PlayerCharacterMasterController> instances = PlayerCharacterMasterController.instances;
				for (int i = 0; i < instances.Count; i++)
				{
					GameObject gameObject = instances[i].gameObject;
					CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
					uint num;
					if (!this.burstSizes.TryGetValue(gameObject, out num))
					{
						num = (uint)Mathf.CeilToInt(component.money / (float)this.burstCount);
						this.burstSizes[gameObject] = num;
					}
					if (num > component.money)
					{
						num = component.money;
					}
					component.money -= num;
					GameObject bodyObject = component.GetBodyObject();
					ulong num2 = (ulong)(num / 2f / (float)instances.Count);
					if (num > 0u)
					{
						flag = true;
					}
					if (bodyObject)
					{
						ExperienceManager.instance.AwardExperience(base.transform.position, bodyObject.GetComponent<CharacterBody>(), num2);
					}
					else
					{
						TeamManager.instance.GiveTeamExperience(component.teamIndex, num2);
					}
				}
				if (flag)
				{
					this.burstTimer = this.burstInterval;
					return;
				}
				if (this.burstTimer < -2.5f)
				{
					UnityEngine.Object.Destroy(this);
				}
			}
		}

		// Token: 0x04001213 RID: 4627
		private Dictionary<GameObject, uint> burstSizes = new Dictionary<GameObject, uint>();

		// Token: 0x04001214 RID: 4628
		private float burstTimer;

		// Token: 0x04001215 RID: 4629
		public float burstInterval = 0.25f;

		// Token: 0x04001216 RID: 4630
		public int burstCount = 8;
	}
}
