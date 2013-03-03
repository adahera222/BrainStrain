using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

enum MenuState
{
	Main,
	Settings,
	Credits,
	Worlds,
	Levels,
}

public class MainMenuGUI : BaseGUI
{
	private Global _global;
	private Vector2 scrollWorlds;
	private Vector2 scrollLevels;
	private MenuState state;
	private World currentWorld;

	// Use this for initialization
	protected override void Start()
	{
		base.Start();
		
		GetComponent<CameraControl>().State = CameraControl.CameraState.MainMenu;
		_global = GameObject.Find("Global").GetComponent<Global>();
		if(_global.CurrentLevel.Number == 1 && _global.CurrentWorld.Number == 1)
		{
			state = MenuState.Main;
		}
		else
		{
			state = MenuState.Levels;
			currentWorld = _global.CurrentWorld;
		}
	}
	
	// Update is called once per frame
	protected override void Update()
	{
		base.Update();
		#if UNITY_ANDROID || UNITY_IOS
		//this is for the scrollers
		if(Input.touchCount > 0)
		{	
			scrollWorlds.x -= Input.GetTouch(0).deltaPosition.x;
			scrollWorlds.y += Input.GetTouch(0).deltaPosition.y;
			
			scrollLevels.x -= Input.GetTouch(0).deltaPosition.x;
			scrollLevels.y += Input.GetTouch(0).deltaPosition.y;
		}
		#endif
	}
	
	protected override void OnGUI()
	{
		base.OnGUI();
		
		switch (state)
		{
			case MenuState.Main:
				if(Input.GetKeyDown(KeyCode.Escape))
				{
					Application.Quit();
				}
				MainMenu();
				break;
			case MenuState.Settings:
				SettingsMenu();
				break;
			case MenuState.Credits:
				CreditsMenu();
				break;
			case MenuState.Worlds:
				WorldMenu();
				break;
			case MenuState.Levels:
				LevelMenu();
				break;
		}
	}
	
	private void MainMenu()
	{
		GUILayout.BeginArea(new Rect(0, 0, w, h));
			GUILayout.BeginVertical();
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal();
					if(GUILayout.Button(Images[(int)Textures.Settings], GUILayout.Width(button_size), GUILayout.Height(button_size)))
					{
						state = MenuState.Settings;
					}
					GUILayout.FlexibleSpace();
					if(GUILayout.Button(Localization.Get("play"), GUILayout.MaxWidth(w_2 / 3f), GUILayout.Height(button_size)))
					{
						state = MenuState.Worlds;
					}
					GUILayout.FlexibleSpace();
					if(GUILayout.Button(Images[(int)Textures.Credits], GUILayout.Width(button_size), GUILayout.Height(button_size)))
					{
						state = MenuState.Credits;
					}
				GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	private void SettingsMenu()
	{
		var bg = Resources.Load("Localization/BG", typeof(Texture)) as Texture;
		var en = Resources.Load("Localization/EN", typeof(Texture)) as Texture;
		
		if(GUILayout.Button(Images[(int)Textures.Back], GUILayout.Width(button_w), GUILayout.Height(button_h)))
		{
			state = MenuState.Main;
		}
		
		GUILayout.BeginArea(new Rect(0, 0, w, h));
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.Box(Localization.Get("lang"));
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(en, GUILayout.Height(button_h)))
			{
				Localization.Language = SystemLanguage.English;
			}
			if(GUILayout.Button(bg, GUILayout.Height(button_h)))
			{
				Localization.Language = SystemLanguage.Bulgarian;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(button_h);
			if(GUILayout.Button(Localization.Get("cls"), GUILayout.Height(button_h)))
			{
				foreach(Level level in _global.Worlds.SelectMany(world => world.Levels))
				{
					level.Stars = 0;
					level.Completed = false;
				}
				if(File.Exists(Global.LevelsFile))
				{
					File.Delete(Global.LevelsFile);
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void CreditsMenu()
	{
		if(GUILayout.Button(Images[(int)Textures.Back], GUILayout.Width(button_w), GUILayout.Height(button_h)))
		{
			state = MenuState.Main;
		}
		
		GUILayout.BeginArea(new Rect(0, 0, w, h));
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical();
					GUILayout.FlexibleSpace();
					GUILayout.Box(Localization.Get("credits"));
					GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void WorldMenu()
	{
		GUILayout.BeginArea(new Rect(0, 0, w, h));
			GUILayout.BeginVertical();
				if(GUILayout.Button(Images[(int)Textures.Back], GUILayout.Width(button_w), GUILayout.Height(button_h)))
				{
					state = MenuState.Main;
				}
				GUILayout.BeginHorizontal();
					GUILayout.Space(big_offset);
					scrollWorlds = GUILayout.BeginScrollView(scrollWorlds, GUILayout.ExpandHeight(true));
						GUILayout.BeginHorizontal();
							foreach(World world in _global.Worlds)
							{
								if(WorldButton(world))
								{
									currentWorld = world;
									state = MenuState.Levels;
								}
							}
						GUILayout.EndHorizontal();
					GUILayout.EndScrollView();
					GUILayout.Space(big_offset);
				GUILayout.EndHorizontal();
				GUILayout.Space(big_offset);
			GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	private void LevelMenu()
	{
		GUILayout.BeginVertical();
			if(GUILayout.Button(Images[(int)Textures.Back], GUILayout.Width(button_w), GUILayout.Height(button_h)))
			{
				state = MenuState.Main;
			}
			scrollLevels = GUILayout.BeginScrollView(scrollLevels, GUILayout.Width(w));
				foreach(Level level in currentWorld.Levels)
				{
					if(LevelButton(level))
					{
						_global.CurrentLevel = level;
						Application.LoadLevel("Game");
					}
				}
			GUILayout.EndScrollView();
			GUILayout.Space(big_offset);
		GUILayout.EndVertical();
	}
	
	private bool LevelButton(Level level)
	{
		bool pressed;
		var w = GUILayout.Width(h_2 / 3f);
		var h = GUILayout.Height(h_2 / 3f);
		
		GUILayout.BeginHorizontal();
		var col = GUI.color;
		if(level.Completed)
			GUI.color = Color.green;
		else
			GUI.color = Color.gray;
		GUILayout.Box(level.Number.ToString(), w, h);
		GUI.color = col;
		
		GUILayout.Box(Localization.Get(level.Name), GUILayout.ExpandWidth(true), h);
		
		for(int i = 1; i <= 3; i++)
		{
			if(level.Stars >= i)
				GUILayout.Box(Images[(int)Textures.StarFull], w, h);
			else
				GUILayout.Box(Images[(int)Textures.StarEmpty], w, h);
		}
		pressed = GUILayout.Button(Images[(int)Textures.Play], w, h);
		GUILayout.EndHorizontal();
		return pressed;
	}
	
	private bool WorldButton(World world)
	{
		bool pressed;
		var w = GUILayout.Width(base.w / 3);
		var h = GUILayout.Height(button_h);
		var dificultyImage = Resources.Load("Dificulties/Difficulty" + world.Number, typeof(Texture)) as Texture;
		
		GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.Box(dificultyImage, w, GUILayout.Height(button_h * 3.5f));
			GUILayout.Box(new GUIContent(world.Levels.Sum(level => level.Stars) + "/" + world.Levels.Count * 3, Images[(int)Textures.StarFull]), w, h);
			pressed = GUILayout.Button(Localization.Get("world" + world.Number), w, h);
		GUILayout.EndVertical();
		return pressed;
	}
}