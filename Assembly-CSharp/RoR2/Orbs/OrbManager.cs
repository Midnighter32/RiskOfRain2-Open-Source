using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x0200051D RID: 1309
	public class OrbManager : MonoBehaviour
	{
		// Token: 0x1700029B RID: 667
		// (get) Token: 0x06001D6E RID: 7534 RVA: 0x00089472 File Offset: 0x00087672
		// (set) Token: 0x06001D6F RID: 7535 RVA: 0x00089479 File Offset: 0x00087679
		public static OrbManager instance { get; private set; }

		// Token: 0x06001D70 RID: 7536 RVA: 0x00089481 File Offset: 0x00087681
		private void OnEnable()
		{
			if (!OrbManager.instance)
			{
				OrbManager.instance = this;
				return;
			}
			Debug.LogErrorFormat(this, "Duplicate instance of singleton class {0}. Only one should exist at a time.", new object[]
			{
				base.GetType().Name
			});
		}

		// Token: 0x06001D71 RID: 7537 RVA: 0x000894B5 File Offset: 0x000876B5
		private void OnDisable()
		{
			if (OrbManager.instance == this)
			{
				OrbManager.instance = null;
			}
		}

		// Token: 0x06001D72 RID: 7538 RVA: 0x000894CC File Offset: 0x000876CC
		private void FixedUpdate()
		{
			this.time += Time.fixedDeltaTime;
			for (int i = 0; i < this.orbsWithFixedUpdateBehavior.Count; i++)
			{
				this.orbsWithFixedUpdateBehavior[i].FixedUpdate();
			}
			if (this.nextOrbArrival <= this.time)
			{
				this.nextOrbArrival = float.PositiveInfinity;
				for (int j = this.travelingOrbs.Count - 1; j >= 0; j--)
				{
					Orb orb = this.travelingOrbs[j];
					if (orb.arrivalTime <= this.time)
					{
						this.travelingOrbs.RemoveAt(j);
						IOrbFixedUpdateBehavior orbFixedUpdateBehavior = orb as IOrbFixedUpdateBehavior;
						if (orbFixedUpdateBehavior != null)
						{
							this.orbsWithFixedUpdateBehavior.Remove(orbFixedUpdateBehavior);
						}
						orb.OnArrival();
					}
					else if (this.nextOrbArrival > orb.arrivalTime)
					{
						this.nextOrbArrival = orb.arrivalTime;
					}
				}
			}
		}

		// Token: 0x06001D73 RID: 7539 RVA: 0x000895A8 File Offset: 0x000877A8
		public void AddOrb(Orb orb)
		{
			orb.Begin();
			orb.arrivalTime = this.time + orb.duration;
			this.travelingOrbs.Add(orb);
			IOrbFixedUpdateBehavior orbFixedUpdateBehavior = orb as IOrbFixedUpdateBehavior;
			if (orbFixedUpdateBehavior != null)
			{
				this.orbsWithFixedUpdateBehavior.Add(orbFixedUpdateBehavior);
			}
			if (this.nextOrbArrival > orb.arrivalTime)
			{
				this.nextOrbArrival = orb.arrivalTime;
			}
		}

		// Token: 0x04001FC1 RID: 8129
		private float time;

		// Token: 0x04001FC2 RID: 8130
		private List<Orb> travelingOrbs = new List<Orb>();

		// Token: 0x04001FC3 RID: 8131
		private float nextOrbArrival = float.PositiveInfinity;

		// Token: 0x04001FC4 RID: 8132
		private List<IOrbFixedUpdateBehavior> orbsWithFixedUpdateBehavior = new List<IOrbFixedUpdateBehavior>();
	}
}
