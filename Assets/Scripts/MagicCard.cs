using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicCard : CardBase, System.ICloneable
{
    public enum CardEffect { ToAll, ToEnemies, ToSpecific };
    public CardEffect cardeffect;
    public int AddedHealth;
    public int AddedAttack;

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


        //if (!BoardBehaviourScript.instance.currentCard && cardStatus == CardStatus.OnTable)
        //{
        //    //clicked on friendly card on table to attack another table card
        //    BoardBehaviourScript.instance.currentCard = this;
        //    print("Selected card: " + _Attack + ":" + health);
        //}
        //else if (BoardBehaviourScript.instance.currentCard && BoardBehaviourScript.instance.currentCard.cardtype == CardType.Magic && BoardBehaviourScript.instance.turn == BoardBehaviourScript.Turn.MyTurn && cardStatus == CardStatus.OnTable)
        //{
        //    if (BoardBehaviourScript.instance.currentCard.cardeffect == CardEffect.ToSpecific)//Magic VS Card
        //    {//What Magic Card Will Do To MonsterCard
        //        BoardBehaviourScript.instance.targetCard = this;
        //        print("Target card: " + _Attack + ":" + health);
        //        if (BoardBehaviourScript.instance.currentCard.canPlay)
        //        {
        //            AddToMonster(BoardBehaviourScript.instance.currentCard, BoardBehaviourScript.instance.targetCard, true, delegate
        //            {
        //                BoardBehaviourScript.instance.currentCard.Destroy(BoardBehaviourScript.instance.currentCard);
        //            });
        //        }
        //    }

        //}
        //else if (BoardBehaviourScript.instance.currentCard && BoardBehaviourScript.instance.currentCard.cardtype == CardType.Monster && BoardBehaviourScript.instance.turn == BoardBehaviourScript.Turn.MyTurn && cardStatus == CardStatus.OnTable && BoardBehaviourScript.instance.currentCard != this)//Card VS Card
        //{
        //    //clicked opponent card on table on your turn
        //    if (BoardBehaviourScript.instance.currentCard != null && BoardBehaviourScript.instance.currentCard.canPlay)
        //    {
        //        BoardBehaviourScript.instance.targetCard = this;
        //        print("Target card: " + _Attack + ":" + health);
        //        if (BoardBehaviourScript.instance.currentCard.canPlay)
        //        {
        //            //AttackCard(BoardBehaviourScript.instance.currentCard, BoardBehaviourScript.instance.targetCard, true, delegate
        //            //{
        //            //    BoardBehaviourScript.instance.currentCard.canPlay = false;
        //            //});
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
    public void AddToHero(MagicCard magic, HeroBehaviourScript target, CustomAction action)
    {
        if (magic.canPlay)
        {
            target._Attack += magic.AddedAttack;
            if (target.health + magic.AddedHealth <= 30)
                target.health += magic.AddedHealth;
            else
                target.health = 30;
            action();
            BoardBehaviourScript.instance.AddHistory(magic, target);
        }
    }//Magic
    public void AddToMonster(MagicCard magic, MonsterCard target, bool addhistory, CustomAction action)
    {
        if (magic.canPlay && target)
        {
            target._Attack += magic.AddedAttack;
            target.health += magic.AddedHealth;
            action();
            if (addhistory)
                BoardBehaviourScript.instance.AddHistory(magic, target);
        }
    }//Magic

    public void AddToAll(MagicCard magic, bool addhistory, CustomAction action)
    {
        if (magic.canPlay)
        {
            foreach (var target in BoardBehaviourScript.instance.AITableCards)
            {
                AddToMonster(magic, target.GetComponent<MonsterCard>(), addhistory, delegate { });
            }
            foreach (var target in BoardBehaviourScript.instance.MyTableCards)
            {
                AddToMonster(magic, target.GetComponent<MonsterCard>(), addhistory, delegate { });
            }
            action();
        }
    }//Magic
    public void AddToEnemies(MagicCard magic, List<GameObject> targets, bool addhistory, CustomAction action)
    {
        if (magic.canPlay)
        {
            foreach (var target in targets)
            {
                AddToMonster(magic, target.GetComponent<MonsterCard>(), addhistory, delegate { });
            }
            action();
        }
    }//Magic
    public void AddToEnemies(MagicCard magic, List<MonsterCard> targets, bool addhistory, CustomAction action)
    {
        if (magic.canPlay)
        {
            foreach (var target in targets)
            {
                AddToMonster(magic, target, addhistory, delegate { });
            }
            action();
        }
    }//Magic

    public object Clone()
    {
        MagicCard temp = new MagicCard();
        temp._name = _name;
        temp.description = this.description;
        temp.health = this.health;
        temp._Attack = this._Attack;
        temp.mana = this.mana;
        temp.canPlay = this.canPlay;
        temp.cardStatus = this.cardStatus;
        temp.cardeffect = this.cardeffect;
        temp.AddedHealth = this.AddedHealth;
        temp.AddedAttack = this.AddedAttack;
        temp.team = this.team;
        temp.newPos = this.newPos;
        temp.distance_to_screen = this.distance_to_screen;
        temp.Selected = this.Selected;
        return temp;
    }
}
