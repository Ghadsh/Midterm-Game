//using UnityEngine;
//using System.Collections.Generic;

//public class Plant : MonoBehaviour
//{
    //[SerializeField] GameObject seedMode1;
  //  [SerializeField] GameObject youngPlantMode1;
   // [SerializeField] GameObject maturePlantMode1;

    //[SerializeField] List<GameObject> PlantProduceSpawns;

    //[SerializeField] GameObject producePrefab;

    //public int dayOfPlanting; 
    //*
  //  [SerializeField] int plantAge = 0;
   // [SerializeField] int plantAge = 0;
   // [SerializeField] int ageForYoungMode1;
   // [SerializeField] int ageForMatureMode1;
   // [SerializeField] int ageForFirstProduceBatch;

   // [SerializeField] int dayForNewProduce;
   // [SerializeField] int daysRemainingForNewProduceCounter;

   // [SerializeField] bool isOneTimeHarvest;
    //[SerializeField] bool isWatered;

  //  public bool isReadyToHarvest = false;

   //private void OnEnable()
   // {
       // TimeManager.Instance.OnDayPass.AddListener(DayPass);
  //  }

   // private void OnDisable()
   // {
       // TimeManager.Instance.OnDayPass.RemoveListener(DayPass);
   // }

   // private void DayPass()
  //  {
        //if (isWatered)
       // {
          //  plantAge++;
        //}

      //  CheckGrowth();
      //  CheckProduce();
       // UpdateHarvestReady();
   // }

   // private void CheckGrowth()
   // {
       // seedMode1.SetActive(plantAge < ageForMatureMode1);
       // youngPlantMode1.SetActive(plantAge >= ageForYoungMode1 && plantAge < ageForMatureMode1);
       // maturePlantMode1.SetActive(plantAge >= ageForMatureMode1);
   // }

    //private void CheckProduce()
  //  {
        //if (plantAge == ageForFirstProduceBatch)
       // {
         //   GenerateProduceForEmptySpawns();
      //  }
      //  if (plantAge > ageForFirstProduceBatch)
       // {
            //if (daysRemainingForNewProduceCounter == 0)
           // {
             //   GenerateProduceForEmptySpawns();
             //   daysRemainingForNewProduceCounter = dayForNewProduce;
          //  }
           // else
            //{
              //  daysRemainingForNewProduceCounter--;
           // }
      //  }
   // }

 //   private void GenerateProduceForEmptySpawns()
   // {
      //  foreach (GameObject spawn in PlantProduceSpawns)
       // {
           // if (spawn.transform.childCount == 0)
           // {
              //  GameObject produce = Instantiate(producePrefab);
              //  produce.transform.parent = spawn.transform;

              //  Vector3 producePosition = Vector3.zero;
               // producePosition.y = 0f;
              //  produce.transform.localPosition = producePosition;
           // }
       // }
    //}

   //// {
       // isReadyToHarvest = false;
       // foreach (GameObject spawn in PlantProduceSpawns)
        //{
           //if (spawn.transform.childCount > 0)
           // {
               // isReadyToHarvest = true;
              //  break;
          //  }
     //   }
   // }

  //  public void Harvest()
   // {
       // foreach (GameObject spawn in PlantProduceSpawns)
      //  {
         //  foreach (Transform child in spawn.transform)
           // {
           //  //   Destroy(child.gameObject);
           // }
     //   }

      //  if (isOneTimeHarvest)
     //   {
         //   Destroy(gameObject);
      //  }
     //   else
     //   {
          //  daysRemainingForNewProduceCounter = dayForNewProduce;
     //     //  isReadyToHarvest = false;
       // }
  //  }
///}*//*

