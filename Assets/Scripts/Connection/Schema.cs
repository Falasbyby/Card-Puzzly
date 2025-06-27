using System;
using UnityEngine.Serialization;

namespace Messages
{
    public enum MessageFromReact
    {
        Init = 1000,
        AppliedBoosts = 1001,
        Continue = 1002
    }

    public enum MessageFromUnity
    {
        BoostGame = 2000,
        Lose = 2001,
        Finish = 2002,

        BackButton = 2003,
        GameIdNotMetch = 2404,
        Cheat = 2500,
    }

    [Serializable]
    public struct InitMessage
    {
        public string game_id;
    }

    public enum FixisRarity
    {
        Common,
        Legendary,
        Rare
    }
    public enum CharacterType
    {
        Krosh = 0,
        Nyusha = 1,
        Barash = 2,
        Ezhik = 3,
        Pin = 4,
        Kopatych = 5,
        KarKarych = 6,
        Losyash = 7,
        Sovunya = 8,
        Bibi = 9
    }


    [Serializable]
    public struct AppliedBoostsMessage
    {
        public string game_id;
        /* public Boost[] boosts;

        [Serializable]
        public struct Boost
        {
            public string type;
            public int index;
            public bool enabled;
        } */
    }

    [Serializable]
    public struct GameIdMessage
    {
        public string game_id;
    }

    [Serializable]
    public struct NFT
    {
        public bool activeNFT;
        public FixisRarity rarity;
    }

    [Serializable]
    public struct ContinueMessage
    {
        public string game_id;
    }

    [Serializable]
    public struct RestartSceneMessage
    {
        public string game_id;
    }

    [Serializable]
    public struct BoostGameMessage
    {
        public string game_id;
    }

    [Serializable]
    public struct LoseMessage
    {
        public string game_id;
        public int points;
        public string time;
    }

    [Serializable]
    public struct FinishMessage
    {
        public string game_id;
        public int points;
    }

    [Serializable]
    public struct CheatMessage
    {
        public string game_id;
    }


    [Serializable]
    public struct ErrorMessage
    {
        public string code;
        public string message;
        public GameIdMismatchData data;

        [Serializable]
        public struct GameIdMismatchData
        {
            public string game_id;
        }
    }
}