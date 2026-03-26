using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HouseInfoDisplay : MonoBehaviour
{
    public static HouseInfoDisplay Instance;

    [Header("House Info")]
    [SerializeField] private House house;
    [SerializeField] private string houseName;
    [SerializeField] private int housePrice;
    [SerializeField] private HouseSize houseSize;


    [Header("UI Objects")]
    public TextMeshProUGUI houseNameText;
    public TextMeshProUGUI housePriceText;
    public TextMeshProUGUI houseSizeText;
    public TextMeshProUGUI houseContentText;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // Suicide if Instance another game manager exists
            Destroy(this.gameObject);
        }
        else
        {
            // Otherwise, set this as the instance
            Instance = this;
        }
    }

    public void UpdateHouseInfo()
    {
        // Get house info
        house = GameManager.currentHome;
        houseName = house.houseName;
        housePrice = house.realPrice;
        houseSize = house.houseSize;

        // Apply house info to UI
        houseNameText.text = houseName;
        housePriceText.text = "$" + housePrice.ToString();
        houseSizeText.text = houseSize.ToString();
        switch (houseSize)
        {
            case(HouseSize.small):
                houseContentText.text = "1 Bedroom";
                break;
            case(HouseSize.medium):
                houseContentText.text = "2 Bedrooms";
                break;
            case(HouseSize.large):
                houseContentText.text = "3 Bedrooms";
                break;
        }
    }
}
