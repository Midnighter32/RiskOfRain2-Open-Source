using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000303 RID: 771
	public class GoldshoresMissionController : MonoBehaviour
	{
		// Token: 0x17000153 RID: 339
		// (get) Token: 0x06000FDA RID: 4058 RVA: 0x0004F809 File Offset: 0x0004DA09
		// (set) Token: 0x06000FDB RID: 4059 RVA: 0x0004F810 File Offset: 0x0004DA10
		public static GoldshoresMissionController instance { get; private set; }

		// Token: 0x06000FDC RID: 4060 RVA: 0x0004F818 File Offset: 0x0004DA18
		private void OnEnable()
		{
			GoldshoresMissionController.instance = SingletonHelper.Assign<GoldshoresMissionController>(GoldshoresMissionController.instance, this);
		}

		// Token: 0x06000FDD RID: 4061 RVA: 0x0004F82A File Offset: 0x0004DA2A
		private void OnDisable()
		{
			GoldshoresMissionController.instance = SingletonHelper.Unassign<GoldshoresMissionController>(GoldshoresMissionController.instance, this);
		}

		// Token: 0x06000FDE RID: 4062 RVA: 0x0004F83C File Offset: 0x0004DA3C
		private void Start()
		{
			this.rng = new Xoroshiro128Plus((ulong)Run.instance.stageRng.nextUint);
			this.beginTransitionIntoBossFightEffect.SetActive(false);
			this.exitTransitionIntoBossFightEffect.SetActive(false);
		}

		// Token: 0x06000FDF RID: 4063 RVA: 0x0004F874 File Offset: 0x0004DA74
		public void SpawnBeacons()
		{
			for (int i = 0; i < this.beaconsToSpawnOnMap; i++)
			{
				GameObject gameObject = DirectorCore.instance.TrySpawnObject(this.beaconSpawnCard, new DirectorPlacementRule
				{
					placementMode = DirectorPlacementRule.PlacementMode.Random
				}, this.rng);
				if (gameObject)
				{
					this.beaconInstanceList.Add(gameObject);
				}
			}
			this.beaconsToSpawnOnMap = this.beaconInstanceList.Count;
		}

		// Token: 0x06000FE0 RID: 4064 RVA: 0x0004F8DA File Offset: 0x0004DADA
		public void BeginTransitionIntoBossfight()
		{
			this.beginTransitionIntoBossFightEffect.SetActive(true);
			this.exitTransitionIntoBossFightEffect.SetActive(false);
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x0004F8F4 File Offset: 0x0004DAF4
		public void ExitTransitionIntoBossfight()
		{
			this.beginTransitionIntoBossFightEffect.SetActive(false);
			this.exitTransitionIntoBossFightEffect.SetActive(true);
		}

		// Token: 0x040013DD RID: 5085
		public Xoroshiro128Plus rng;

		// Token: 0x040013DE RID: 5086
		public EntityStateMachine entityStateMachine;

		// Token: 0x040013DF RID: 5087
		public GameObject beginTransitionIntoBossFightEffect;

		// Token: 0x040013E0 RID: 5088
		public GameObject exitTransitionIntoBossFightEffect;

		// Token: 0x040013E1 RID: 5089
		public Transform bossSpawnPosition;

		// Token: 0x040013E2 RID: 5090
		public List<GameObject> beaconInstanceList = new List<GameObject>();

		// Token: 0x040013E3 RID: 5091
		public int beaconsActive;

		// Token: 0x040013E4 RID: 5092
		public int beaconsRequiredToSpawnBoss;

		// Token: 0x040013E5 RID: 5093
		public int beaconsToSpawnOnMap;

		// Token: 0x040013E6 RID: 5094
		public InteractableSpawnCard beaconSpawnCard;
	}
}
