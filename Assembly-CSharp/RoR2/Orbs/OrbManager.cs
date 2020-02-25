using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x020004D7 RID: 1239
	public class OrbManager : MonoBehaviour
	{
		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06001D9F RID: 7583 RVA: 0x0007E7AF File Offset: 0x0007C9AF
		// (set) Token: 0x06001DA0 RID: 7584 RVA: 0x0007E7B6 File Offset: 0x0007C9B6
		public static OrbManager instance { get; private set; }

		// Token: 0x06001DA1 RID: 7585 RVA: 0x0007E7BE File Offset: 0x0007C9BE
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

		// Token: 0x06001DA2 RID: 7586 RVA: 0x0007E7F2 File Offset: 0x0007C9F2
		private void OnDisable()
		{
			if (OrbManager.instance == this)
			{
				OrbManager.instance = null;
			}
		}

		// Token: 0x06001DA3 RID: 7587 RVA: 0x0007E808 File Offset: 0x0007CA08
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

		// Token: 0x06001DA4 RID: 7588 RVA: 0x0007E8E4 File Offset: 0x0007CAE4
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

		// Token: 0x04001ADA RID: 6874
		private float time;

		// Token: 0x04001ADB RID: 6875
		private List<Orb> travelingOrbs = new List<Orb>();

		// Token: 0x04001ADC RID: 6876
		private float nextOrbArrival = float.PositiveInfinity;

		// Token: 0x04001ADD RID: 6877
		private List<IOrbFixedUpdateBehavior> orbsWithFixedUpdateBehavior = new List<IOrbFixedUpdateBehavior>();
	}
}
