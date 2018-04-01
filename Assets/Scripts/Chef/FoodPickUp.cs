﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FoodPickUp : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{
	public static GameObject itemBeingDragged;
	public GameObject[] dragDestinations;
	public ChefCoolDown chef;
	private bool isDragging = false;
	Vector3 startPosition;

	#region IBeginDragHandler implementation
	public void OnBeginDrag (PointerEventData eventData)
	{
		FoodCoolDown foodManager = GetComponent<FoodCoolDown> ();
		if (!foodManager.isInCoolDown) {
			itemBeingDragged = gameObject;
			startPosition = transform.position;
			isDragging = true;
		}
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
					coolDown.emptyFood ();
					break;
				case "Tra":
					//////THROW FOOD AWAY HERE//////
					coolDown.binFood();
					break;
				case "Che":
					//////COOK FOOD HERE//////
					if (!chef.isInUse) {
						coolDown.cookFood();
						chef.CookThis (gameObject);
					}
					break;
				}
			}
			i++;
		}
	}

	bool WithinRange(GameObject gameObject){
		RectTransform rectTransform = gameObject.GetComponent<RectTransform> ();
		bool withinXRange = Mathf.Abs(transform.localPosition.x - rectTransform.localPosition.x) < (rectTransform.sizeDelta.x / 2);
		bool withinYRange = Mathf.Abs(transform.localPosition.y - rectTransform.localPosition.y) < (rectTransform.sizeDelta.y / 2);
		return withinXRange && withinYRange;
	}
}