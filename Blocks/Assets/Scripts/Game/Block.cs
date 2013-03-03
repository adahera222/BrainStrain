using UnityEngine;
using System;
using System.Collections;

public enum State
{
	Unsolved,
	Solved,
	Wrong
}

public class Block : MonoBehaviour
{	
	public Texture2D[] Textures;
	public Color[] Palete;
	
	public int Number{ get; set; }
	public int Id{ get; set; }
	public bool IsDiggit
	{
		get
		{
			return Number != 0;
		}
	}
	public State State
	{ 
		get
		{
			return _state;
		}
		set
		{
			_state = value;
			_material.mainTexture = Textures[Number];
			_material.color = Palete[(int)_state];
			_colider.enabled = (_state == State.Unsolved);
		}
	}
	public bool Marked
	{
		get
		{
			return _marked;
		}
		set
		{
			_marked = value;
		}
	}
	public bool Inspected
	{
		get
		{
			return _inspected;
		}
		set
		{
			_inspected = value;
			
			if(!IsDiggit)
			{
				if(State == State.Unsolved)
				{
					if(_inspected)
					{
						var col = _material.color;
						col.a = 0.5f;
						_material.color = col;
						_colider.enabled = false;
					}
					else
					{
						var col = _material.color;
						col.a = 1f;
						_material.color = col;
						_colider.enabled = true;
					}
				}
			}
		}
	}
	
	private Material _material;
	private Collider _colider;
	private State _state;
	private bool _marked;
	private bool _inspected;
	
	private bool pressed;
	private float time;
	
	void Awake()
	{
		_material = GetComponent<Renderer>().material;
		_material.mainTexture = Textures[Number];
		_colider = GetComponent<Collider>();
	}
	
	void Start()
	{
		State = State.Unsolved;
	}
	
	// Update is called once per frame
	void Update()
	{
		if(Marked && _material.mainTexture != Textures[Textures.Length - 1])
		{
			_material.mainTexture = Textures[Textures.Length - 1];
		}
		else if(!Marked && _material.mainTexture != Textures[Number])
		{
			_material.mainTexture = Textures[Number];
		}
	}
	
	void OnMouseUpAsButton()
	{
		if(State == State.Unsolved)
		{
			if(!IsDiggit && !Inspected)
			{
				var obj = GameObject.Find("Main Camera").GetComponent<Spawner>();
				if(obj != null)
				{
					obj.SendMessage("BlockPressed", Id);
				}
			}
		}
	}
}
