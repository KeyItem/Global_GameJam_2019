using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : EntityAbility
{
    [Header("Player Action Attributes")]
    public BaseAbilityCard playerBasicAttack;
    
    [Space(10)]
    public AbilityDeck playerAbilityDeck;

    [Space(10)]
    public AbilityHand playerAbilityHand;

    [Space(10)]
    public int debugHandSize = 3;

    public override void InitializeAbilitySystem()
    {
        InitializeAbilityDeck();
        InitializeAbilityHand();
        InitializeAbilityCards();
    }

    public void InitializeAbilityDeck()
    {
        playerAbilityDeck = new AbilityDeck(entityAbilities);
        playerAbilityDeck.ShuffleAbilityDeck();
    }

    public void InitializeAbilityHand()
    {
        playerAbilityHand = playerAbilityDeck.RequestNewAbilityHand(debugHandSize);
    }

    public void InitializeAbilityCards()
    {
        for (int i = 0; i < playerAbilityHand.availableAbilityCardsInHand.Count; i++)
        {
            playerAbilityHand.availableAbilityCardsInHand[i].cardAbility.InitializeAbility(entityAbilityObjects[i]);
        }
    }

    public override void PerformAbilities(InputInfo inputInfo, EntityCollisionInfo collisionInfo, EntityDetectionInfo detectionInfo)
    {
        if (CheckForSelectionInput(inputInfo))
        {
            if (inputInfo.ReturnButtonReleaseState("ShiftActionLeft"))
            {
                playerAbilityHand.ShiftCardsInHandLeft();
            }
            else if (inputInfo.ReturnButtonReleaseState("ShiftActionRight"))
            {
                playerAbilityHand.ShiftCardsInHandRight();
            }

            return;
        }
        else if (CheckForBasicAttackInput(inputInfo) && CheckForActiveAbilities())
        {
            BaseAbilityCard requestAbilityCard = playerBasicAttack;
            BaseAbility requestedAbility = requestAbilityCard.cardAbility; //TODO: Add dynamic ability selection for NPC
            ActiveAbility requestActiveAbility = new ActiveAbility(requestedAbility, ReturnRequestAbilityIndex(inputInfo));

            if (CheckIfAbilityIfOnCooldown(requestedAbility))
            {
                PrepareAbility(requestedAbility);
                UseAbility(requestedAbility);
                AddAbilityToActive(requestActiveAbility);
            }
        }
        else if (CheckInput(inputInfo) && CheckForActiveAbilities())
        {
            BaseAbilityCard requestAbilityCard = ReturnNextAbility();
            BaseAbility requestedAbility = requestAbilityCard.cardAbility; //TODO: Add dynamic ability selection for NPC
            ActiveAbility requestActiveAbility = new ActiveAbility(requestedAbility, ReturnRequestAbilityIndex(inputInfo));

            if (CheckIfAbilityIfOnCooldown(requestedAbility))
            {
                PrepareAbility(requestedAbility);
                UseAbility(requestedAbility);
                AddAbilityToActive(requestActiveAbility);
                DiscardAbilityCard(requestAbilityCard);
            }
        }
    }

    public void DiscardAbilityCard(BaseAbilityCard abilityCard)
    {
        playerAbilityHand.RemoveCardFromHand(abilityCard);

        if (playerAbilityHand.IsAbilityHandEmpty())
        {
            if (playerAbilityDeck.IsAvailableCardsInDeckEmpty())
            {
                playerAbilityDeck.ResetAvailableCards();
                playerAbilityDeck.ShuffleAbilityDeck();
            }

            playerAbilityHand = playerAbilityDeck.RequestNewAbilityHand(debugHandSize);
        }
    }

    public BaseAbilityCard ReturnNextAbility()
    {
        if (playerAbilityHand.IsAbilityHandEmpty())
        {
            if (playerAbilityDeck.IsAvailableCardsInDeckEmpty())
            {
                playerAbilityDeck.ResetAvailableCards();
                playerAbilityDeck.ShuffleAbilityDeck();
            }

            playerAbilityHand = playerAbilityDeck.RequestNewAbilityHand(debugHandSize);
        }

        return playerAbilityHand.availableAbilityCardsInHand[0];
    }

    public bool CheckForBasicAttackInput(InputInfo input)
    {
        if (input.ReturnCurrentButtonState("BasicAction"))
        {
            return true;
        }

        return false;
    }

    public bool CheckForActiveAbilities()
    {
        if (entityActiveAbilities.Count > 0)
        {
            return false;
        }

        return true;
    }

    public bool CheckForSelectionInput(InputInfo input)
    {
        if (input.ReturnCurrentButtonState("ShiftActionLeft") || input.ReturnCurrentButtonState("ShiftActionRight"))
        {
            return true;
        }

        return false;
    }
}

[System.Serializable]
public struct AbilityDeck
{
    [Header("Ability Deck Attributes")]
    public List<BaseAbilityCard> baseAbilityCardDeck;

    [Space(10)]
    public List<BaseAbilityCard> availableAbilityCardsInDeck;

    [Space(10)]
    public List<BaseAbilityCard> discardedAbilityCardsInDeck;

    public AbilityDeck(List<BaseAbilityCard> abilitiesToFormDeck)
    {
        baseAbilityCardDeck = new List<BaseAbilityCard>(abilitiesToFormDeck);
        availableAbilityCardsInDeck = new List<BaseAbilityCard>(abilitiesToFormDeck);
        discardedAbilityCardsInDeck = new List<BaseAbilityCard>();
    }

