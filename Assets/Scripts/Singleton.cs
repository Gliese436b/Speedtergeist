using UnityEngine;

public class Singleton<T>:MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// Singleton que me robé de programación 3
    /// Basicamente se usa como padre para la clase que se le quiere implementar el patron.
    /// </summary>
    /// <typeparam name="T"> Nombre de la clase, Ej.: GameManager : Singleton<GameManager> </typeparam>

    public static T Instance { get; private set; }
    public bool dontdestroyOnLoad;

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("A instance already exists");
            Destroy(this); //Or GameObject as appropriate
            return;
        }
        Instance = this as T;
        if (dontdestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }

}
