using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
	[SerializeField] private ZoneType _type = ZoneType.Default;
	[SerializeField] private List<PlayerStats> _playersInZone = new List<PlayerStats>(2);
	[SerializeField] private List<GameObject> _objs = new List<GameObject>();
	[SerializeField] private bool _isActivateOneTime;
	[SerializeField] private Quest _quest;
	[SerializeField] private Item _item;
	[SerializeField] private Chest DEBUGChest;
	private bool _isActiveted;
	public int CountPlayersInZone => _playersInZone.Count;
	[SerializeField] private UnityEvent onTriggerExe;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			if (_isActivateOneTime)
			{
				if (!_isActiveted)
				{
					_isActiveted = true;
					ActivateTrigger(other.gameObject);
				}
				else return;
			}
			else 
			{
				_isActiveted = true;
				ActivateTrigger(other.gameObject);
			}
		}
	}
	private void ActivateTrigger(GameObject player)
	{
		PlayerStats playerStats = player.gameObject.GetComponent<PlayerStats>();
		AddPlayerToZone(playerStats);
		if (_type == ZoneType.HasPlayers)
		{
			foreach (PlayerStats playerSt in _playersInZone)
			{
				playerSt.gameObject.transform.position = _objs[0].transform.position;
			}
		}
		if (_type == ZoneType.Exit)
		{
			if (_playersInZone.Count >= 2 || PhotonNetwork.offlineMode)
			{
				GUIControl.Instance.ExitButton.SetActive(true);
			}
		}
		if (_type == ZoneType.DestroyWithDelay)
		{
			StartCoroutine(DestroyDelay());
		}
		if (_type == ZoneType.AddQuest)
		{
			if (_quest != null)
			{
				PlayerQuest playerQuest = player.gameObject.GetComponent<PlayerQuest>();
				playerQuest.AddActiveQuest(_quest);
			}
			else { Debug.Log("Quest is null"); }
		}
		if (_type == ZoneType.SpawnItemInRandomChest)
		{
			DungeonObjects dungeonObjects = DungeonObjects.Instance;
			Chest chest = dungeonObjects.ReturnRandomClosedChest();

			if (chest != null)
			{ chest.AddItemToChest(_item); DEBUGChest = chest; }
			else
			{
				Debug.Log("Not enought closed chests");
				if (_item != null) player.gameObject.GetComponent<Inventory>().AddItems(_item, 1);
				else Debug.Log("Item equals null");

			}
		}
		if (_type == ZoneType.CheckContainItem)
		{
			if (player.GetComponent<Inventory>().GetCountItemsWithId(_item.Id) >= 1)
			{
				foreach (GameObject obj in _objs)
				{
					if (obj.GetComponent<DoorBlock>() != null)
					{
						obj.GetComponent<DoorBlock>().SwitchDoor(false);
					}
				}
			}
		}

		onTriggerExe.Invoke();
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			if (_type == ZoneType.Exit)
			{
				GUIControl.Instance.ExitButton.SetActive(false);
			}
			RemovePlayerFromZone(other.GetComponent<PlayerStats>());
		}
	}
	private void RemovePlayerFromZone(PlayerStats playerStats)
	{
		if (_playersInZone.Contains(playerStats)) _playersInZone.Remove(playerStats);
	}
	private void AddPlayerToZone(PlayerStats playerStats)
	{
		if (_playersInZone.Contains(playerStats)) return;
		else
		{
			_playersInZone.Add(playerStats);
		}
	}
	private IEnumerator DestroyDelay()
	{
		yield return new WaitForSecondsRealtime(1f);
		{
			foreach (GameObject obj in _objs)
			{
				Destroy(obj);
			}

		}
	}
}
public enum ZoneType
{
	Default,
	Exit,
	DestroyWithDelay, // уничножение объекта с задержкой
	AddQuest, // выдача квеста
	CheckContainItem, // проверка наличия предмета у игрока 
	SpawnItemInRandomChest,
	HasPlayers,
}
