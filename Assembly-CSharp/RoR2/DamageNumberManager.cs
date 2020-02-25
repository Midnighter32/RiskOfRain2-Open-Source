using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001CF RID: 463
	[ExecuteAlways]
	public class DamageNumberManager : MonoBehaviour
	{
		// Token: 0x17000144 RID: 324
		// (get) Token: 0x060009EB RID: 2539 RVA: 0x0002B567 File Offset: 0x00029767
		// (set) Token: 0x060009EC RID: 2540 RVA: 0x0002B56E File Offset: 0x0002976E
		public static DamageNumberManager instance { get; private set; }

		// Token: 0x060009ED RID: 2541 RVA: 0x0002B576 File Offset: 0x00029776
		private void OnEnable()
		{
			DamageNumberManager.instance = SingletonHelper.Assign<DamageNumberManager>(DamageNumberManager.instance, this);
		}

		// Token: 0x060009EE RID: 2542 RVA: 0x0002B588 File Offset: 0x00029788
		private void OnDisable()
		{
			DamageNumberManager.instance = SingletonHelper.Unassign<DamageNumberManager>(DamageNumberManager.instance, this);
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x0002B59A File Offset: 0x0002979A
		private void Awake()
		{
			this.ps = base.GetComponent<ParticleSystem>();
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x0000409B File Offset: 0x0000229B
		private void Update()
		{
		}

		// Token: 0x060009F1 RID: 2545 RVA: 0x0002B5A8 File Offset: 0x000297A8
		public void SpawnDamageNumber(float amount, Vector3 position, bool crit, TeamIndex teamIndex, DamageColorIndex damageColorIndex)
		{
			Color a = DamageColor.FindColor(damageColorIndex);
			Color b = Color.white;
			if (teamIndex != TeamIndex.None)
			{
				if (teamIndex == TeamIndex.Monster)
				{
					b = new Color(0.5568628f, 0.29411766f, 0.6039216f);
				}
			}
			else
			{
				b = Color.gray;
			}
			this.ps.Emit(new ParticleSystem.EmitParams
			{
				position = position,
				startColor = a * b,
				applyShapeToPosition = true
			}, 1);
			this.ps.GetCustomParticleData(this.customData, ParticleSystemCustomData.Custom1);
			this.customData[this.customData.Count - 1] = new Vector4(1f, 0f, amount, crit ? 1f : 0f);
			this.ps.SetCustomParticleData(this.customData, ParticleSystemCustomData.Custom1);
		}

		// Token: 0x04000A24 RID: 2596
		private List<Vector4> customData = new List<Vector4>();

		// Token: 0x04000A25 RID: 2597
		private ParticleSystem ps;
	}
}
