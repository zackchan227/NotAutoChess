using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfiguration/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{
	public byte enemyCount; 
}