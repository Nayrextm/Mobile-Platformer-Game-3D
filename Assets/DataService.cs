//using SQLite4Unity3d;
//using UnityEngine;
//#if !UNITY_EDITOR
//using System.Collections;
//using System.IO;
//#endif
//using System.Collections.Generic;

//public class DataService  {

//	private SQLiteConnection _connection;

//	public DataService(string DatabaseName){

//#if UNITY_EDITOR
//            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
//#else
//        // check if file exists in Application.persistentDataPath
//        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

//        if (!File.Exists(filepath))
//        {
//            Debug.Log("Database not in Persistent path");
//            // if it doesn't ->
//            // open StreamingAssets directory and load the db ->

//#if UNITY_ANDROID 
//            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
//            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
//            // then save to Application.persistentDataPath
//            File.WriteAllBytes(filepath, loadDb.bytes);
//#elif UNITY_IOS
//                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
//                // then save to Application.persistentDataPath
//                File.Copy(loadDb, filepath);
//#elif UNITY_WP8
//                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
//                // then save to Application.persistentDataPath
//                File.Copy(loadDb, filepath);

//#elif UNITY_WINRT
//		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
//		// then save to Application.persistentDataPath
//		File.Copy(loadDb, filepath);

//#elif UNITY_STANDALONE_OSX
//		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
//		// then save to Application.persistentDataPath
//		File.Copy(loadDb, filepath);
//#else
//	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
//	// then save to Application.persistentDataPath
//	File.Copy(loadDb, filepath);

//#endif

//            Debug.Log("Database written");
//        }

//        var dbPath = filepath;
//#endif
//            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
//        Debug.Log("Final PATH: " + dbPath);     

//	}

//	public void CreateDB(){
//		_connection.DropTable<Person> ();
//		_connection.CreateTable<Person> ();

//		_connection.InsertAll (new[]{
//			new Person{
//				Id = 1,
//				Name = "Tom",
//				Surname = "Perez",
//				Age = 56
//			},
//			new Person{
//				Id = 2,
//				Name = "Fred",
//				Surname = "Arthurson",
//				Age = 16
//			},
//			new Person{
//				Id = 3,
//				Name = "John",
//				Surname = "Doe",
//				Age = 25
//			},
//			new Person{
//				Id = 4,
//				Name = "Roberto",
//				Surname = "Huertas",
//				Age = 37
//			}
//		});
//	}

//	public IEnumerable<Person> GetPersons(){
//		return _connection.Table<Person>();
//	}

//	public IEnumerable<Person> GetPersonsNamedRoberto(){
//		return _connection.Table<Person>().Where(x => x.Name == "Roberto");
//	}

//	public Person GetJohnny(){
//		return _connection.Table<Person>().Where(x => x.Name == "Johnny").FirstOrDefault();
//	}

//	public Person CreatePerson(){
//		var p = new Person{
//				Name = "Johnny",
//				Surname = "Mnemonic",
//				Age = 21
//		};
//		_connection.Insert (p);
//		return p;
//	}
//}


using SQLite4Unity3d;
using UnityEngine;
using System.Collections.Generic;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
//Покращена версія без WWW



// Додаємо бібліотеку для роботи з мережею/файлами APK
using UnityEngine.Networking;

public class DataService
{
    private SQLiteConnection _connection;

    public DataService(string DatabaseName)
    {
#if UNITY_EDITOR
        var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // Перевіряємо, чи існує файл у Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // Якщо ні -> відкриваємо StreamingAssets і копіюємо базу ->

#if UNITY_ANDROID 
            // СУЧАСНИЙ ПІДХІД ЗАМІСТЬ WWW
            string loadDbPath = "jar:file://" + Application.dataPath + "!/assets/" + DatabaseName;
            
            using (UnityWebRequest request = UnityWebRequest.Get(loadDbPath))
            {
                var operation = request.SendWebRequest();
                
                // Це синхронне очікування (блокує гру на долю секунди). 
                // Для конструктора це єдиний вихід.
                while (!operation.isDone) { } 

                if (request.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllBytes(filepath, request.downloadHandler.data);
                    Debug.Log("Database successfully extracted via UnityWebRequest");
                }
                else
                {
                    Debug.LogError("Error extracting database: " + request.error);
                }
            }
#elif UNITY_IOS
            var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  
            File.Copy(loadDb, filepath);
#elif UNITY_WP8
            var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  
            File.Copy(loadDb, filepath);
#elif UNITY_WINRT
            var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  
            File.Copy(loadDb, filepath);
#elif UNITY_STANDALONE_OSX
            var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  
            File.Copy(loadDb, filepath);
#else
            var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  
            File.Copy(loadDb, filepath);
#endif
            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);
    }

    // ... (Далі йдуть ваші методи CreateDB, GetPersons тощо без змін)
    public void CreateDB()
    {
        _connection.DropTable<Person>();
        _connection.CreateTable<Person>();

        _connection.InsertAll(new[]{
            new Person{ Id = 1, Name = "Tom", Surname = "Perez", Age = 56 },
            new Person{ Id = 2, Name = "Fred", Surname = "Arthurson", Age = 16 },
            new Person{ Id = 3, Name = "John", Surname = "Doe", Age = 25 },
            new Person{ Id = 4, Name = "Roberto", Surname = "Huertas", Age = 37 }
        });
    }

    public IEnumerable<Person> GetPersons()
    {
        return _connection.Table<Person>();
    }

    public IEnumerable<Person> GetPersonsNamedRoberto()
    {
        return _connection.Table<Person>().Where(x => x.Name == "Roberto");
    }

    public Person GetJohnny()
    {
        return _connection.Table<Person>().Where(x => x.Name == "Johnny").FirstOrDefault();
    }

    public Person CreatePerson()
    {
        var p = new Person { Name = "Johnny", Surname = "Mnemonic", Age = 21 };
        _connection.Insert(p);
        return p;
    }
}