using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
public struct PlayerStruct : INetworkSerializable
{
    public char[] name;
    public int maxMoney;
    public int id;
    public int sciencePoints;
    public int money;
    public int actionPoints;

    public int[] rawDeck;
    public int[] deck;
    public CardStruct[] hand;
    public CardStruct[] units;// = new Card[3, 3]

    public PlayerStruct(Player player)
    {
        name = player.name.ToCharArray();
        maxMoney = player.maxMoney;
        id = player.id;
        sciencePoints = player.sciencePoints;
        money = player.Money;
        actionPoints = player.actionPoints;

        rawDeck = new int[player.rawDeck.Count];
        deck = new int[player.deck.Count];
        hand = new CardStruct[player.hand.Count];
        units = new CardStruct[9];

        for (int i = 0; i < player.rawDeck.Count; i++)
        {
            if (player.rawDeck[i] != null) rawDeck[i] = player.rawDeck[i].CardDataId;
        }

        for (int i = 0; i < player.deck.Count; i++)
        {
            if (player.deck[i] != null) deck[i] = player.deck[i].CardDataId;
        }

        for (int i = 0; i < player.hand.Count; i++)
        {
            if (player.hand[i] != null) hand[i] = new CardStruct(player.hand[i]);
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; i < 3; i++)
            {
                if (player.units[i, j] != null) units[(i*3) + j] = new CardStruct(player.units[i, j]);
            }        
        }
        //new CardStruct(player.deck[i]);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref maxMoney);
        serializer.SerializeValue(ref id);
        serializer.SerializeValue(ref sciencePoints);
        serializer.SerializeValue(ref money);
        serializer.SerializeValue(ref actionPoints);


        //ARRAYS
        //serialize length

        int array1Length = name != null ? name.Length : 0;
        int array2Length = deck != null ? deck.Length : 0;
        int array3Length = rawDeck != null ? rawDeck.Length : 0;
        int array4Length = hand != null ? hand.Length : 0;
        int array5Length = units != null ? units.Length : 0;

        serializer.SerializeValue(ref array1Length);
        serializer.SerializeValue(ref array2Length);
        serializer.SerializeValue(ref array3Length);
        serializer.SerializeValue(ref array4Length);
        serializer.SerializeValue(ref array5Length);

        //allocate array
        if (serializer.IsReader)
        {
            name = new char[array1Length];
            deck = new int[array2Length];
            rawDeck = new int[array3Length];
            hand = new CardStruct[array4Length];
            units = new CardStruct[array5Length];
        }

        //serialize elements
        for (int i = 0; i < array1Length; i++)
        {
            serializer.SerializeValue(ref name[i]);
        }

        for (int i = 0; i < array2Length; i++)
        {
            serializer.SerializeValue(ref deck[i]);
        }

        for (int i = 0; i < array3Length; i++)
        {
            serializer.SerializeValue(ref rawDeck[i]);
        }

        for (int i = 0; i < array4Length; i++)
        {
            serializer.SerializeValue(ref hand[i]);
        }
        
