using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using Photon.Pun; // INFO-GAMER 
using System.IO;  // INFO-GAMER 

public class DeckOfCards : MonoBehaviour
{
    #region variables
    int capsuleOffset = 0;  // INFO-GAMER  
    public Text MessageText;
    public Text[] OppMessageText = new Text[4];
    public Text ScoreText;

    public CardInDeck thisCard;
    CardInDeck[] tempHand;
    public CardInDeck[] FullDeck;
    CardInDeck[] tempDeck;
    CardInDeck[] afterDraw;
    CardInDeck[] emptyHand;
    public Player[] players;
    public OppArea[] oppArea;
    public CardInDeck[] gameBoard;  // name ??

    public GameObject cardImagePrefab;
    public GameObject playedImagePrefab;
    public GameObject yourHand;
    public GameObject newWordArea;
    public GameObject[] discardPiles;
    public GameObject[] tabletop;
    public GameObject[][] oppWords = new GameObject[4][];
    // not used ??    public string[] dummyArray = new string[4];
    public int[] cardPositions = new int[3];
    public string cardInfoText;
    public static DeckOfCards deckOfCards;

    IDictionary<string, int> ImageIndex = new Dictionary<string, int>();
    public Dictionary<string, string> words3 = new Dictionary<string, string>();
    string fs;
    public string[] fLines;
    string[] keypair;
    string firstFound;
    List<int> searchedPos = new List<int>();
    List<int> searchedBlack = new List<int>();
    List<int> searchedBlue = new List<int>();
    List<int> searchedGreen = new List<int>();
    List<int> searchedRed = new List<int>();

    int[] ptr = new int[3];
    // delete ??     int[] posArr = new int[3];
    int oppNbr;
    string[] playerName = new string[] { "You", "#1Bot", "#2Bot", "#3Bot" };
    string endTurnMessage;
    int deckIndex;
    int currentCardIndex;
    public int currentPlayerIndex;
    public int handSize = 12;
    public static string currentColor;      // Will this work for multi Player ??

    public Button DrawButton;
    public Button AddButton;
    public Button PlayButton;
    public Button ResetButton;
    public Button EndButton;
    public Button EndOfRoundButton;

    public static bool lettersAdded;

    bool canEnd;   // remove  ?
    bool hasChosen;
    public Transform middlePileEmpty;
    public CardInDeck chosenCard;
    // not used ??    public List<CardInDeck> cardList;

    public Sprite[] cardImages;

    public static IDictionary<string, int> LetVal = new Dictionary<string, int>();
    public static int[] playerScore = new int[4];
    public static int addedValue = 0;
    int turnCount = 0;
    int wordCount = 0;
    public int[] oppWordCount = new int[] { 0, 0, 0, 0 };

    // ??    public GameObject DeckButton;

    public TextAsset textFile;

    List<string> occurs_2X = new List<string> { "A", "I", "O", "U", "L", "N", "R", "S", "T" };
    List<string> occurs_3X = new List<string> { "E" };

    #endregion variables

    private void Awake()
    {
        deckOfCards = this;

        textFile = (TextAsset)Resources.Load(("words3.501"), typeof(TextAsset));

        players = new Player[4];
        for (int i = 0; i < 4; i++) playerScore[i] = 0;



        for (int i = 0; i < players.Length; i++)
        {
            Player p = new Player()
            {
                playerList = new List<CardInDeck>(),
                playerHand = new CardInDeck[0],
                index = i
            };
            players[i] = p;
        }


        BuildFullDeck();

        Shuffle();

        LetVal["A"] = LetVal["E"] = LetVal["I"] = LetVal["O"] = LetVal["U"] = 1;
        LetVal["D"] = LetVal["G"] = LetVal["L"] = LetVal["N"] = LetVal["R"] = LetVal["S"] = LetVal["T"] = 2;
        LetVal["B"] = LetVal["C"] = LetVal["F"] = LetVal["H"] = LetVal["M"] = LetVal["P"] = LetVal["W"] = LetVal["Y"] = 3;
        LetVal["J"] = LetVal["K"] = LetVal["Q"] = LetVal["V"] = LetVal["X"] = LetVal["Z"] = 4;
        string[] Alphabet = new string[] {
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
        };

        DealHands();
    }


