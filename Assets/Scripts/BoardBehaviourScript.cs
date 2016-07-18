using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class BoardBehaviourScript : MonoBehaviour
{
    
    public static BoardBehaviourScript instance;
    public UnityEngine.UI.Text winnertext;
    public Transform MYDeckPos;
    public Transform MyHandPos;
    public Transform MyTablePos;

    public Transform AIDeckPos;
    public Transform AIHandPos;
    public Transform AITablePos;

    public List<GameObject> MyDeckCards = new List<GameObject>();
    public List<GameObject> MyHandCards = new List<GameObject>();
    public List<GameObject> MyTableCards = new List<GameObject>();

    public List<GameObject> AIDeckCards = new List<GameObject>();
    public List<GameObject> AIHandCards = new List<GameObject>();
    public List<GameObject> AITableCards = new List<GameObject>();

    public TextMesh MyManaText;
    public TextMesh AIManaText;

    public HeroBehaviourScript MyHero;
    public HeroBehaviourScript AIHero;

    public enum Turn { MyTurn, AITurn };

    #region SetStartData
    public Turn turn = Turn.MyTurn;

    int maxMana = 1;
    int MyMana = 1;
    int AIMana = 1;

    public bool gameStarted = false;
    int turnNumber = 1;
    #endregion

    public CardBase currentCard;
    public CardBase targetCard;
    public HeroBehaviourScript currentHero;
    public HeroBehaviourScript targetHero;

    public List<Hashtable> boardHistory = new List<Hashtable>();
    public int AILEVEL = 0;
    public LayerMask layer;
    public void AddHistory(GameBase a, GameBase b)
    {
        Hashtable hash = new Hashtable();

        hash.Add(a, b);

        boardHistory.Add(hash);
        currentCard = null;
        targetCard = null;
        currentHero = null;
        targetHero = null;
    }
    void Awake()
    {
        instance = this;
    }
    void Start()
    {

        foreach (GameObject CardObject in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardObject.GetComponent<Rigidbody>().isKinematic = true;
            CardBase c = CardObject.GetComponent<CardBase>();

            if (c.team == MonsterCard.Team.My)
                MyDeckCards.Add(CardObject);
            else
                AIDeckCards.Add(CardObject);
        }
        //Update Deck Cards Position
        DecksPositionUpdate();
        //Update Hand Cards Position
        HandPositionUpdate();

        //Start Game
        StartGame();
    }
    public void StartGame()
    {
        gameStarted = true;
        UpdateGame();

        for (int i = 0; i < 3; i++)
        {
            DrawCardFromDeck(MonsterCard.Team.My);
            DrawCardFromDeck(MonsterCard.Team.AI);
        }
    }
    public void DrawCardFromDeck(CardBase.Team team)
    {

        if (team == CardBase.Team.My && MyDeckCards.Count != 0 && MyHandCards.Count < 10)
        {
            int random = Random.Range(0, MyDeckCards.Count);
            GameObject tempCard = MyDeckCards[random];

            //tempCard.transform.position = MyHandPos.position;
            tempCard.GetComponent<CardBase>().newPos = MyHandPos.position;
            tempCard.GetComponent<CardBase>().SetCardStatus(CardBase.CardStatus.InHand);

            MyDeckCards.Remove(tempCard);
            MyHandCards.Add(tempCard);
        }

        if (team == CardBase.Team.AI && AIDeckCards.Count != 0 && AIHandCards.Count < 10)
        {
            int random = Random.Range(0, AIDeckCards.Count);
            GameObject tempCard = AIDeckCards[random];

            tempCard.transform.position = AIHandPos.position;
            tempCard.GetComponent<CardBase>().SetCardStatus(CardBase.CardStatus.InHand);

            AIDeckCards.Remove(tempCard);
            AIHandCards.Add(tempCard);
        }

        UpdateGame();
        //Update Hand Cards Position
        HandPositionUpdate();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            currentCard = null;
            targetCard = null;
            currentHero = null;
            targetHero = null;
            Debug.Log("Action Revet");
        }
        //if(BoardBehaviourScript.instance.currentCard&&BoardBehaviourScript.instance.targetCard)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 250, layer))
            {
                if (hit.transform.CompareTag("Board"))
                {
                    //do whatever......
                    //Debug.Log(hit.point);
                    if(BoardBehaviourScript.instance.currentHero)
                    drawMyLine(BoardBehaviourScript.instance.currentHero.transform.position, hit.point, Color.blue, 0.1f);
                    if (BoardBehaviourScript.instance.currentCard)
                    drawMyLine(BoardBehaviourScript.instance.currentCard.transform.position, hit.point, Color.blue, 0.1f);
                }
            }
        }

        if (MyHero.health <= 0)
            EndGame(AIHero);
        if (AIHero.health <= 0)
            EndGame(MyHero);

    }
    void drawMyLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {

        StartCoroutine(drawLine(start, end, color, duration));

    }
    IEnumerator drawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Additive"));
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetVertexCount(3);
        lr.SetPosition(0, start);
        lr.SetPosition(1,(( (-start+ end)*0.5f+start))+new Vector3(0,0,-5.0f));
        lr.SetPosition(2, end);
        yield return new WaitForSeconds(duration);
        GameObject.Destroy(myLine);
    }
    void UpdateGame()
    {
        MyManaText.text = MyMana.ToString() + "/" + maxMana;
        AIManaText.text = AIMana.ToString() + "/" + maxMana;

        if (MyHero.health <= 0)
            EndGame(AIHero);
        if (AIHero.health <= 0)
            EndGame(MyHero);

        //UpdateBoard();
    }

    void DecksPositionUpdate()
    {
        foreach (GameObject CardObject in MyDeckCards)
        {
            CardBase c = CardObject.GetComponent<CardBase>();

            if (c.cardStatus == CardBase.CardStatus.InDeck)
            {
                c.newPos = MYDeckPos.position + new Vector3(Random.value, Random.value, Random.value);
            }
        }

        foreach (GameObject CardObject in AIDeckCards)
        {
            CardBase c = CardObject.GetComponent<CardBase>();

            if (c.cardStatus == CardBase.CardStatus.InDeck)
            {
                c.newPos = AIDeckPos.position + new Vector3(Random.value, Random.value, Random.value);
            }
        }
    }
    public void HandPositionUpdate()
    {
        float space = 0f;
        float space2 = 0f;
        float gap = 1.3f;

        foreach (GameObject card in MyHandCards)
        {
            int numberOfCards = MyHandCards.Count;
            card.GetComponent<CardBase>().newPos = MyHandPos.position + new Vector3(-numberOfCards / 2 + space, 0, 0);
            space += gap;
        }

        foreach (GameObject card in AIHandCards)
        {
            int numberOfCards = AIHandCards.Count;
            card.GetComponent<CardBase>().newPos = AIHandPos.position + new Vector3(-numberOfCards / 2 + space2, 0, 0);
            space2 += gap;
        }
    }
    public void TablePositionUpdate()
    {
        float space = 0f;
        float space2 = 0f;
        float gap = 3;

        foreach (GameObject card in MyTableCards)
        {
            int numberOfCards = MyTableCards.Count;
            //card.transform.position = myTablePos.position + new Vector3(-numberOfCards + space - 2,0,0);
            card.GetComponent<CardBase>().newPos = MyTablePos.position + new Vector3(-numberOfCards + space - 2, 0, 0);
            space += gap;
        }

        foreach (GameObject card in AITableCards)
        {
            int numberOfCards = AITableCards.Count;
            //card.transform.position = AITablePos.position + new Vector3(-numberOfCards + space2,0,0);
            card.GetComponent<CardBase>().newPos = AITablePos.position + new Vector3(-numberOfCards + space2, 0, 0);
            space2 += gap;
        }
    }

    public void PlaceCard(CardBase card)
    {
        if (card.team == CardBase.Team.My && MyMana - card.mana >= 0 && MyTableCards.Count < 10)
        {
            //card.gameObject.transform.position = MyTablePos.position;
            card.newPos = MyTablePos.position;

            MyHandCards.Remove(card.gameObject);
            MyTableCards.Add(card.gameObject);

            card.SetCardStatus(CardBase.CardStatus.OnTable);
            //PlaySound(cardDrop);

            if (card.GetType() == typeof(MagicCard))///Apply Magic Effect 
            {
                MagicCard temp = (MagicCard)card;
                temp.canPlay = true;
                if (temp.cardeffect == MagicCard.CardEffect.ToAll)
                {
                    temp.AddToAll(temp, true, delegate { temp.Destroy(card); });
                }
                else if (temp.cardeffect == MagicCard.CardEffect.ToEnemies)
                {
                    temp.AddToEnemies(temp, AITableCards,true, delegate { temp.Destroy(card); });
                }
            }

            MyMana -= card.mana;
        }

        if (card.team == CardBase.Team.AI && AIMana - card.mana >= 0 && AITableCards.Count < 10)
        {
            //card.gameObject.transform.position = AITablePos.position;
            card.GetComponent<MonsterCard>().newPos = AITablePos.position;

            AIHandCards.Remove(card.gameObject);
            AITableCards.Add(card.gameObject);

            card.SetCardStatus(CardBase.CardStatus.OnTable);
            //PlaySound(cardDrop);

            if (card.GetType() == typeof(MagicCard))///Apply Magic Effect 
            {
                MagicCard temp = (MagicCard)card;
                temp.canPlay = true;
                if (temp.cardeffect == MagicCard.CardEffect.ToAll)
                {
                    temp.AddToAll(temp, true, delegate { temp.Destroy(card); });
                }
                else if (temp.cardeffect == MagicCard.CardEffect.ToEnemies)
                {
                    temp.AddToEnemies(temp, MyTableCards,true, delegate { temp.Destroy(card); });
                }
            }

            AIMana -= card.mana;
        }

        TablePositionUpdate();
        HandPositionUpdate();
        UpdateGame();
    }
    public void PlaceRandomCard(MonsterCard.Team team)
    {
        if (team == MonsterCard.Team.My && MyHandCards.Count != 0)
        {
            int random = Random.Range(0, MyHandCards.Count);
            GameObject tempCard = MyHandCards[random];

            PlaceCard(tempCard.GetComponent<MonsterCard>());
        }

        if (team == MonsterCard.Team.AI && AIHandCards.Count != 0)
        {
            int random = Random.Range(0, AIHandCards.Count);
            GameObject tempCard = AIHandCards[random];

            PlaceCard(tempCard.GetComponent<MonsterCard>());
        }

        UpdateGame();
        EndTurn();

        TablePositionUpdate();
        HandPositionUpdate();
    }
    public void EndGame(HeroBehaviourScript winner)
    {
        if (winner == MyHero)
        {
            Debug.Log("MyHero");
            Time.timeScale = 0;
            winnertext.text = "You Won";
            //Destroy(this);
        }

        if (winner == AIHero)
        {
            Time.timeScale = 0;
            Debug.Log("AIHero");
            winnertext.text = "You Losse";
            //Destroy(this);
        }
    }
    void OnGUI()
    {
        if (gameStarted)
        {
            if (turn == Turn.MyTurn)
            {
                if (GUI.Button(new Rect(Screen.width - 200, Screen.height / 2 - 50, 100, 50), "End Turn"))
                {
                    EndTurn();
                }
            }

            GUI.Label(new Rect(Screen.width-200, Screen.height / 2 - 100, 100, 50), "Turn: " + turn + " Turn Number: " + turnNumber.ToString());

            foreach (Hashtable history in boardHistory)
            {
                foreach (DictionaryEntry entry in history)
                {
                    GameBase card1 = entry.Key as GameBase;
                    GameBase card2 = entry.Value as GameBase;

                    GUILayout.Label(card1._name + " > " + card2._name);
                }
            }
            if (boardHistory.Count > 25)
            {
                Hashtable temp;
                temp = boardHistory[boardHistory.Count - 1];
                boardHistory.Clear();
                boardHistory.Add(temp);
            }
        }
    }
    void EndTurn()
    {
        maxMana += (turnNumber-1)%2;
        if (maxMana >= 10) maxMana = 10;
        MyMana = maxMana;
        AIMana = maxMana;
        turnNumber += 1;
        currentCard = new MonsterCard() ;
        targetCard = new MonsterCard();
        currentHero = new HeroBehaviourScript();
        targetHero = new HeroBehaviourScript();
        foreach (GameObject card in MyTableCards)
            card.GetComponent<MonsterCard>().canPlay = true;

        foreach (GameObject card in AITableCards)
            card.GetComponent<MonsterCard>().canPlay = true;
        MyHero.CanAttack = true;
        AIHero.CanAttack = true;

        if (turn == Turn.AITurn)
        {
            DrawCardFromDeck(MonsterCard.Team.My);
            turn = Turn.MyTurn;
        }
        else if (turn == Turn.MyTurn)
        {
            DrawCardFromDeck(MonsterCard.Team.AI);
            turn = Turn.AITurn;
        }

        HandPositionUpdate();
        TablePositionUpdate();

        OnNewTurn();
    }
    void OnNewTurn()
    {
        UpdateGame();

        if (turn == Turn.AITurn)
            AI_Think();
        //Invoke("AI_Think", 5.0f);
    }
    void AI_Think()
    {
        if (AILEVEL == 0)
        {
            if (turn == Turn.AITurn)
            {
                Invoke("RendomActions", 2.0f);
            }
        }
        else if (AILEVEL > 0)
        {
            if (turn == Turn.AITurn)
            {
                Invoke("AIthink", 2.0f);
            }
        }
    }
    void AIthink()
    {
        AIGameState.AllStates.Clear();// = new List<AIGameState>();
        AIGetPlacing();
        AIGetAttacks();
        EndTurn();
    }
    void RendomActions()
    {
        #region placing cards
        //float chanceToPlace = Random.value;

        if (AIHandCards.Count == 0)
        {
            EndTurn();
        }
        else
        {
            PlaceRandomCard(MonsterCard.Team.AI);
        }
        #endregion
        #region attacking
        Hashtable attacks = new Hashtable();
        foreach (GameObject Card in AITableCards)
        {
            MonsterCard card = Card.GetComponent<MonsterCard>();

            if (card.canPlay)
            {
                float changeToAttackhero = Random.value;

                if (changeToAttackhero < 0.50f)
                {
                    card.AttackHero(card, MyHero, true, delegate
                    {
                        card.canPlay = false;
                    });
                }
                else if (MyTableCards.Count > 0)
                {
                    int random = Random.Range(0, MyTableCards.Count);
                    GameObject CardToAttack = MyTableCards[random];

                    attacks.Add(card, CardToAttack.GetComponent<MonsterCard>());
                }
            }
        }

        foreach (DictionaryEntry row in attacks)
        {
            CardBase tempCard = row.Key as CardBase;
            CardBase temp2 = row.Value as CardBase;

            if (tempCard.GetType() == typeof(MonsterCard))
            {
                ((MonsterCard)tempCard).AttackCard((MonsterCard)tempCard, (MonsterCard)temp2, true, delegate
                {
                    tempCard.canPlay = false;
                });
            }
            else if (tempCard.GetType() == typeof(MagicCard))
            {
                ((MagicCard)tempCard).AddToMonster((MagicCard)tempCard, (MonsterCard)temp2,true, delegate
                {
                    tempCard.canPlay = false;
                    tempCard.Destroy(tempCard);
                });
            }

        }
        #endregion
    }
    void AIGetPlacing()
    {
        AIGameState InitialState = new AIGameState(/*MyHandCards,*/MyTableCards,AIHandCards,AITableCards,MyHero,AIHero,maxMana,MyMana,AIMana,turn,null);
        InitialState.GetAllPlacingAction();
        //Find Best Score
        float MaxScore = float.MinValue;
        AIGameState BestState = new AIGameState();
        foreach (AIGameState item in AIGameState.AllStates)
        {
            if (item.State_Score > MaxScore)
            {
                MaxScore = item.State_Score;
                BestState = item;
            }
        }
        int count = BestState.Actions.Count;
        //GetActions
        for (int i = 0; i < count; i++)
        {
            AIGameState.Action a;
            a = BestState.Actions.Dequeue();
            if (a.OpCode == 0)
            {

                foreach (var item in AIHandCards)
                {
                    if (item.GetComponent<MonsterCard>()._name == a.Card1)
                    {
                        PlaceCard(item.GetComponent<MonsterCard>());
                        break;
                    }
                }
            }
        }
        AIGameState.AllStates.Clear();
    }
    void AIGetAttacks()
    {
        AIGameState InitialState = new AIGameState(/*MyHandCards,*/MyTableCards, AIHandCards, AITableCards, MyHero, AIHero, maxMana, MyMana, AIMana, turn,null);
        InitialState.GetAllAttackingActions(AILEVEL);
        //Find Best Score
        float MaxScore = float.MinValue;
        AIGameState BestState = new AIGameState();
        foreach (AIGameState item in AIGameState.AllStates)
        {
            if (item.State_Score > MaxScore)
            {
                MaxScore = item.State_Score;
                BestState = item;
            }
        }
        //Debug.Log("Best choice Index" + BestState.Index);
        int count = BestState.Actions.Count;
        //GetActions

        //for (int i = 0; i < count; i++)
        //{
        //    AIGameState.Action a;
        //    a = BestState.Actions.Dequeue();
        //    if (a.OpCode == 1)
        //    {
        //        foreach (var item in AITableCards)//Find Card1
        //        {
        //            if (item.GetComponent<MonsterCard>()._name == a.Card1)
        //            {
        //                currentCard = item.GetComponent<MonsterCard>();
        //                break;
        //            }
        //        }
        //        foreach (var item in MyTableCards)//Find Card2
        //        {
        //            if (item.GetComponent<MonsterCard>()._name == a.Card2)
        //            {
        //                targetCard = item.GetComponent<MonsterCard>();
        //                break;
        //            }
        //        }
        //        if (currentCard != null && targetCard != null)//MakeAction
        //        {
        //            currentCard.AttackCard(currentCard, targetCard, true, delegate
        //            {
        //                currentCard.canPlay = false;
        //            });
        //        }
        //    }
        //    else if (a.OpCode == 2)
        //    {
        //        foreach (var item in AITableCards)//Find Card1
        //        {
        //            if (item.GetComponent<MonsterCard>()._name == a.Card1)
        //            {
        //                currentCard = item.GetComponent<MonsterCard>();
        //                break;
        //            }
        //        }
        //        if (a.Hero == "MyHero")
        //        {
        //            targetHero = MyHero;
        //        }
        //        if (currentCard != null && targetHero != null)
        //        {
        //            currentCard.AttackHero(currentCard, MyHero, true, delegate
        //            {
        //                currentCard.canPlay = false;
        //            });
        //        }
        //    }else if (a.OpCode == 3)
        //    {
        //        foreach (var item in AITableCards)//Find Card1
        //        {
        //            if (item.GetComponent<MonsterCard>()._name == a.Card1)
        //            {
        //                currentCard = item.GetComponent<MonsterCard>();
        //                break;
        //            }
        //        }
        //        foreach (var item in MyTableCards)//Find Card2
        //        {
        //            if (item.GetComponent<MonsterCard>()._name == a.Card2)
        //            {
        //                targetCard = item.GetComponent<MonsterCard>();
        //                break;
        //            }
        //        }
        //        if (currentCard != null && targetCard != null)//MakeAction
        //        {
        //            currentCard.AddToMonster(currentCard, targetCard, true, delegate
        //            {
        //                currentCard.Destroy(currentCard);
        //            });
        //        }
        //    }
        //}


        //AIGameState.AllStates=new List< AIGameState > ();
    }
    void OnTriggerEnter(Collider Obj)
    {
        CardBase card = Obj.GetComponent<CardBase>();
        if (card)
        {
            card.PlaceCard();
        }

    }
}
