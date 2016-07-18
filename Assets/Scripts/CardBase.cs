using UnityEngine;
using System.Collections;

public class CardBase : GameBase {
    public string description = "Description";
    public Texture2D image;
    public int health;
    public int _Attack;
    public int mana;

    public bool canPlay = false;

    public enum Team { My, AI };
    public Team team = Team.My;

    protected float distance_to_screen;
    protected bool Selected = false;
    public enum CardStatus { InDeck, InHand, OnTable, Destroyed };
    public CardStatus cardStatus = CardStatus.InDeck;
    public Vector3 newPos;

    public TextMesh nameText;
    public TextMesh healthText;
    public TextMesh AttackText;
    public TextMesh manaText;
    public TextMesh DescriptionText;
    public TextMesh DebugText;

    public delegate void CustomAction();

    void Start()
    {
        distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z - 1;
        DescriptionText.text = description.ToString();
    }
    protected void FixedUpdate()
    {
        if (!Selected)
        {
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 3);
            if (cardStatus != CardStatus.InDeck)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0.0f, 180.0f, 0.0f), Time.deltaTime * 3);
            }
        }
        //Update Visuals
        nameText.text = _name.ToString();
        healthText.text = health.ToString();
        AttackText.text = _Attack.ToString();
        manaText.text = mana.ToString();
        DebugText.text = canPlay ? "Ready to attack" : "Nope";
    }
    void OnMouseUp()
    {
        //Debug.Log("On Mouse Up Event");
        Selected = false;
    }
    void OnMouseOver()
    {

        //Debug.Log("On Mouse Over Event");
    }
    void OnMouseEnter()
    {
        //Debug.Log("On Mouse Enter Event");
        //newPos += new Vector3(0,0.5f,0);
    }
    void OnMouseExit()
    {
        //Debug.Log("On Mouse Exit Event");
        //newPos -= new Vector3(0,0.5f, 0);
    }
    void OnMouseDrag()
    {
        //Debug.Log("On Mouse Drag Event");
        GetComponent<Rigidbody>().MovePosition(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen)));
    }
    public void SetCardStatus(CardStatus status)
    {
        cardStatus = status;
    }
    public void Destroy(CardBase card)
    {
        if (card)
        {
            if (card.gameObject != null)
            {
                if (card.team == CardBase.Team.My)
                    BoardBehaviourScript.instance.MyTableCards.Remove(card.gameObject);
                else if (card.team == CardBase.Team.AI)
                    BoardBehaviourScript.instance.AITableCards.Remove(card.gameObject);


                //BoardBehaviourScript.instance.PlaySound(BoardBehaviourScript.instance.cardDestroy);
                Destroy(card.gameObject);

                BoardBehaviourScript.instance.TablePositionUpdate();
            }

        }
        else
        {
            //card = null;
        }
    }
    public void PlaceCard()
    {
        if (BoardBehaviourScript.instance.turn == BoardBehaviourScript.Turn.MyTurn && cardStatus == CardStatus.InHand && team == Team.My)
        {
            //Selected = false;
            BoardBehaviourScript.instance.PlaceCard(this);
        }
    }
}
