using UnityEngine;
using System.Collections;

public class BaseGUI : MonoBehaviour
{
	public GUISkin Skin;
	public Texture2D[] Images;
	
	protected enum Textures
	{
		Restart,
		Menu,
		Undo,
		Block,
		Mark,
		Inspect,
		Back,
		Next,
		StarFull,
		StarEmpty,
		Settings,
		Play,
		Credits
	}
	
	protected int w;
	protected int w_2;
	protected int h;
	protected int h_2;
	protected Vector2 center;
	protected int offset;
	protected int big_offset;
	protected int button_w;
	protected int button_h;
	protected int button_size;
	
	// Use this for initialization
	protected virtual void Start()
	{
		InitGUI();
	}
	
	protected void InitGUI()
	{
		w = Screen.width;
		h = Screen.height;
		w_2 = (int)(w / 2);
		h_2 = (int)(h / 2);
		center = new Vector2(w_2, h_2);
		offset = (int)Mathf.Min(0.0078125f * w, 0.0078125f * h);
		
		button_w = (int)(w / 8f);
		button_h = (int)(h / 8f);
		button_size = (int)((button_w + button_h) / 2f);//Mathf.Min((int)(w / 8f), (int)(h / 8f));
		
		big_offset = button_size / 3;
	}
	
	// Update is called once per frame
	protected virtual void Update()
	{
		InitGUI();
	}
	
	protected virtual void OnGUI()
	{
		GUI.skin = Skin;
	}
}
