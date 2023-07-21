using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class MenuSwipe : MonoBehaviour
{
	[SerializeField]
	private Scrollbar _scrollbar;

	[SerializeField, Tooltip("Sensitivity multiplier of the scroll"), Range(0.5f, 2.0f)]
	private float _scrollMultiplier;

	private float[] _screenPages;

	private float _distance;

	private int _currentPage = 1;

	private Color _selectedButton = new Color(1, 0.6f, 0.6f);

	private Color _deselectedButton = new Color(1, 1, 1);

	private SwipeTrack _swipe;


	public Button[] _buttons;


	internal int _nextPage = 1;

	void Awake()
	{
		_swipe = new SwipeTrack();
		_swipe.SwipeMovement.Enable();
	}
	void Start()
	{
		_screenPages = new float[transform.childCount];
		_distance = 1f / (_screenPages.Length - 1f);
	}
	void Update()
	{

		SwipeManager();
		ButtonHighlight(_buttons);

	}

	private void ButtonHighlight(Button[] menuButtons)
	{
		for (int i = 0; i < menuButtons.Length; i++)
		{
			if (_scrollbar.value > i / (float)menuButtons.Length && _scrollbar.value <= (i + 1) / (float)menuButtons.Length)
			{
				menuButtons[i].image.color = _selectedButton;
			}
			else
			{
				menuButtons[i].image.color = _deselectedButton;
			}
		}

		// I have to do this manually because Unity is dumb and when the scroll bar is all the way
		// to the right or left it's never 0 or 1, is always 0.000001 or 0.999999.

		if (_scrollbar.value <= 0)
		{
			menuButtons[0].image.color = _selectedButton;
		}
		else if (_scrollbar.value >= 1)
		{
			menuButtons[2].image.color = _selectedButton;
		}
	}

	private void SwipeManager()
	{
		Vector2 swipeDistance = _swipe.SwipeMovement.SwipeTrack.ReadValue<Vector2>();

		for (int i = 0; i < _screenPages.Length; i++)
		{
			_screenPages[i] = i * _distance;
		}

		// Check for the input touches and updates _currentPage.
		if (Input.touches.Length > 0)
		{
			if (Input.touches[0].phase == UnityEngine.TouchPhase.Began)
			{
				if (_currentPage != _nextPage)
				{
					_currentPage = _nextPage;
				}
			}
		}

		// While swiping, updates _scrollbar.value and _nextPage.
		if (_swipe.SwipeMovement.SwipeTrack.IsInProgress())
		{
			for (int i = 0; i < _screenPages.Length; i++)
			{
				_scrollbar.value = Mathf.Lerp(_scrollbar.value, _scrollbar.value - (swipeDistance.x * _scrollMultiplier / Screen.width), 0.1f);
			}

			
		}

		// When the player releases the touch, it snaps the page.
		else if (Input.touchCount == 0)
		{
			// Left swipe
			if (_scrollbar.value < _screenPages[_currentPage] - 0.1f)
			{
				if (_nextPage == _currentPage && _currentPage > 0)
					_nextPage = _currentPage - 1;
			}
			// Right swipe
			else if (_scrollbar.value > _screenPages[_currentPage] + 0.1f)
			{
				if (_nextPage == _currentPage && _currentPage < 2)
				{
					TeamManager.s_TeamManagerInstance.LoadDeck();
					_nextPage = _currentPage + 1;
				}
			}

			for (int i = 0; i < _screenPages.Length; i++)
			{
				if (i == _currentPage)
				{
					_scrollbar.value = Mathf.Lerp(_scrollbar.value, _screenPages[_nextPage], 0.1f);
				}
			}
		}
	}
}