    void Start()
    {
        CreateCapsule(); // INFO-GAMER Create a networked player object for each player that loads into the multiplayer scenes.
            

    }

    private void CreateCapsule()  // INFO-GAMER 
    {
        capsuleOffset++;
        Debug.Log("Creating prefab Capsule    capsuleOffset" + capsuleOffset);
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), Vector3.right * capsuleOffset, Quaternion.identity);
    }
        private void BuildFullDeck()
    {
        FullDeck = new CardInDeck[99];
        gameBoard = new CardInDeck[0];

        deckIndex = 0;
        foreach (string colorx in Enum.GetNames(typeof(CardInDeck.CommonColors)))
        {
            foreach (string letterx in Enum.GetNames(typeof(CardInDeck.CommonLetters)))
            {
                CardInDeck tempCard = new CardInDeck();
                tempCard.GLetter = letterx;
                tempCard.GColor = colorx;
                FullDeck[deckIndex++] = tempCard;
                /* 
                 * Add copies of letters that occur more than once in the deck
                */
                if (occurs_2X.Contains(letterx))
                {
                    FullDeck[deckIndex++] = tempCard;
                }
                else if (occurs_3X.Contains(letterx))
                {
                    FullDeck[deckIndex++] = tempCard;
                    FullDeck[deckIndex++] = tempCard;
                }
            }

        }
        foreach (string colorx in Enum.GetNames(typeof(CardInDeck.RareColors)))
        {
            foreach (string letterx in Enum.GetNames(typeof(CardInDeck.RareLetters)))
            {
                CardInDeck tempCard = new CardInDeck();
                tempCard.GLetter = letterx;
                tempCard.GColor = colorx;
                FullDeck[deckIndex++] = tempCard;
            //    Debug.Log("deckIndex=" + deckIndex + "   tempCard=" + tempCard.GColor + " " + tempCard.GLetter);
            }
        }
    }

    private void DealHands()
    {
        for (int i = 0; i < players.Length; i++)
        {
            DealMethod2(players[i]);
        }

        if (playerScore[0] == 0)
        {
            for (int j = 0; j < handSize; j++) PlayCard(players[0], 0);   // Deal first hand to the active player
        }
        else
        {
            for (int j = 0; j < handSize; j++) PlayCard(players[0], 0);   // Deal first hand to the active player
        }
        AddButton.enabled = false;
        CheckEndButton();
    }




    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (hasChosen)
            {
                PlayCard(players[currentPlayerIndex], currentCardIndex);
                EndTurn();
            }
            else
            {
                Debug.Log("You need to choose a card");   //  Change to UI message
            }
        }

        if (newWordArea.transform.childCount >= 3)
        {
            PlayButton.enabled = true;
        }
        else
        {
            PlayButton.enabled = false;
        }

        if (newWordArea.transform.childCount > 0)
        {
            ResetButton.enabled = true;
        }
        else
        {
            ResetButton.enabled = false;
        }

    }
    public void EnableAddButton()
    {
        {
            Debug.Log("start of EnableAddButton script");
            AddButton.enabled = true;
        }

    }
    public void EnablePlayButton()
    {
        Debug.Log("start of EnablePlayButton script");
        if (newWordArea.transform.childCount >= 3)
        {
            PlayButton.enabled = true;
        }
        else
        {
            PlayButton.enabled = false;
        }

        if (newWordArea.transform.childCount > 0)
        {
            ResetButton.enabled = true;
        }
        else
        {
            ResetButton.enabled = false;
        }
    }

    public void Shuffle()
    {
        int swap;
        CardInDeck c;

        for (int i = FullDeck.Length - 1; i > 0; i--)
        {
            swap = UnityEngine.Random.Range(0, i);

            c = FullDeck[swap];
            FullDeck[swap] = FullDeck[i];
            FullDeck[i] = c;
        }
    }
    public void DealMethod2(Player p)
    {
        //       Debug.Log("p.playerHand.Length = " + p.playerHand.Length);
        //       Debug.Log("p.playerList.Count  = " + p.playerList.Count);
        afterDraw = new CardInDeck[p.playerHand.Length + handSize];
        p.playerHand.CopyTo(afterDraw, 0);

        for (int h = 0; h < handSize; h++)
        {
            afterDraw[p.playerHand.Length + h] = FullDeck[h];
            p.playerList.Add(FullDeck[h]);
        }
        p.playerHand = afterDraw;
        Array.Sort(p.playerHand,
            delegate (CardInDeck x, CardInDeck y) { return (x.GColor + x.GLetter).CompareTo(y.GColor + y.GLetter); });
        p.playerList.Sort(delegate (CardInDeck x, CardInDeck y)
        {
            return (x.GColor + x.GLetter).CompareTo(y.GColor + y.GLetter);
        }
        );
        tempDeck = new CardInDeck[FullDeck.Length - handSize];
        for (int j = 0; j < FullDeck.Length - handSize; j++)
        {
            tempDeck[j] = FullDeck[j + handSize];
        }
        FullDeck = tempDeck;
        SetMessage($"Your turn. Pick a card from your hand.");
        //        SetMessage("Click \"Draw Card\", or drag another player's discard to your hand");

        // Debug.Log("end of DealMethod2 player=" + p.index);
    }

    public void DrawCard() // (Player p)
    {

        ModifiedShowCard(FullDeck[0]);
        tempDeck = new CardInDeck[FullDeck.Length - 1];
        for (int j = 0; j < FullDeck.Length - 1; j++)
        {
            tempDeck[j] = FullDeck[j + 1];
        }
        FullDeck = tempDeck;
        // ??       PlayButton.enabled = true;
        DrawButton.enabled = false;

        SetMessage("Make a word, add to an old word, or discard");
    }

    public void TakeDiscard() // (Player p)
    {
        //        players[2].playerHand[0] = FullDeck[0];
        //        PlayCard(players[2], 0);
        Debug.Log("trying to enable PlayButton button");
        PlayButton.enabled = true;
        //  ?????     discardPiles[1].transform.GetChild(0).SetParent(yourHand.transform);
    }
    public void CheckEndButton()
    {
    //  Debug.Log("CheckEndButton.  aparent count = " + yourHand.transform.childCount);
        if (yourHand.transform.childCount > 3)
        {
            EndButton.enabled = false;
        }
        else
        {
            EndButton.enabled = true;
        }
    }

    public void EnableEndButton()
    {
        EndButton.enabled = true;
    }

    [Serializable]
    public class CardInDeck
    {
        public string GLetter;
        public string GColor;
        public bool played = false;
        public enum CommonColors { red, blue, green };
        public enum RareColors { black };
        public enum RareLetters { J, K, Q, V, X, Z };
        public enum CommonLetters { A, E, L, B, T, D, C, F, G, H, I, M, N, O, P, R, S, U, W, Y };
    }

    [Serializable]
    public class Player
    {
        public CardInDeck[] playerHand;
        public List<CardInDeck> playerList;
        public int index;
    }

    [Serializable]
    public class OppArea
    {
        public GameObject[] oppWords = new GameObject[4];
    }

    public void PlayCard(Player p, int selectedCard)
    {
        CardInDeck selection = p.playerHand[selectedCard];

        CardInDeck[] tempGameBoard = new CardInDeck[gameBoard.Length + 1];
        gameBoard.CopyTo(tempGameBoard, 1);
        tempGameBoard[0] = selection;
        gameBoard = tempGameBoard;
        //        ShowCard(selection);
        ModifiedShowCard(selection);

        tempHand = new CardInDeck[p.playerHand.Length - 1];
        for (int j = 0; j < p.playerHand.Length; j++)
        {
            //                Debug.Log("copying card j=" + j + "   letter=" + p.playerHand[j].GLetter);
            if (j < selectedCard)
            { tempHand[j] = p.playerHand[j]; }
            if (j > selectedCard)
            { tempHand[j - 1] = p.playerHand[j]; }
        }
        p.playerHand = tempHand;
    }

    public void ShowCard(CardInDeck c)
    {
        GameObject newestPrefab = Instantiate(cardImagePrefab);
        newestPrefab.transform.SetParent(yourHand.transform);
        newestPrefab.name = c.GColor + " " + c.GLetter;
        //       newestPrefab.transform.SetParent(Solitaire.handArea.transform, false);
        Text childText = newestPrefab.GetComponentInChildren<Text>();
        cardInfoText = c.GLetter;   // "NEW TEXT?"; //c.value + " of " + c. 
                                    //     Image childImage = newestPrefab.GetComponentInChildren<Image>();
                                    //     childImage = cardImages[2] as Sprite;
        childText.text = cardInfoText;

    }

    public void ModifiedShowCard(CardInDeck c)
    {
        GameObject newestPrefab = Instantiate(cardImagePrefab);
        newestPrefab.transform.SetParent(yourHand.transform);
        newestPrefab.name = c.GColor + " " + c.GLetter;

        switch (currentPlayerIndex)
        {
            case 0:
                newestPrefab.GetComponent<RectTransform>().localPosition = Vector3.zero + Vector3.up * 150;
                break;
            case 1:
                newestPrefab.GetComponent<RectTransform>().localPosition = Vector3.zero + Vector3.right * 150;
                break;
            case 2:
                newestPrefab.GetComponent<RectTransform>().localPosition = Vector3.zero + Vector3.up * -150;
                break;
            case 3:
                newestPrefab.GetComponent<RectTransform>().localPosition = Vector3.zero + Vector3.right * -150;
                break;
        }
        /*       Text childText = newestPrefab.GetComponentInChildren<Text>();
               cardInfoText = c.GLetter; 
               childText.text = cardInfoText;
               childText.fontSize = 28;

               switch (c.GColor)
               {
                   case "red":
                       childText.color = Color.red;
                       break;
                   case "green":
                       childText.color = Color.green;
                       break;
                   case "blue":
                       childText.color = Color.blue;
                       break;
                   case "black":
                       childText.color = Color.black;
                       break;
               }
       */
        ImageIndex["A"] = 0;
        ImageIndex["B"] = 1;
        ImageIndex["C"] = 2;
        ImageIndex["D"] = 3;
        ImageIndex["E"] = 4;
        ImageIndex["F"] = 5;
        ImageIndex["G"] = 6;
        ImageIndex["H"] = 7;
        ImageIndex["I"] = 8;
        ImageIndex["J"] = 60;
        ImageIndex["K"] = 61;
        ImageIndex["L"] = 9;
        ImageIndex["M"] = 10;
        ImageIndex["N"] = 11;
        ImageIndex["O"] = 12;
        ImageIndex["P"] = 13;
        ImageIndex["Q"] = 62;
        ImageIndex["R"] = 14;
        ImageIndex["S"] = 15;
        ImageIndex["T"] = 16;
        ImageIndex["U"] = 17;
        ImageIndex["V"] = 63;
        ImageIndex["W"] = 18;
        ImageIndex["X"] = 64;
        ImageIndex["Y"] = 19;
        ImageIndex["Z"] = 65;

        int cardIndex = ImageIndex[c.GLetter];
        if (c.GColor == "blue") cardIndex += 20;
        if (c.GColor == "green") cardIndex += 40;

        CardInfo ci = newestPrefab.GetComponent<CardInfo>();
        newestPrefab.GetComponent<Image>().sprite = cardImages[cardIndex];

        if (hasChosen)
        {
            Debug.Log("ci.card=" + ci.card);
            //        ci.card = c;
        }
    }

    public void ChooseCard(CardInDeck c)
    {
        chosenCard = c;

        for (int i = 0; i < players[currentPlayerIndex].playerHand.Length; i++)
        {
            if (c == players[currentPlayerIndex].playerHand[i])
            {
                currentCardIndex = i;
            }
            hasChosen = true;
        }
    }

    public void EndTurn()
    {
        if (currentPlayerIndex == players.Length - 1)
        { currentPlayerIndex = 0; }
        else { currentPlayerIndex++; }

        currentCardIndex = 0;
        hasChosen = false;
    }

    public void ResetHand()
    {
        while (newWordArea.transform.childCount > 0)
        {
            //           Debug.Log("newWordArea child count=" + newWordArea.transform.childCount + "    child" + newWordArea.transform.GetChild(0));
            newWordArea.transform.GetChild(0).transform.SetParent(yourHand.transform);
            //           newWordArea.transform.GetChild(i).transform.SetSiblingIndex(0);            //  see OnDrag() in Draggable script
        }
        currentColor = null;
        currentCardIndex = 0;
        hasChosen = false;

    }

    public void ToInsert()
    {
        Debug.Log("ToInsert() position=" + this.transform.position);
    }



    public void PlayWord3()
    {
        int wordValue = 0;
        string thisLetter;
        string thisWord = "";
        // ??        Debug.Log("Before SetParent: newWordArea child count=" + newWordArea.transform.childCount + "    child=" + newWordArea.transform.GetChild(0));
        while (newWordArea.transform.childCount > 0)
        {
            thisLetter = newWordArea.transform.GetChild(0).name.Substring(newWordArea.transform.GetChild(0).name.Length - 1, 1);
            thisWord += thisLetter;
            wordValue += LetVal[thisLetter]; newWordArea.transform.GetChild(0).transform.SetParent(tabletop[wordCount].transform);
        }

        wordCount++;

        playerScore[0] += wordValue;
        SetMessage(thisWord + " scores " + wordValue + "   total = " + playerScore[0] + ".   Discard a card and click \"End Turn\" ");
        ScoreText.text = "Your score: " + playerScore[0];

        currentColor = null;
        currentCardIndex = 0;
        hasChosen = false;
        PlayButton.enabled = false;
        CheckEndButton();
    }

    public void PlayOppWord()
    {
        int childCount;
        string cardName;
        int wordValue = 0;
        string thisLetter;
        string thisWord = "";
        GameObject newestPrefab;

        Debug.Log("firstFound=" + firstFound);
        ptr[0] = Convert.ToInt32(firstFound.Substring(3, 1));
        ptr[1] = Convert.ToInt32(firstFound.Substring(4, 1));
        ptr[2] = Convert.ToInt32(firstFound.Substring(5, 1));

        // delete ??        Debug.Log("   Ptrs:" + ptr[0] + ptr[1] + ptr[2] + "   posArrs:" + posArr[0] + posArr[1] + posArr[2]);
        for (int i = 0; i < 3; i++)
        {
            CardInDeck c = players[oppNbr].playerList[cardPositions[ptr[i] - 1]];   //  posArr?   
            players[oppNbr].playerList[cardPositions[i]].played = true;

            newestPrefab = Instantiate(cardImagePrefab);
            newestPrefab.transform.SetParent(oppArea[oppNbr].oppWords[oppWordCount[oppNbr]].transform);
            newestPrefab.name = c.GColor + " " + c.GLetter;

            int cardIndex = ImageIndex[c.GLetter];
            if (c.GColor == "blue") cardIndex += 20;
            if (c.GColor == "green") cardIndex += 40;

            CardInfo ci = oppArea[oppNbr].oppWords[oppWordCount[oppNbr]].GetComponent<CardInfo>();
            newestPrefab.GetComponent<Image>().sprite = cardImages[cardIndex];
            childCount = oppArea[oppNbr].oppWords[oppWordCount[oppNbr]].transform.childCount;
            cardName = oppArea[oppNbr].oppWords[oppWordCount[oppNbr]].transform.GetChild(childCount - 1).name;
            thisLetter = cardName.Substring(cardName.Length - 1, 1);
            thisWord += thisLetter;
            wordValue += LetVal[thisLetter];
        }

        oppWordCount[oppNbr]++;

        playerScore[oppNbr] += wordValue;
        Debug.Log(thisWord + " scores " + wordValue + "   total = " + playerScore[oppNbr]);
#if UNITY_EDITOR
        EditorUtility.DisplayDialog("Turn" + turnCount, playerName[oppNbr] + " makes " + thisWord + " for " + wordValue + " points,  total = " + playerScore[oppNbr], "OK");
#else
//            SetMessage(playerName[oppNbr] + " makes " + thisWord + " for " + wordValue + " points,  total = " + playerScore[oppNbr]);
#endif
        endTurnMessage += playerName[oppNbr] + " makes " + thisWord + " for " + wordValue + " points, total = " + playerScore[oppNbr] + ".  ";
        OppMessageText[oppNbr].text = playerName[oppNbr] + " score: " + playerScore[oppNbr];
        ScoreText.text = "Your score: " + playerScore[0];

        for (int i = players[oppNbr].playerList.Count - 1; i >= 0; i--)
        {
            if (players[oppNbr].playerList[i].played)
            {
                players[oppNbr].playerList[i].played = false;
                players[oppNbr].playerList.RemoveAt(i);
            }
        }

        CheckEndButton();
    }

    public void DoneAdding()
    {
        playerScore[0] += AdditionZone.addedValue;
        if (AdditionZone.extendedOpp > 0)
        {
            playerScore[AdditionZone.extendedOpp] += AdditionZone.addedValue;
            SetMessage(AdditionZone.origWord + " to " + AdditionZone.expandedWord + " scores " + AdditionZone.addedValue
                + " points for " + playerName[AdditionZone.extendedOpp] + " (total " + playerScore[AdditionZone.extendedOpp]
                + ") and you (total " + playerScore[0] + ").\x0A  Make a new word, or discard");
            OppMessageText[AdditionZone.extendedOpp].text = playerName[AdditionZone.extendedOpp] + " " + playerScore[AdditionZone.extendedOpp];
        }
        else
        {
            SetMessage(AdditionZone.origWord + " to " + AdditionZone.expandedWord + " scores " + AdditionZone.addedValue
                + " points, total " + playerScore[0] + ".  \x0AMake a new word, or discard");
        }

        ScoreText.text = "Your score: " + playerScore[0];
        AdditionZone.origWord = AdditionZone.expandedWord;
        AdditionZone.addedValue = 0;

        AddButton.enabled = false;
        CheckEndButton();
    }


    internal protected void SetMessage(string message)
    {
        MessageText.text = message;
    }

    public void EndRound()
    {
        GameObject newestPrefab;

        for (int i = 1; i <= 3; i++)
        {
            afterDraw = new CardInDeck[players[i].playerHand.Length + 1];
            players[i].playerHand.CopyTo(afterDraw, 0);
            afterDraw[players[i].playerHand.Length] = FullDeck[i];
            players[i].playerHand = afterDraw;

            players[i].playerList.Add(FullDeck[i - 1]);
            // ??            EditorUtility.DisplayDialog("Debug", "adding " + FullDeck[i - 1].GColor + " " + FullDeck[i - 1].GLetter + "  to player " + i, "OK");

            Array.Sort(players[i].playerHand,
                    delegate (CardInDeck x, CardInDeck y) { return (x.GColor + x.GLetter).CompareTo(y.GColor + y.GLetter); });
            players[i].playerList.Sort(delegate (CardInDeck x, CardInDeck y)
            {
                return (x.GColor + x.GLetter).CompareTo(y.GColor + y.GLetter);
            }
        ); ;

        }
        tempDeck = new CardInDeck[FullDeck.Length - 3];
        for (int j = 0; j < FullDeck.Length - 3; j++)
        {
            tempDeck[j] = FullDeck[j + 3];
        }
        FullDeck = tempDeck;


        for (int i = 1; i <= 3; i++)
        {
            CardInDeck c = players[i].playerList[0];
            newestPrefab = Instantiate(cardImagePrefab);
            newestPrefab.transform.SetParent(discardPiles[i].transform);
            newestPrefab.name = c.GColor + " " + c.GLetter;

            int cardIndex = ImageIndex[c.GLetter];
            if (c.GColor == "blue") cardIndex += 20;
            if (c.GColor == "green") cardIndex += 40;

            CardInfo ci = discardPiles[i].GetComponent<CardInfo>();
            newestPrefab.GetComponent<Image>().sprite = cardImages[cardIndex];
            players[i].playerList.RemoveAt(0);
        }
        turnCount++;

        string possibleWord, blackLetters, blueLetters, redLetters, greenLetters;


        fs = textFile.text;
        fLines = Regex.Split(fs, "\r\n");

        for (int i = 0; i < fLines.Length - 1; i++)
        {
            keypair = Regex.Split(fLines[i], "\t");
            words3[keypair[0]] = keypair[1];
        }

        endTurnMessage = "";

        oppNbr = 1;
        for (oppNbr = 1; oppNbr <= 3; oppNbr++)
        {
            List<CardInDeck> tempCard = players[oppNbr].playerList;
            blackLetters = blueLetters = redLetters = greenLetters = "";
            searchedBlack.RemoveRange(0, searchedBlack.Count);
            searchedBlue.RemoveRange(0, searchedBlue.Count);
            searchedGreen.RemoveRange(0, searchedGreen.Count);
            searchedRed.RemoveRange(0, searchedRed.Count);

            for (int i = 0; i < tempCard.Count; i++)
            {
                switch (tempCard[i].GColor)
                {
                    case "black":
                        searchedBlack.Add(i);
                        searchedBlue.Add(i);
                        searchedGreen.Add(i);
                        searchedRed.Add(i);
                        blackLetters += tempCard[i].GLetter;
                        break;
                    case "blue":
                        searchedBlue.Add(i);
                        blueLetters += tempCard[i].GLetter;
                        break;
                    case "green":
                        greenLetters += tempCard[i].GLetter;
                        searchedGreen.Add(i);
                        break;
                    case "red":
                        redLetters += tempCard[i].GLetter;
                        searchedRed.Add(i);
                        break;
                }
            }

            firstFound = "";
            possibleWord = blackLetters + blueLetters;
            //    Debug.Log("blackLetters = " + blackLetters + "blueLetters = " + blueLetters + "   possible word " + possibleWord + "   redLetters= " + redLetters + "  searchedBlack.Count=" + searchedBlack.Count);


            if (possibleWord.Length >= 3)
            {
                searchedPos = searchedBlue;
                Debug.Log("returned from FindWords with blue " + FindWords(possibleWord));
            }

            possibleWord = blackLetters + greenLetters;
            if (firstFound == "" && possibleWord.Length >= 3)
            {
                searchedPos = searchedGreen;
                Debug.Log("returned from FindWords with green " + FindWords(possibleWord));
            }

            possibleWord = blackLetters + redLetters;
            if (firstFound == "" && possibleWord.Length >= 3)
            {
                //                Debug.Log("before Red search: black count=" + searchedBlack.Count + "   red count=" + searchedRed.Count + "   full list count=" + searchedPos.Count);
                searchedPos = searchedRed;
                //                EditorUtility.DisplayDialog("Debug", "calling Findwords with red " + possibleWord, "whatever");
                Debug.Log("returned from FindWords with red " + FindWords(possibleWord));
            }

            if (firstFound != "")
            {
                PlayOppWord();
            }
        }

        CheckEndButton();

        DrawButton.enabled = true;

        if (CheckWinner() == false)
        {
            if (CheckFinish() == false)
            {
                SetMessage(endTurnMessage + "Click \"Draw Card\", or drag a discard to your hand");      //    Application.Quit();
            }
        }
    }
    public bool CheckFinish()
    {
        if (yourHand.transform.childCount == 0)
        {
            SetMessage("End of round. You get a 5 point bonus for using all of your cards. Click \"Start Next Round\"");
            playerScore[0] += 5;
            ScoreText.text = "Your score: " + playerScore[0];
            //            EndOfRoundButton.enabled = true;
            return true;
        }

        for (int i = 1; i < players.Length; i++)
        {
            if (players[i].playerList.Count == 0)
            {
                SetMessage("End of round. " + playerName[i] + " gets A 5 point bonus for using all cards. Click \"Start Next Round\"");
                playerScore[i] += 5;
                //                EndOfRoundButton.enabled = true;
                return true;
            }
        }

        if (turnCount >= 5)
        {
            SetMessage("End of round. All players have had 5 turns. Click \"Start Next Round\"");
            //            EndOfRoundButton.enabled = true;
            return true;
        }

        return false;
    }


    string FindWords(string possibleWord)
    {
        string lookUpThis, lookUpThese;
        lookUpThese = "";
        for (int j = 0; j <= possibleWord.Length - 3; j++)
        {
            for (int k = j + 1; k <= possibleWord.Length - 2; k++)
            {
                for (int l = k + 1; l <= possibleWord.Length - 1; l++)
                {
                    lookUpThis = possibleWord.Substring(j, 1) + possibleWord.Substring(k, 1) + possibleWord.Substring(l, 1);
                    //                   Debug.Log("looking up " + lookUpThis);
                    if (words3.ContainsKey(lookUpThis.ToLower()))
                    {
                        cardPositions[0] = searchedPos[j];
                        cardPositions[1] = searchedPos[k];
                        cardPositions[2] = searchedPos[l];
                        // delete ??                         posArr[0] = searchedPos[j];
                        // delete ?? posArr[1] = searchedPos[k];
                        // delete ?? posArr[2] = searchedPos[l]; 
                        Debug.Log(lookUpThis + " =>  " + words3[lookUpThis.ToLower()] + "   positions " + searchedPos[j] + " " + searchedPos[k] + " " + searchedPos[l]);
                        if (firstFound == "")
                        {
                            firstFound = words3[lookUpThis.ToLower()] + j + k + l;
                        }
                        return words3[lookUpThis.ToLower()];
                    }
                    lookUpThese += (possibleWord.Substring(j, 1) + possibleWord.Substring(k, 1) + possibleWord.Substring(l, 1) + " ");
                }
            }
        }
        Debug.Log("No words among these: " + lookUpThese);
        return lookUpThese;
    }

    public void StartNewRound()
    {
        turnCount = 0;
        Debug.Log("StartNewRound:  #1Bot child count=" + oppArea[1].oppWords[0].transform.childCount + "    child 0 =" + oppArea[1].oppWords[0].transform.GetChild(0));

        while (yourHand.transform.childCount > 0)
        {
            Debug.Log("Trying to remove from hand: " + yourHand.transform.GetChild(0));
            yourHand.transform.GetChild(0).transform.SetParent(null);
        }

        for (int i = 1; i < players.Length; i++)
        {
            for (int j = 0; j < oppWordCount[i]; j++)
            {
                while (oppArea[i].oppWords[j].transform.childCount > 0)
                {
                    Debug.Log("Trying to remove " + oppArea[i].oppWords[j].transform.GetChild(0) + " from opponent " +  i + " word " + j);
        oppArea[i].oppWords[j].transform.GetChild(0).transform.SetParent(null);
                }
            }
            oppWordCount[i] = 0;
        }

        for (int k = 0; k < wordCount; k++)
        {
            while (tabletop[k].transform.childCount > 0)
            {
               Debug.Log("Trying to remove " + tabletop[k].transform.GetChild(0) + " from my played word " + k);
                tabletop[k].transform.GetChild(0).transform.SetParent(null);
            }
        }
        wordCount = 0;

        for (int n = 0; n < players.Length; n++)
        {
            while (discardPiles[n].transform.childCount > 0)
            {
                Debug.Log("Trying to remove discard " + discardPiles[n].transform.GetChild(0));
                discardPiles[n].transform.GetChild(0).transform.SetParent(null);
            }
        }

        for (int i = 0; i < players.Length; i++)
        {
            Player p = new Player();
            p.playerList = new List<CardInDeck>();
            p.playerHand = new CardInDeck[0];
            p.index = i;
            players[i] = p;
        }

        Debug.Log("      Rebuilding deck");
        BuildFullDeck();
        Shuffle();
        DealHands();
    }

    public bool CheckWinner()
    {
        int maxScore = 0;
        string endGameMessage = "Game is over. Congrats to ";
        string winnerName = "";

        for (int i = 0; i < players.Length; i++)
        {
            if (playerScore[i] >= 50)
            {
                if (maxScore < playerScore[i]) { maxScore = playerScore[i]; }
            }
        }
        if (maxScore >= 50)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (playerScore[i] == maxScore)
                {
                    if (winnerName == "")
                    {
                        winnerName = playerName[i];
                    }
                    else
                    {
                        winnerName += " and " + playerName[i];
                    }
                }
            }
            Debug.Log("Game is over");
            SetMessage(endGameMessage + winnerName);
            return true;
        }
        else
        {
//            Debug.Log("CheckWinner returned false");
            return false;
        }
    }

}
