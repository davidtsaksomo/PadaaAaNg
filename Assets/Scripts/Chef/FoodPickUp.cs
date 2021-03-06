﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FoodPickUp : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{
	public static GameObject itemBeingDragged;
	ChefNetworkManager networkManager;
	GameObject[] dragDestinations;
	public ChefCoolDown chef;
	private bool isDragging = false;
	Vector3 startPosition;
	private AudioSource sounds;
	public AudioClip droppingPlate;

	void Start(){
		dragDestinations = GameObject.FindGameObjectsWithTag ("DragDestination");
		chef = GameObject.Find ("IsiUlang").GetComponent<ChefCoolDown>();
		networkManager = GameObject.FindGameObjectWithTag ("ChefNetworkManager").GetComponent<ChefNetworkManager>();
		sounds = GetComponent<AudioSource> ();
	}
	#region IBeginDragHandler implementation
	public void OnBeginDrag (PointerEventData eventData)
	{
		FoodCoolDown foodManager = GetComponent<FoodCoolDown> ();
		itemBeingDragged = gameObject;
		startPosition = transform.position;
		isDragging = true;
	}
	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		if (isDragging) {
			transform.position = Input.mousePosition;

		}
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		if (isDragging) {
			ActivateDrag ();
			itemBeingDragged = null;
			transform.position = startPosition;
			isDragging = false;
		}
	}

	#endregion

	void ActivateDrag(){
		bool found = false;
		int i = 0;
		while (i < dragDestinations.Length && !found) {
			if (WithinRange (dragDestinations [i])) {
				found = true;
				FoodCoolDown coolDown = GetComponent<FoodCoolDown> ();
				switch (dragDestinations [i].name.Substring (0, 3)) {
				case "Win":
					//////FOOD DELIVERY HERE//////
					if (coolDown.isInCoolDown) {
						Debug.Log ("Gabisa ngirim makanan kosong");

					} else {
						coolDown.emptyFood ();
						Debug.Log ("Put On Window");
						int foodID = GetComponent<FoodID> ().id;
						int playerID = dragDestinations [i].GetComponent<Window> ().id;
						networkManager.SendFood (foodID, playerID);
					}

					break;
				case "Tra":
					//////THROW FOOD AWAY HERE//////
					if (coolDown.isInCoolDown) {
						Debug.Log ("Gabisa buang makanan kosong");
					} else {
						coolDown.binFood();
						Debug.Log ("Put On Trash");

					}

					break;
				case "Isi":
					//////COOK FOOD HERE//////
					if (!chef.isInUse && coolDown.isInCoolDown) {
						coolDown.cookFood();
						Debug.Log ("Put On IsiUlang");
						chef.CookThis (gameObject);
					}
					break;
				}
			}
			i++;
		}
		if (!found) {
			sounds.clip = droppingPlate;
			sounds.Play ();
		}
	}

	bool WithinRange(GameObject gameObject){
		RectTransform rectTransform = gameObject.GetComponent<RectTransform> ();
		if (gameObject.activeInHierarchy == false) {
			return false;
		}
		bool withinXRange = Mathf.Abs(GetComponent<RectTransform>().localPosition.x - rectTransform.localPosition.x) < (rectTransform.sizeDelta.x / 2);
		bool withinYRange = Mathf.Abs(GetComponent<RectTransform>().localPosition.y - rectTransform.localPosition.y) < (rectTransform.sizeDelta.y / 2);

		return withinXRange && withinYRange;
	}
}
