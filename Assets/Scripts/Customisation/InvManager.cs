using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        int dicePlacementAndMarge = 200;

        Debug.Log("Nombre de dés: " + DicesPanelContainer.transform.childCount);

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
    private void SetDices(Dices dice, int index){
        GameObject dicePrefab = Instantiate(DiceInvPrefab, DicesPanelContainer.transform);
        dicePrefab.GetComponent<DiceInv>().index = index;
        dicePrefab.GetComponent<DiceInv>().updateSlots(dice);
        dicePrefab.GetComponent<Button>().onClick.AddListener(() => selectDice(dicePrefab.GetComponent<DiceInv>().index));
    }


    private IEnumerator DelayedUpdateContainerSize(){
        yield return null; // Attendre la prochaine frame
        updateContainerSize(); // Mettre à jour la taille du conteneur
    }

    private void getAllData(){
        // Supprime les dés déjà présents dans le container
        for (int i = 0; i < DicesPanelContainer.transform.childCount; i++){
            if (DicesPanelContainer.transform.GetChild(i).name != "Buy New Dice"){
                Destroy(DicesPanelContainer.transform.GetChild(i).gameObject);
            }
        }

        GameData gameData = SaveSystem.LoadGameData();
        MoneyText.text = gameData.money.ToString() + " $";

        int count = 0;
        foreach (Dices dice in gameData.dices){
            SetDices(dice, count);
            count++;
        }

        // Chercher un élément nommé "Buy Next Dice" dans le container et le mettre à la fin du parent
        GameObject buyNextDice = DicesPanelContainer.transform.Find("Buy New Dice").gameObject;
        buyNextDice.transform.SetAsLastSibling();

        StartCoroutine(DelayedUpdateContainerSize()); // Appeler la coroutine pour mettre à jour la taille du conteneur
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
            slotImage.color = new Color(0.8431373f, 0.9254902f, 0.8980392f, 1);
        }
    }

    // Mettre a jour les listeners des boutons du shop
    private void updateShopListener(){
        // pour chaque bouton du shop, ajouter un listener
        for (int i = 0; i < ShopPanel.transform.childCount; i++)
        {
            Transform child = ShopPanel.transform.GetChild(i);
            child.GetComponent<Button>().onClick.RemoveAllListeners();
            child.GetComponent<Button>().onClick.AddListener(() => buySkill(child.name));
        }
    }

    private void updateAllVisuals(){
        getAllData();
        // updateContainerSize();
        updateShopListener();
        setNewDicePrice();
    }

    private int getDicePriceNewDice(){
        GameData gameData = SaveSystem.LoadGameData();
        return 1000 + gameData.dices.Count * 100;
    }

    private void setNewDicePrice(){
        GameData gameData = SaveSystem.LoadGameData();
        int price = getDicePriceNewDice();
        GameObject buyNextDice = DicesPanelContainer.transform.Find("Buy New Dice").gameObject;
        buyNextDice.transform.Find("Price").GetComponent<Text>().text = price.ToString() + " $";
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
        ShopPanel.SetActive(true);
        InstructionText.text = "Selectionnez une compétence -<\npour remplacer la face sélectionnée";
    }

    public void buySkill(string skillName){
        // find the gameObjet with the name skillName in the shopPanel
        Transform skill = ShopPanel.transform.Find(skillName);
        Transform price = skill.Find("Price");
        int priceValue = int.Parse(price.GetComponent<Text>().text.Replace(" ", "").Split('$')[0]);

        GameData gameData = SaveSystem.LoadGameData();
        if (gameData.money >= priceValue){
            gameData.money -= priceValue;
            // get the Dices indexSelectedDice from the gameData
            Dices dice = gameData.dices[indexSelectedDice];
            // get the face from the shopPanel
            string faceName = skillName;
            // set the face to the dice
            dice.SetItem(indexSelectedFace, faceName);
            // save the gameData
            SaveSystem.SaveGameData(gameData);
            indexSelectedFace = -1;
            updateAllVisuals();
            updatePatron();
            InstructionText.text = "Selectionnez une face ci-dessous pour la remplacer";
        }
    }




    public void buyNewDice(){
        GameData gameData = SaveSystem.LoadGameData();
        int price = getDicePriceNewDice();
        if (gameData.money >= price){
            gameData.money -= price;
            Dices defaultDice = new Dices();
            gameData.dices.Add(defaultDice);
            SaveSystem.SaveGameData(gameData);
            updateAllVisuals();
            updatePatron();
        }
    }


    public void backToHome(){
        SceneManager.LoadScene("Home");
    }

    // TODO:
    // - Listener du bouton "Buy New Dice"



}
