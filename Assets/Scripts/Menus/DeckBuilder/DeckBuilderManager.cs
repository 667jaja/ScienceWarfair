using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeckBuilderManager : MonoBehaviour
{
    public static DeckBuilderManager instance;

    [SerializeField] private List<PlayerData> savedPlayers;

    //LoadButtons
    [SerializeField] private string levelSelectName;
    [SerializeField] private string mainMenuName;
    [SerializeField] private List<CardData> defaultDeck;

    //Card list
    public List<CardData> allCards;
    public CardVisual cardVisual; 
    public GameObject cardParent;

    //Decks
    [SerializeField] private int deckMinimum = 20;
    public bool deckSwitched;
    [SerializeField] private string deck1Name;
    public GameObject deck1Slider;
    public GameObject deck1Contents;
    [SerializeField] private string deck2Name;
    public GameObject deck2Slider;
    public GameObject deck2Contents;
    [SerializeField] private TextMeshProUGUI deckTitle, deckCardCount;
    [SerializeField] private List<Collider2D> dropAreas;
    
    //deck Spacing 
    [SerializeField] private float allCardsSpacers = 480;
    [SerializeField] private float allCardsCap = 260;
    [SerializeField] private float deckSpacers = 480;
    [SerializeField] private float deckCap = 2700;

//    [SerializeField] private List<Transform> Bounds;


//    public List<Card> AllOwnedCards;
    void Awake()
    {
        instance = this;

        float thingX = allCards.Count*allCardsSpacers+allCardsCap;
        cardParent.GetComponent<RectTransform>().offsetMax = new Vector2 (thingX, cardParent.GetComponent<RectTransform>().offsetMax.y);

    }
    void Start()
    {
        deck1Name = (savedPlayers[0].playerName != null && savedPlayers[0].playerName.Length >= 1)? savedPlayers[0].playerName + " Deck" : "Player1 Deck";
        deck2Name = (savedPlayers[1].playerName != null && savedPlayers[1].playerName.Length >= 1)? savedPlayers[1].playerName + " Deck"  : "Player2 Deck";

        deckTitle.text = deck1Name;
        UpdateUI();
    }
    public void LoadLevelSelect()
    {
        SceneManager.LoadSceneAsync(levelSelectName);
        foreach (PlayerData player in savedPlayers)
        {
            if (player.deck.Count < deckMinimum)
            {
                player.deck = defaultDeck;
            }
        }
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync(mainMenuName);
    }
    
    public void ClearDeck()
    {
        Debug.Log("Clear Deck");

        if (deckSwitched)
        {
            foreach(Transform transform in deck2Contents.transform)
            {
                Destroy(transform.gameObject);
            } 
            savedPlayers[1].deck = new List<CardData>();
        }
        else
        {
            foreach(Transform transform in deck1Contents.transform)
            {
                Destroy(transform.gameObject);
            } 
            savedPlayers[0].deck = new List<CardData>();
        }
        
        SetDeckCountText();
        RefreshDeckScroll();
    }
    public void RandomDeck()
    {
        Debug.Log("Random Deck");

        int MinCurrentDif;
        int currentPlayer = deckSwitched? 1 : 0;


        MinCurrentDif = deckMinimum - savedPlayers[currentPlayer].deck.Count;

        for (int i = 0; i < MinCurrentDif;)
        {
            i++;
            int gen = UnityEngine.Random.Range(0, allCards.Count);

            savedPlayers[currentPlayer].deck.Add(allCards[gen]);
        }

        Debug.Log("Added " + MinCurrentDif + "Cards");


        UpdateUI();
    }
    public void SwitchDeck()
    {
        if (deckSwitched)
        {
            deckTitle.text = deck1Name;
            deckSwitched = false;
        }
        else
        {
            deckTitle.text = deck2Name;
            deckSwitched = true;
        }

        UpdateUI();
    }

    public void AddCardToCurrentDeck(CardData addedCard)
    {
        if (deckSwitched)
        {
            savedPlayers[1].deck.Add(addedCard);
        }
        else 
        {
            savedPlayers[0].deck.Add(addedCard);
        }

        UpdateUI();
    }
    public void RemoveCardFromCurrentDeck(CardData removedCard)
    {
        if (deckSwitched)
        {
            savedPlayers[1].deck.Remove(removedCard);
        }
        else
        {
            savedPlayers[0].deck.Remove(removedCard);
        }
        UpdateUI();
    }
    public void CardSelectedToggle(bool cardIsSelected)
    {
        if (cardIsSelected)
        {
            foreach (Collider2D col in dropAreas)
            {
                col.enabled = true;
            }
        }
        else
        {
            foreach (Collider2D col in dropAreas)
            {
                col.enabled = false;
            }
        }
    }
    public void UpdateUI()
    {
        SetDeckCountText();
        RefreshDeckScroll();
        UpdateDeckVisuals();
    }
    public void SetDeckCountText()
    {
        if (deckSwitched)
        {
            deckCardCount.text = ""+ savedPlayers[1].deck.Count;
        }
        else
        {
            deckCardCount.text = ""+ savedPlayers[0].deck.Count;
        }
    }
    public void RefreshDeckScroll()
    {
        if (!deckSwitched)
        {
            deck1Contents.SetActive(true);
            deck1Slider.SetActive(true);
            deck2Contents.SetActive(false);
            deck2Slider.SetActive(false);

            float thingX = savedPlayers[0].deck.Count*deckSpacers+deckCap;
            //if (thingX < 2700) thingX = 4800;

            deck1Contents.GetComponent<RectTransform>().offsetMax = new Vector2 (thingX, deck1Contents.GetComponent<RectTransform>().offsetMax.y);
        }
        else
        {
            deck2Contents.SetActive(true);
            deck2Slider.SetActive(true);
            deck1Contents.SetActive(false);
            deck1Slider.SetActive(false);

            float thingX = savedPlayers[1].deck.Count*deckSpacers+deckCap;
            //if (thingX < 2700) thingX = 4800;

            deck2Contents.GetComponent<RectTransform>().offsetMax = new Vector2 (thingX, deck2Contents.GetComponent<RectTransform>().offsetMax.y);

        }
    }
    public void UpdateDeckVisuals()
    {
        foreach(Transform item in cardParent.transform)
        {
            Destroy(item.gameObject);
        }
        
        foreach(Transform item in deck1Contents.transform)
        {
            Destroy(item.gameObject);
        }

        foreach(Transform item in deck2Contents.transform)
        {
            Destroy(item.gameObject);
        }

        foreach(CardData item in allCards)
        {
            CardVisual newCardVisual = Instantiate(cardVisual, cardParent.transform);
            Card newCard = new Card(item);
            newCard.isInADeck = false;
            newCardVisual.Initiate(newCard);
        }
        
        foreach(CardData item in savedPlayers[0].deck)
        {
            CardVisual newCardVisual = Instantiate(cardVisual, deck1Contents.transform);
            Card newCard = new Card(item);
            newCard.isInADeck = true;
            newCardVisual.Initiate(newCard);
        }

        foreach(CardData item in savedPlayers[1].deck)
        {
            CardVisual newCardVisual = Instantiate(cardVisual, deck2Contents.transform);
            Card newCard = new Card(item);
            newCard.isInADeck = true;
            newCardVisual.Initiate(newCard);
        }
    } 
}
