using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDefine
{
    public static bool _isDeveloperBuild = true;
}

public enum EventID
{
    OnTimeCount,
    OnEnemyDecrease,
    OnEnemyIncrease,
    OnPlayerMove,
    OnPlayerKill,
    OnScoreCount,
    OnKillStreak,
    OnFirstBlood,
    OnOneHundredPoint,
    OnThreeHundredPoint,
    OnFiveHundredPoint,
    OnOneThousandPoint,
    OnOneThousandFiveHundredPoint,
    OnTwoThousandPoint,
    OnThreeThousandPoint,
    OnShowMeMore,
    Unkown = 404
}

public enum MoveType
{
    Pawn = 1,
    Knight,
    Bishop,
    Rook,
    Queen,
    King = 6
}

public enum ANNOUNCE
{
    FIRST_BLOOD,
    DOUBLE_KILL,
    TRIPLE_KILL,
    ULTRA_KILL,
    RAMPAGE,
    KILLING_SPREE,
    MEGA_KILL,
    MONSTER_KILL,
    DOMINATING,
    UNSTOPPABLE,
    WICKED_SICK,
    GOD_LIKE,
    HOLY_SHEET,
    SHOW_ME_MORE,
    NONE = 404
}



