using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfiguration/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{
	public byte enemyCount;
	public float globalAnimTime;
	public byte playerMoveSpeed;
	public float playerAnimTime;
	public float enemyAnimTime;
	public Font fontTextLegacy;
	public TMP_FontAsset fontTMP;
	public byte gameLevel;
	public LootTable chessPossibility;
}
