using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvManager : MonoBehaviour
{

    [SerializeField] private GameObject DicesPanelContainer;
    [SerializeField] private GameObject ShopPanel;
    [SerializeField] private GameObject Patron;
    [SerializeField] private Text MoneyText;
    [SerializeField] private Text InstructionText;
    
    [SerializeField] private GameObject DiceInvPrefab;
    private int indexSelectedDice = -1;
    private int indexSelectedFace = -1;


    // Ajoute replace les dés dans le container et met à jour la taille du container
    public void updateContainerSize(){
        int initialMargin = 150;
        int dicePlacementAndMarge = 250;
        // pour chaque enfant du container
        for (int i = 0; i < DicesPanelContainer.transform.childCount; i++)
        {
            Transform child = DicesPanelContainer.transform.GetChild(i);
            RectTransform childRectTransform = child.GetComponent<RectTransform>();
            childRectTransform.anchoredPosition = new Vector2(0, - initialMargin - dicePlacementAndMarge * i);
        }
        DicesPanelContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, initialMargin + dicePlacementAndMarge * DicesPanelContainer.transform.childCount);
    }


    // Génère les prefab des dés dans le container
    private void SetDices(Dices dice){
        GameObject dicePrefab = Instantiate(DiceInvPrefab, DicesPanelContainer.transform);
        dicePrefab.GetComponent<DiceInv>().index = DicesPanelContainer.transform.childCount - 2;
        dicePrefab.GetComponent<DiceInv>().updateSlots(dice);
        dicePrefab.GetComponent<Button>().onClick.AddListener(() => selectDice(dicePrefab.GetComponent<DiceInv>().index));
    }


    // Récupère tous les dés du joueur depuis les données sauvegardées
    private void getAllData(){
        GameData gameData = SaveSystem.LoadGameData();
        MoneyText.text = gameData.money.ToString();

        foreach (Dices dice in gameData.dices)
        {
            SetDices(dice);
        }
        // Chercher un élément nommé "Buy Next Dice" dans le container et le mettre à la fin du parent
        GameObject buyNextDice = DicesPanelContainer.transform.Find("Buy New Dice").gameObject;
        buyNextDice.transform.SetAsLastSibling();
    }

    // Appelé à chaque fois que le joueur clique sur un dé dans la liste
    // Met à jour le patron
    private void updatePatron(){
        // get the Dices[indexSelectedDice] from the gameData
        GameData gameData = SaveSystem.LoadGameData();
        Dices dice = gameData.dices[indexSelectedDice];
        Patron.GetComponent<DiceInv>().updateSlots(dice);
        // pour chaque face du patron, ajouter un listener
        for (int i = 0; i < Patron.transform.childCount; i++)
        {
            int faceIndex = i; // Crée une copie de la variable i à ce stade
            Transform child = Patron.transform.GetChild(i);

            // Supprimer les listeners précédents
            child.GetComponent<Button>().onClick.RemoveAllListeners();
            child.GetComponent<Button>().onClick.AddListener(() => selectFace(faceIndex));
            
            Image slotImage = child.GetComponent<Image>();
            slotImage.color = new Color(1, 1, 1, 1);
        }
    }


    private void updateAllVisuals(){
        getAllData();
        updateContainerSize();
    }

    private void Start() {
        updateAllVisuals();
    }


    private void Update(){
        if (indexSelectedDice == -1 || indexSelectedFace == -1){
            ShopPanel.SetActive(false);
        }
        if (indexSelectedDice == -1){
            Patron.SetActive(false);
        }
    }




    // Listener de boutons 
    public void selectDice(int index){
        indexSelectedFace = -1;
        indexSelectedDice = index;
        Patron.SetActive(true);
        updatePatron();
        InstructionText.text = "Selectionnez une face ci-dessous pour la remplacer";
    }

    public void selectFace(int index){
        indexSelectedFace = index;
        Debug.Log("Selected face: " + index);
        updatePatron();
        // change the color of the selected face
        Transform slot = Patron.transform.GetChild(index);
        Image slotImage = slot.GetComponent<Image>();
        slotImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
    }



    // TODO:
    // - Listener du bouton "Buy New Dice"
    // - Unity - ShopPanel
    // - Remplacement de la face via le shop



}
