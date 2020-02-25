using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002D8 RID: 728
	public class PullNearby : MonoBehaviour
	{
		// Token: 0x0600109D RID: 4253 RVA: 0x00048DD4 File Offset: 0x00046FD4
		private void Start()
		{
			this.teamFilter = base.GetComponent<TeamFilter>();
			if (this.pullOnStart)
			{
				this.InitializePull();
			}
		}

		// Token: 0x0600109E RID: 4254 RVA: 0x00048DF0 File Offset: 0x00046FF0
		private void FixedUpdate()
		{
			this.fixedAge += Time.fixedDeltaTime;
			if (this.fixedAge <= this.pullDuration)
			{
				this.UpdatePull(Time.fixedDeltaTime);
			}
		}

		// Token: 0x0600109F RID: 4255 RVA: 0x00048E20 File Offset: 0x00047020
		private void UpdatePull(float deltaTime)
		{
			if (!this.pulling)
			{
				return;
			}
			for (int i = 0; i < this.victimBodyList.Count; i++)
			{
				CharacterBody characterBody = this.victimBodyList[i];
				Vector3 vector = base.transform.position - characterBody.corePosition;
				float d = this.pullStrengthCurve.Evaluate(vector.magnitude / this.pullRadius);
				Vector3 b = vector.normalized * d * deltaTime;
				CharacterMotor component = characterBody.GetComponent<CharacterMotor>();
				if (component)
				{
					component.rootMotion += b;
				}
				else
				{
					Rigidbody component2 = characterBody.GetComponent<Rigidbody>();
					if (component2)
					{
						component2.velocity += b;
					}
				}
			}
		}

		// Token: 0x060010A0 RID: 4256 RVA: 0x00048EF4 File Offset: 0x000470F4
		public void InitializePull()
		{
			if (this.pulling)
			{
				return;
			}
			this.pulling = true;
			Collider[] array = Physics.OverlapSphere(base.transform.position, this.pullRadius, LayerIndex.defaultLayer.mask);
			int num = 0;
			int num2 = 0;
			while (num < array.Length && num2 < this.maximumPullCount)
			{
				HealthComponent component = array[num].GetComponent<HealthComponent>();
				if (component)
				{
					TeamComponent component2 = component.GetComponent<TeamComponent>();
					bool flag = false;
					if (component2 && this.teamFilter)
					{
						flag = (component2.teamIndex == this.teamFilter.teamIndex);
					}
					if (!flag)
					{
						this.AddToList(component.gameObject);
						num2++;
					}
				}
				num++;
			}
		}

		// Token: 0x060010A1 RID: 4257 RVA: 0x00048FB4 File Offset: 0x000471B4
		private void AddToList(GameObject affectedObject)
		{
			CharacterBody component = affectedObject.GetComponent<CharacterBody>();
			if (!this.victimBodyList.Contains(component))
			{
				this.victimBodyList.Add(component);
			}
		}

		// Token: 0x04000FFA RID: 4090
		public float pullRadius;

		// Token: 0x04000FFB RID: 4091
		public float pullDuration;

		// Token: 0x04000FFC RID: 4092
		public AnimationCurve pullStrengthCurve;

		// Token: 0x04000FFD RID: 4093
		public bool pullOnStart;

		// Token: 0x04000FFE RID: 4094
		public int maximumPullCount = int.MaxValue;

		// Token: 0x04000FFF RID: 4095
		private List<CharacterBody> victimBodyList = new List<CharacterBody>();

		// Token: 0x04001000 RID: 4096
		private bool pulling;

		// Token: 0x04001001 RID: 4097
		private TeamFilter teamFilter;

		// Token: 0x04001002 RID: 4098
		private float fixedAge;
	}
}
