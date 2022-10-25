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
    Unkown = 404
}

public enum MoveType
{
    Pawn = 0,
    Knight,
    Bishop,
    Rook,
    Queen,
    King = 5
}



