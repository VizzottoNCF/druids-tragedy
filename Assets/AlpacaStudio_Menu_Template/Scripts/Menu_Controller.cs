using UnityEngine;
using UnityEngine.UI;

public class Menu_Controller : MonoBehaviour 
{

	[Header("Buttons Settings")]
	public string _sceneToLoadOnPlay = "Level";
	public Button[] _soundButtons;
	public AudioClip _audioClip;
	public AudioSource _audioSource;
	public GameObject _DeckUI;

	[Header("Card/Hero Config")]
	public HeroData _heroData;
	
	//The private variable 'scene' defined below is used for example/development purposes.
	//It is used in correlation with the Escape_Menu script to return to last scene on key press.
	UnityEngine.SceneManagement.Scene scene;

	void Awake () 
	{
		if(!PlayerPrefs.HasKey("_Mute"))
		{
			PlayerPrefs.SetInt("_Mute", 0);
		}

		Mute();
		scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
		PlayerPrefs.SetString("_LastScene", scene.name.ToString()); 
		//Debug.Log(scene.name);
	}	
	public void PlayGame () 
	{
		//_audioSource.PlayOneShot(_audioClip);
		PlayerPrefs.SetString("_LastScene", scene.name);
		UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneToLoadOnPlay);
	}	
	public void Mute () 
	{
		//_audioSource.PlayOneShot(_audioClip);
		_soundButtons[0].interactable = true;
		_soundButtons[1].interactable = false;
		PlayerPrefs.SetInt("_Mute", 1);
	}	
	public void Unmute () 
	{
		//_audioSource.PlayOneShot(_audioClip);
		_soundButtons[0].interactable = false;
		_soundButtons[1].interactable = true;
		PlayerPrefs.SetInt("_Mute", 0);
	}	
	public void QuitGame () 
	{
		_audioSource.PlayOneShot(_audioClip);
		#if !UNITY_EDITOR
			Application.Quit();
		#endif
		
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}
	public void OpenSettings()
	{

	}
	public void CheckDeck()
	{
		if (_DeckUI.transform.childCount == 0) { SpawnDeck(); }

		
		_DeckUI.SetActive(!_DeckUI.activeInHierarchy);
	}
	private void SpawnDeck()
	{
        CardViewCreator service = ServiceLocator.Get<CardViewCreator>();

		foreach (var card in _heroData.Deck)
		{
			Card c = new Card(card);

            GameObject g = service.CreateCardViewUI(c, _DeckUI.transform).gameObject;
			g.transform.parent = _DeckUI.transform;

        }

    }
}