    public AbilityHand RequestNewAbilityHand(int cardHandSize)
    {
        List<BaseAbilityCard> newCardsToFormHand = new List<BaseAbilityCard>();

        for (int i = 0; i < cardHandSize; i++)
        {
            if (availableAbilityCardsInDeck.Count > 0)
            {
                BaseAbilityCard newCardToAdd = availableAbilityCardsInDeck[0];

                newCardsToFormHand.Add(newCardToAdd);

                DiscardCardFromAvailableCards(newCardToAdd);
            }
            else
            {
                break;
            }
        }

        if (newCardsToFormHand.Count == 0)
        {
            Debug.LogError("New Hand Size is 0!");
        }

        return new AbilityHand(newCardsToFormHand);
    }

    public void ResetAvailableCards()
    {
        availableAbilityCardsInDeck = new List<BaseAbilityCard>(baseAbilityCardDeck);
        discardedAbilityCardsInDeck = new List<BaseAbilityCard>();
    }

    public void ShuffleAbilityDeck()
    {
        if (availableAbilityCardsInDeck.Count > 0)
        {
            for (int i = 0; i < availableAbilityCardsInDeck.Count; i++)
            {
                BaseAbilityCard temporaryCard = availableAbilityCardsInDeck[i];

                int randomIndex = Random.Range(i, availableAbilityCardsInDeck.Count);

                availableAbilityCardsInDeck[i] = availableAbilityCardsInDeck[randomIndex];

                availableAbilityCardsInDeck[randomIndex] = temporaryCard;
            }
        }
    }

    public void DiscardCardFromAvailableCards(BaseAbilityCard cardToDiscard)
    {
        if (availableAbilityCardsInDeck.Contains(cardToDiscard))
        {
            availableAbilityCardsInDeck.Remove(cardToDiscard);
            discardedAbilityCardsInDeck.Add(cardToDiscard);
        }
    }

    public bool IsAvailableCardsInDeckEmpty()
    {
        if (availableAbilityCardsInDeck.Count > 0)
        {
            return false;
        }

        return true;
    }
}

[System.Serializable]
public struct AbilityHand
{
    [Header("Ability Hand Attributes")]
    public List<BaseAbilityCard> availableAbilityCardsInHand;

    [Space(10)]
    public List<BaseAbilityCard> discardedAbilityCardsInHand;

    public AbilityHand (List<BaseAbilityCard> newCardsToFormHand)
    {
        if (newCardsToFormHand.Count == 0)
        {
            Debug.LogError("0 Size List passed through to Hand Generation");
        }

        this.availableAbilityCardsInHand = new List<BaseAbilityCard>(newCardsToFormHand);
        this.discardedAbilityCardsInHand = new List<BaseAbilityCard>();
    }

    public void ShiftCardsInHandLeft()
    {
        List<BaseAbilityCard> tempList = new List<BaseAbilityCard>();

        for (int i = 0; i < availableAbilityCardsInHand.Count; i++)
        {
            if (i < availableAbilityCardsInHand.Count - 1)
            {
                tempList.Add(availableAbilityCardsInHand[i + 1]);
            }
            else
            {
                tempList.Add(availableAbilityCardsInHand[0]);
            }
        }

        availableAbilityCardsInHand = new List<BaseAbilityCard>(tempList);
    }

    public void ShiftCardsInHandRight()
    {
        List<BaseAbilityCard> tempList = new List<BaseAbilityCard>();

        for (int i = 0; i < availableAbilityCardsInHand.Count; i++)
        {
            if (i == 0)
            {
                tempList.Add(availableAbilityCardsInHand[availableAbilityCardsInHand.Count - 1]);
            }
            else
            {
                tempList.Add(availableAbilityCardsInHand[i - 1]);
            }
        }

        availableAbilityCardsInHand = new List<BaseAbilityCard>(tempList);
    }

    public void ShuffleAbilityHand()
    {
        for (int i = 0; i < availableAbilityCardsInHand.Count; i++)
        {
            BaseAbilityCard temporaryCard = availableAbilityCardsInHand[i];

            int randomIndex = Random.Range(i, availableAbilityCardsInHand.Count);

            availableAbilityCardsInHand[i] = availableAbilityCardsInHand[randomIndex];

            availableAbilityCardsInHand[randomIndex] = temporaryCard;
        }
    }

    public void AddCardToAbilityHand(BaseAbilityCard cardToAdd)
    {
        availableAbilityCardsInHand.Add(cardToAdd);
    }

    public void AddCardsToAbilityHand(List<BaseAbilityCard> cardsToAddToHand)
    {
        for (int i = 0; i < cardsToAddToHand.Count; i++)
        {
            availableAbilityCardsInHand.Add(cardsToAddToHand[i]);
        }
    }

    public void RemoveCardFromHand(BaseAbilityCard cardToRemove)
    {
        if (availableAbilityCardsInHand.Contains(cardToRemove))
        {
            availableAbilityCardsInHand.Remove(cardToRemove);

            return;
        }

        Debug.LogError("Trying to remove Card from Hand that is not present in Hand");
    }

    public void RemoveCardsFromHand(List<BaseAbilityCard> cardsToRemove)
    {
        for (int i = 0; i < cardsToRemove.Count; i++)
        {
            if (availableAbilityCardsInHand.Contains(cardsToRemove[i]))
            {
                availableAbilityCardsInHand.Remove(cardsToRemove[i]);
            }
        }
    }

    public bool IsAbilityHandEmpty()
    {
        if (availableAbilityCardsInHand.Count > 0)
        {
            return false;
        }

        return true;
    }
}
