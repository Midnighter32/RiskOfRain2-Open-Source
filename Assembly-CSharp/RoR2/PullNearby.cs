using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200039A RID: 922
	public class PullNearby : MonoBehaviour
	{
		// Token: 0x06001378 RID: 4984 RVA: 0x0005F157 File Offset: 0x0005D357
		private void Start()
		{
			this.teamFilter = base.GetComponent<TeamFilter>();
			if (this.pullOnStart)
			{
				this.InitializePull();
			}
		}

		// Token: 0x06001379 RID: 4985 RVA: 0x0005F173 File Offset: 0x0005D373
		private void FixedUpdate()
		{
			this.fixedAge += Time.fixedDeltaTime;
			if (this.fixedAge <= this.pullDuration)
			{
				this.UpdatePull(Time.fixedDeltaTime);
			}
		}

		// Token: 0x0600137A RID: 4986 RVA: 0x0005F1A0 File Offset: 0x0005D3A0
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

		// Token: 0x0600137B RID: 4987 RVA: 0x0005F274 File Offset: 0x0005D474
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

		// Token: 0x0600137C RID: 4988 RVA: 0x0005F334 File Offset: 0x0005D534
		private void AddToList(GameObject affectedObject)
		{
			CharacterBody component = affectedObject.GetComponent<CharacterBody>();
			if (!this.victimBodyList.Contains(component))
			{
				this.victimBodyList.Add(component);
			}
		}

		// Token: 0x04001725 RID: 5925
		public float pullRadius;

		// Token: 0x04001726 RID: 5926
		public float pullDuration;

		// Token: 0x04001727 RID: 5927
		public AnimationCurve pullStrengthCurve;

		// Token: 0x04001728 RID: 5928
		public bool pullOnStart;

		// Token: 0x04001729 RID: 5929
		public int maximumPullCount = int.MaxValue;

		// Token: 0x0400172A RID: 5930
		private List<CharacterBody> victimBodyList = new List<CharacterBody>();

		// Token: 0x0400172B RID: 5931
		private bool pulling;

		// Token: 0x0400172C RID: 5932
		private TeamFilter teamFilter;

		// Token: 0x0400172D RID: 5933
		private float fixedAge;
	}
}
