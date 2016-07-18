using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class MonsterCard : CardBase, System.ICloneable
{
    
    new void FixedUpdate()
    {
        base.FixedUpdate();
        if (health <= 0)
        {
            Destroy(this);
        }
    }
    new public void PlaceCard()
    {
        base.PlaceCard();
    }
    void OnMouseDown()
    {
        if (cardStatus == CardStatus.InHand)
        {
            Selected = true;
        }

        
        //if (!BoardBehaviourScript.instance.currentCard && cardStatus==CardStatus.OnTable)
        //{
        //    //clicked on friendly card on table to attack another table card
        //    BoardBehaviourScript.instance.currentCard = this;
        //    print("Selected card: " + _Attack + ":" + health);
        //}
        //else if (BoardBehaviourScript.instance.currentCard && BoardBehaviourScript.instance.currentCard.cardtype == CardType.Magic && BoardBehaviourScript.instance.turn == BoardBehaviourScript.Turn.MyTurn && cardStatus == CardStatus.OnTable)
        //{

        //}
        //else if (BoardBehaviourScript.instance.currentCard && BoardBehaviourScript.instance.currentCard.cardtype == CardType.Monster && BoardBehaviourScript.instance.turn == BoardBehaviourScript.Turn.MyTurn && cardStatus == CardStatus.OnTable && BoardBehaviourScript.instance.currentCard!=this)//Card VS Card
        //{
        //    //clicked opponent card on table on your turn
        //    if (BoardBehaviourScript.instance.currentCard != null && BoardBehaviourScript.instance.currentCard.canPlay)
        //    {
        //        BoardBehaviourScript.instance.targetCard = this;
        //        print("Target card: " + _Attack + ":" + health);
        //        if (BoardBehaviourScript.instance.currentCard.canPlay)
        //        {
        //            AttackCard(BoardBehaviourScript.instance.currentCard, BoardBehaviourScript.instance.targetCard,true, delegate
        //            {
        //                BoardBehaviourScript.instance.currentCard.canPlay = false;
        //            });
        //        }
        //        else print("Card cannot attack");
        //    }
        //    print("Cannot Attack this Target card: ");
        //}
        //else if ((BoardBehaviourScript.instance.turn == BoardBehaviourScript.Turn.MyTurn && BoardBehaviourScript.instance.currentHero && cardStatus == CardStatus.OnTable))//Hero VS Card
        //{
        //    if (BoardBehaviourScript.instance.currentHero.CanAttack)
        //    {
        //        BoardBehaviourScript.instance.targetCard = this;
        //        print("Target card: " + _Attack + ":" + health);
        //        BoardBehaviourScript.instance.currentHero.AttackCard(BoardBehaviourScript.instance.currentHero, BoardBehaviourScript.instance.targetCard, delegate
        //        {
        //            BoardBehaviourScript.instance.currentHero.CanAttack = false;
        //        });
        //    }
        //}
        //else
        //{
        //    BoardBehaviourScript.instance.currentCard = null;
        //    BoardBehaviourScript.instance.currentHero = null;
        //    BoardBehaviourScript.instance.targetCard = null;
        //    BoardBehaviourScript.instance.targetHero = null;
        //    Debug.Log("Action Reset");
        //}

    }

    
    public void AttackCard(MonsterCard attacker, MonsterCard target,bool addhistory, CustomAction action)
    {
        if (attacker.canPlay)
        {
            target.health -= attacker._Attack;
            attacker.health -= target._Attack;

            if (target.health <= 0)
            {
                    Destroy(target);
            }

            if (attacker.health <= 0)
            {
                    attacker.Destroy(attacker);
            }

            action();
            if(addhistory)
            BoardBehaviourScript.instance.AddHistory(attacker, target);
        }
    }//Attack
    public void AttackHero(MonsterCard attacker, HeroBehaviourScript target, bool addhistory, CustomAction action)
    {
        if (attacker.canPlay)
        {
            target.health -= attacker._Attack;
            attacker.health -= target._Attack;

            action();
            if (addhistory)
                BoardBehaviourScript.instance.AddHistory(attacker, target);
        }
    }//Attack
    
    
    
    

    public object Clone()
    {
        MonsterCard temp = new MonsterCard();
        temp._name = _name;
        temp.description = this.description;
        temp.health = this.health;
        temp._Attack = this._Attack;
        temp.mana = this.mana;
        temp.canPlay = this.canPlay;
        temp.cardStatus = this.cardStatus;
        temp.team = this.team;
        temp.newPos = this.newPos;
        temp.distance_to_screen = this.distance_to_screen;
        temp.Selected = this.Selected;
        return temp;
    }
}