        for (int i = 0; i < array5Length; i++)
        {
            serializer.SerializeValue(ref units[i]);
        }
    }
}
/*

        // //ARRAYS
        // int array1Length = 0;
        // int array2Length = 0;
        // int array3Length = 0;
        // int array4Length = 0;

        // int[] array1;
        // int[] array2;
        // int[] array3;
        // int[] array4;

        // if (serializer.IsWriter)
        // {
        //     array1 = refArray1;
        //     array2 = refArray2;
        //     array3 = refArray3;
        //     array4 = refArray4;
        //     array1Length = array1.Length;
        //     array2Length = array2.Length;
        //     array3Length = array3.Length;
        //     array4Length = array4.Length;
        // }
        // else
        // {
        //     array1 = new int[array1Length];
        //     array2 = new int[array2Length];
        //     array3 = new int[array3Length];
        //     array4 = new int[array4Length];
        // }

        // serializer.SerializeValue(ref array1Length);
        // serializer.SerializeValue(ref array2Length);
        // serializer.SerializeValue(ref array3Length);
        // serializer.SerializeValue(ref array4Length);

        // serializer.SerializeValue(ref array1);
        // serializer.SerializeValue(ref array2);
        // serializer.SerializeValue(ref array3);
        // serializer.SerializeValue(ref array4);

        // if (serializer.IsReader)
        // {
        //     refArray1 = array1;
        //     refArray2 = array2;
        //     refArray3 = array3;
        //     refArray4 = array4;
        // }

    }

}
/*
        //ARRAYS

// Length
    int array1Length = 0;
    int array2Length = 0;
    int array3Length = 0;
    int array4Length = 0;
    int array5Length = 0;

    if (!serializer.IsReader)
    {
        if (array1 != null) array1Length = array1.Length;
        if (array2 != null) array2Length = array2.Length;
        if (array3 != null) array3Length = array3.Length;
        if (array4 != null) array4Length = array4.Length;
        if (array5 != null) array5Length = array5.Length;

    }

    if (array1 != null) serializer.SerializeValue(ref array1Length);
    if (array2 != null) serializer.SerializeValue(ref array2Length);
    if (array3 != null) serializer.SerializeValue(ref array3Length);
    if (array4 != null) serializer.SerializeValue(ref array4Length);
    if (array5 != null) serializer.SerializeValue(ref array5Length);


// Array
    if (serializer.IsReader)
    {
        if (array1 != null) array1 = new Vector3[array1Length];
        if (array2 != null) array2 = new int[array2Length];
        if (array3 != null) array3 = new int[array3Length];
        if (array4 != null) array4 = new int[array4Length];
        if (array5 != null) array5 = new bool[array5Length];
    }

    if (array1 != null) 
    {
        for (int n = 0; n < array1Length; ++n)
        {
            serializer.SerializeValue(ref array1[n]);
        }
    }

    if (array2 != null) 
    {
        for (int n = 0; n < array2Length; ++n)
        {
            serializer.SerializeValue(ref array2[n]);
        }
    }

    if (array3 != null) 
    {
        for (int n = 0; n < array3Length; ++n)
        {
            serializer.SerializeValue(ref array3[n]);
        }
    }

    if (array4 != null) 
    {
        for (int n = 0; n < array4Length; ++n)
        {
            serializer.SerializeValue(ref array4[n]);
        }
    }

    if (array5 != null) 
    {
        for (int n = 0; n < array5Length; ++n)
        {
            serializer.SerializeValue(ref array5[n]);
        }
    }










        //ARRAYS

        int array1Length = 0;
        int array2Length = 0;
        int array3Length = 0;
        int array4Length = 0;
        int array5Length = 0;

        int[] array1;
        int[] array2;
        int[] array3;
        int[] array4;
        int[] array5;

        if (serializer.IsWriter)
        {
            array1 = refArray1;
            array2 = refArray2;
            array3 = refArray3;
            array4 = refArray4;
            array5 = refArray5;
            array1Length = array1.Length;
            array2Length = array2.Length;
            array3Length = array3.Length;
            array4Length = array4.Length;
            array5Length = array5.Length;
        }
        else
        {
            array1 = new int[array1Length];
            array2 = new int[array2Length];
            array3 = new int[array3Length];
            array4 = new int[array4Length];
            array5 = new int[array5Length];
        }

        serializer.SerializeValue(ref array1Length);
        serializer.SerializeValue(ref array2Length);
        serializer.SerializeValue(ref array3Length);
        serializer.SerializeValue(ref array4Length);
        serializer.SerializeValue(ref array5Length);

        serializer.SerializeValue(ref array1);
        serializer.SerializeValue(ref array2);
        serializer.SerializeValue(ref array3);
        serializer.SerializeValue(ref array4);
        serializer.SerializeValue(ref array5);

        if (serializer.IsReader)
        {
            refArray1 = array1;
            refArray2 = array2;
            refArray3 = array3;
            refArray4 = array4;
            refArray5 = array5;
        }



        //ARRAYS 

        //serialize length

        int array1Length = refArray1 != null ? refArray1.Length : 0;
        int array2Length = refArray2 != null ? refArray2.Length : 0;
        int array3Length = refArray3 != null ? refArray3.Length : 0;
        int array4Length = refArray4 != null ? refArray4.Length : 0;
        int array5Length = refArray5 != null ? refArray5.Length : 0;

        serializer.SerializeValue(ref array1Length);
        serializer.SerializeValue(ref array2Length);
        serializer.SerializeValue(ref array3Length);
        serializer.SerializeValue(ref array4Length);
        serializer.SerializeValue(ref array5Length);

        //allocate array
        if (serializer.IsReader)
        {
            refArray1 = new int[array1Length];
            refArray2 = new int[array2Length];
            refArray3 = new int[array3Length];
            refArray4 = new int[array4Length];
            refArray5 = new int[array5Length];
        }

        // serialize elements
        for (int i = 0; i < array1Length; i++)
        {
            serializer.SerializeValue(ref refArray1[i]);
        }

        for (int i = 0; i < array2Length; i++)
        {
            serializer.SerializeValue(ref refArray2[i]);
        }

        for (int i = 0; i < array3Length; i++)
        {
            serializer.SerializeValue(ref refArray3[i]);
        }

        for (int i = 0; i < array4Length; i++)
        {
            serializer.SerializeValue(ref refArray4[i]);
        }

        for (int i = 0; i < array5Length; i++)
        {
            serializer.SerializeValue(ref refArray5[i]);
        }

    */
