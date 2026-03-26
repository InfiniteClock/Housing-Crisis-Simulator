using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HouseInfoDisplay : MonoBehaviour
{
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
    
}
