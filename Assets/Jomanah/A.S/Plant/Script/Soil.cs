using UnityEngine;
using UnityEngine.Playables;

public class Soil : MonoBehaviour
{
    public bool isEmpty = true;
    public bool playerInRange;
    public string plantName;
    private Plant currentPlant;

    private void Update()
    {
        // 🔸 معلق حالياً إلى أن تضيف سكربت PlayerState
        /*
        float distance = Vector3.Distance(PlayerState.Instance.playerBody.transform.position, transform.position);
        playerInRange = (distance < 10f);
        */
    }

    internal void PlantSeed()
    {
        // 🔸 معلق حالياً إلى أن تضيف EquipSystem و InventoryItem
        /*
        InventoryItem selectedSeed = EquipSystem.Instance.selectedItem.GetComponent<InventoryItem>();
        isEmpty = false;

        string onlyPlantName = selectedSeed.thisName.Split(new string[] { " Seed" }, System.StringSplitOptions.None)[0];
        plantName = onlyPlantName;

        GameObject instantiatedPlant = Instantiate(Resources.Load($"{onlyPlantName}Plant") as GameObject);

        // ✅ التعديل هنا: ما عاد نخليه Child للتربة عشان ما يتضخم
        instantiatedPlant.transform.SetParent(null);
        instantiatedPlant.transform.localPosition = Vector3.zero;

        currentPlant = instantiatedPlant.GetComponent<Plant>();
        currentPlant.dayOfPlanting = TimeManager.Instance.dayInGame;
        */
    }

    public void HarvestPlant()
    {
        if (currentPlant != null)
        {
            currentPlant.Harvest();

            if (currentPlant == null || currentPlant.gameObject == null)
            {
                isEmpty = true;
                plantName = "";
                currentPlant = null;
            }
            else
            {
                isEmpty = false;
            }

        }
    }
}
