using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001C8 RID: 456
	public class ConvertPlayerMoneyToExperience : MonoBehaviour
	{
		// Token: 0x060009C9 RID: 2505 RVA: 0x0002ABC1 File Offset: 0x00028DC1
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

		// Token: 0x060009CA RID: 2506 RVA: 0x0002ABFC File Offset: 0x00028DFC
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
					if (num > 0U)
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

		// Token: 0x040009FB RID: 2555
		private Dictionary<GameObject, uint> burstSizes = new Dictionary<GameObject, uint>();

		// Token: 0x040009FC RID: 2556
		private float burstTimer;

		// Token: 0x040009FD RID: 2557
		public float burstInterval = 0.25f;

		// Token: 0x040009FE RID: 2558
		public int burstCount = 8;
	}
}
