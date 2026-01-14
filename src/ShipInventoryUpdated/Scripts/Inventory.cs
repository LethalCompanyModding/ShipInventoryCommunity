using ShipInventoryUpdated.Helpers;
using ShipInventoryUpdated.Objects;
using Unity.Netcode;

namespace ShipInventoryUpdated.Scripts;

public class Inventory : NetworkBehaviour
{
	private static Inventory? _instance;

	private readonly NetworkList<ItemData> _storedItems = new(
		null,
		NetworkVariableReadPermission.Everyone,
		NetworkVariableWritePermission.Owner
	);

	#region API

	public delegate void OnListChanged(ItemData[] value);

	public static OnListChanged? OnAdded;
	public static OnListChanged? OnRemoved;

	/// <summary>
	/// Adds the given items to the inventory
	/// </summary>
	public static void Add(ItemData[] items)
	{
		if (_instance == null)
		{
			Logger.Warn("Tried to add an item to the inventory, but no instance was defined.");
			return;
		}

		_instance.AddServerRpc(items);
	}

	/// <summary>
	/// Removes the given items from the inventory
	/// </summary>
	/// <param name="items"></param>
	public static void Remove(ItemData[] items)
	{
		if (_instance == null)
		{
			Logger.Warn("Tried to remove an item to the inventory, but no instance was defined.");
			return;
		}

		_instance.RemoveServerRpc(items);
	}

	/// <summary>
	/// Clears the items stored in the inventory
	/// </summary>
	public static void Clear()
	{
		if (_instance == null)
		{
			Logger.Warn("Tried to remove an item to the inventory, but no instance was defined.");
			return;
		}

		_instance.ClearServerRpc();
	}

	/// <summary>
	/// Gets the number of items stored in the inventory
	/// </summary>
	public static int Count => _instance?._storedItems.Count ?? 0;

	/// <summary>
	/// Gets the items stored in the inventory
	/// </summary>
	public static ItemData[] Items
	{
		get
		{
			if (_instance == null)
				return [];

			var items = new ItemData[Count];

			for (var i = 0; i < items.Length; i++)
				items[i] = _instance._storedItems[i];

			return items;
		}
	}

	/// <summary>
	/// Marks the items as they persisted through rounds
	/// </summary>
	public static void MarkPersisted(ItemData[] items)
	{
		if (_instance == null)
		{
			Logger.Warn("Tried to modify items in the inventory, but no instance was defined.");
			return;
		}

		_instance.MarkPersistedServerRpc(items);
	}

	#endregion

	#region Unity

	/// <inheritdoc/>
	public override void OnNetworkSpawn()
	{
		_instance = this;
	}

	/// <inheritdoc/>
	public override void OnNetworkDespawn()
	{
		if (_instance == this)
			_instance = null;
	}

	#endregion

	#region RPC

	[ServerRpc(RequireOwnership = false)]
	private void AddServerRpc(params ItemData[] items)
	{
		foreach (var item in items)
			_storedItems.Add(item);

		OnAdded?.Invoke(items);
	}

	[ServerRpc(RequireOwnership = false)]
	private void RemoveServerRpc(params ItemData[] items)
	{
		foreach (var item in items)
			_storedItems.Remove(item);

		OnRemoved?.Invoke(items);
	}

	[ServerRpc(RequireOwnership = false)]
	private void ClearServerRpc()
	{
		_storedItems.Clear();
	}

	[ServerRpc(RequireOwnership = false)]
	private void MarkPersistedServerRpc(params ItemData[] items)
	{
		var newItems = new HashSet<ItemData>(items);

		for (var i = 0; i < _storedItems.Count; i++)
		{
			var item = _storedItems[i];
			
			if (!newItems.Contains(item))
				continue;

			item.PERSISTED_THROUGH_ROUNDS = true;
			_storedItems[i] = item;
		}
	}

	#endregion
}
