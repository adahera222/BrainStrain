using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class BrainStrain : MonoBehaviour {
	
	public GameObject BlockPrefab;
	
	private Vector3 RotationCenter
	{
		get
		{
			var vect = new Vector3();
			var scale = BlockPrefab.transform.localScale;
			vect.x = (blocks.GetLength(2) * scale.z) / 2f - scale.z / 2f;
			vect.y = (blocks.GetLength(1) * scale.y) / 2f - scale.y / 2f;
			vect.z = (blocks.GetLength(0) * scale.x) / 2f - scale.x / 2f;
			return vect;
		}
	}
	
	private float MinDistance
	{
		get
		{
			var vect =  new Vector3();
			vect.x = blocks.GetLength(0);
			vect.y = blocks.GetLength(1);
			vect.z = blocks.GetLength(2);
			return vect.magnitude / 2f;
		}
	}
	
	private const byte _ = byte.MaxValue;
	private Block[,,] blocks;
	
	// Use this for initialization
	void Start()
	{
		var brainstrain = LoadBrainStrain();
		LoadLevel(brainstrain);
		
		var cameraControl = GameObject.Find("Main Camera").GetComponent<CameraControl>();
		if(cameraControl != null)
		{
			cameraControl.RotationCenter = this.RotationCenter;
			cameraControl.MinDistance = this.MinDistance + 1.5f;
			cameraControl.MaxDistance = cameraControl.MinDistance * 2.5f;
			cameraControl.RestartView();
		}
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	private byte[,,] LoadBrainStrain()
	{
		var result = new byte[25,9,31];
		for(var zz = 0; zz < result.GetLength(2); zz++)
		for(var yy = 0; yy < result.GetLength(1); yy++)
		for(var xx = 0; xx < result.GetLength(0); xx++)
		{
			result[xx,yy,zz] = _;
		}
		
		var jsonString = (Resources.Load("BrainStrain") as TextAsset).text;
		
		var list = Json.Deserialize(jsonString) as List<object>;
		foreach(var item in list)
		{
			var dict = item as Dictionary<string,object>;
			var x = (byte)(long)dict["x"];
			var y = (byte)(long)dict["y"];
			var z = (byte)(long)dict["z"];
			result[x,y,z] = 0;
		}
		Resources.UnloadUnusedAssets();
		return result;
	}
	
	private void LoadLevel(byte[,,] level)
	{
		blocks = new Block[level.GetLength(0),level.GetLength(1),level.GetLength(2)];
		for(var z = 0; z < level.GetLength(2); z++)
		for(var y = 0; y < level.GetLength(1); y++)
		for(var x = 0; x < level.GetLength(0); x++)
		{
			var number = level[x,y,z];
			if(number != _)
			{
				CreateBlock(x, y, z, number);
			}
		}	
	}
	
	public GameObject CreateBlock(int x, int y, int z, int number)
	{
		var scale = BlockPrefab.transform.localScale;
		var posX = x * scale.x;
		var posY = y * scale.y;
		var posZ = z * scale.z;
		var posVect = Vector3.Scale(new Vector3(posZ, posY, posX), scale);
		
		var newObject = (GameObject)Instantiate(BlockPrefab, posVect, BlockPrefab.transform.rotation);
		newObject.name = "Block " + number;
		newObject.transform.parent = GameObject.Find("Brain Strain").transform;
			
		Block newBlock = newObject.GetComponent<Block>();
		newBlock.Number = number;
		
		blocks[x,y,z] = newBlock;
		return newObject;
	}
}
