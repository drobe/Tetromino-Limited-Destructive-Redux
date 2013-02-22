using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is a temporary declaration of the Core Entity class. This is to make the Core happy. Be happy, Core, be happy.
public class CoreEntity{public void update(float delta){}};

// A core is a boss-type quasi-singleton entity that overrides the Unity MonoBehavior Update scheme.
// Note, this is 'like a singleton' because it should exist only once, but nothing is stopping you from creating several Cores. You have three Cores for all I care, but you'll only be able to communicate with one using the GetInstance() function.
public class Core : MonoBehaviour 
{	
	#region Attributes
	[System.Serializable]
	public class TimedUpdateProperties
	{
		float sinceLast_AsleepUpdate;
		float sinceLast_LowUpdate;
		float sinceLast_NormalUpdate;
		float sinceLast_HighUpdate;
		float sinceLast_RealtimeUpdate;
	
		public float AsleepInterval;
		public float LowInterval;
		public float NormalInterval;
		public float HighInterval;
		public float RealtimeInterval;
	};
	
	[System.Serializable]
	public class CountedUpdateProperties
	{
		int AsleepUpdates;
		int LowUpdates;
		int NormalUpdates;
		int HighUpdates;
		int RealtimeUpdates;
		
		public int AsleepInterval;
		public int LowInterval;
		public int NormalInterval;
		public int HighInterval;
		public int RealtimeInterval;
	};
	
	private class UpdateQueue
	{
		void SetQueue(uint queueSize)
		{
		}
		void GetQueue()
		{
		}
		void AddToQueue(CoreEntity entity)
		{
		}
	}
	
	public enum  UpdateType
	{
		Timed_Interval 		= 0,
		Counted_Interval 	= 1
	};
	
	// Object pool related attributes
	public int PoolSize = 1000;
	static CoreEntity[] ObjectPool;
	
	// Update Scheme
	// An empty method prototype for a delegate method
	delegate void UpdateFunction(float delta); 
	UpdateFunction updateFunction;
	public UpdateType UpdateScheme = UpdateType.Timed_Interval;
	
	public TimedUpdateProperties timedUpdateProperties 		= new TimedUpdateProperties();
	public CountedUpdateProperties countedUpdateProperties 	= new CountedUpdateProperties();
	
	// Update Pools
	static List<CoreEntity> AsleepPool;
	static List<CoreEntity> LowPool;
	static List<CoreEntity> NormalPool;
	static List<CoreEntity> HighPool;
	static List<CoreEntity> RealtimePool;
	#endregion
	#region Unity Start and Awake
	// Runs first, always.
	void Awake ()
	{
		// The core must survive.
		DontDestroyOnLoad(this.gameObject);
		Instance = this;
		
		// Create pools
		ObjectPool 	= new CoreEntity[PoolSize];
		AsleepPool 	= new List<CoreEntity>();
		LowPool 	= new List<CoreEntity>();
		NormalPool 	= new List<CoreEntity>();
		HighPool 	= new List<CoreEntity>();
		RealtimePool= new List<CoreEntity>();
		
		// Setup update scheme
		if (UpdateScheme == UpdateType.Counted_Interval)
		{
			updateFunction = Core.DoCountedUpdate;
		}
		else if (UpdateScheme == UpdateType.Timed_Interval)
		{
			updateFunction = Core.DoTimedUpdate;
		}
	}
	
	// Runs before first update.
	void Start () 
	{
	
	}
	#endregion
	#region GetInstance
	// There can only be one.
	static Core Instance = null;
	static public Core GetInstance()
	{
		// Because a Core is a monobehavior, it shouldn't ever be constructed manually.
		// We will trust that a Core is created long before anything tries talking it.
		return Instance;
	}
	#endregion
	#region update
	// Update is called once per frame
	void Update () 
	{
		float delta = Time.deltaTime;
		updateFunction(delta);
	}
	
	// Pooled update functions.
	static void DoTimedUpdate(float delta)
	{
		foreach(CoreEntity entity in RealtimePool)
		{
			entity.update(delta);
		}
	}
	
	static void DoCountedUpdate(float delta)
	{			
	}
	#endregion
}
