using System;
using System.Collections.Generic;
using EntityStates.Interactables.GoldBeacon;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200021B RID: 539
	public class GoldshoresMissionController : MonoBehaviour
	{
		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06000BE5 RID: 3045 RVA: 0x00035606 File Offset: 0x00033806
		// (set) Token: 0x06000BE6 RID: 3046 RVA: 0x0003560D File Offset: 0x0003380D
		public static GoldshoresMissionController instance { get; private set; }

		// Token: 0x06000BE7 RID: 3047 RVA: 0x00035615 File Offset: 0x00033815
		private void OnEnable()
		{
			GoldshoresMissionController.instance = SingletonHelper.Assign<GoldshoresMissionController>(GoldshoresMissionController.instance, this);
		}

		// Token: 0x06000BE8 RID: 3048 RVA: 0x00035627 File Offset: 0x00033827
		private void OnDisable()
		{
			GoldshoresMissionController.instance = SingletonHelper.Unassign<GoldshoresMissionController>(GoldshoresMissionController.instance, this);
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06000BE9 RID: 3049 RVA: 0x00035639 File Offset: 0x00033839
		public int beaconsActive
		{
			get
			{
				return Ready.count;
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06000BEA RID: 3050 RVA: 0x00035640 File Offset: 0x00033840
		public int beaconCount
		{
			get
			{
				return Ready.count + NotReady.count;
			}
		}

		// Token: 0x06000BEB RID: 3051 RVA: 0x0003564D File Offset: 0x0003384D
		private void Start()
		{
			this.rng = new Xoroshiro128Plus((ulong)Run.instance.stageRng.nextUint);
			this.beginTransitionIntoBossFightEffect.SetActive(false);
			this.exitTransitionIntoBossFightEffect.SetActive(false);
		}

		// Token: 0x06000BEC RID: 3052 RVA: 0x00035684 File Offset: 0x00033884
		public void SpawnBeacons()
		{
			if (NetworkServer.active)
			{
				for (int i = 0; i < this.beaconsToSpawnOnMap; i++)
				{
					GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(this.beaconSpawnCard, new DirectorPlacementRule
					{
						placementMode = DirectorPlacementRule.PlacementMode.Random
					}, this.rng));
					if (gameObject)
					{
						this.beaconInstanceList.Add(gameObject);
					}
				}
				this.beaconsToSpawnOnMap = this.beaconInstanceList.Count;
			}
		}

		// Token: 0x06000BED RID: 3053 RVA: 0x000356F6 File Offset: 0x000338F6
		public void BeginTransitionIntoBossfight()
		{
			this.beginTransitionIntoBossFightEffect.SetActive(true);
			this.exitTransitionIntoBossFightEffect.SetActive(false);
		}

		// Token: 0x06000BEE RID: 3054 RVA: 0x00035710 File Offset: 0x00033910
		public void ExitTransitionIntoBossfight()
		{
			this.beginTransitionIntoBossFightEffect.SetActive(false);
			this.exitTransitionIntoBossFightEffect.SetActive(true);
		}

		// Token: 0x04000BF4 RID: 3060
		public Xoroshiro128Plus rng;

		// Token: 0x04000BF5 RID: 3061
		public EntityStateMachine entityStateMachine;

		// Token: 0x04000BF6 RID: 3062
		public GameObject beginTransitionIntoBossFightEffect;

		// Token: 0x04000BF7 RID: 3063
		public GameObject exitTransitionIntoBossFightEffect;

		// Token: 0x04000BF8 RID: 3064
		public Transform bossSpawnPosition;

		// Token: 0x04000BF9 RID: 3065
		public List<GameObject> beaconInstanceList = new List<GameObject>();

		// Token: 0x04000BFA RID: 3066
		public int beaconsRequiredToSpawnBoss;

		// Token: 0x04000BFB RID: 3067
		public int beaconsToSpawnOnMap;

		// Token: 0x04000BFC RID: 3068
		public InteractableSpawnCard beaconSpawnCard;
	}
}
