﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
namespace SSR
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Mask))]
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollSnapRect : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {

        [Tooltip("Set starting page index - starting from 0")]
        public int startingPage = 0;
        [Tooltip("Threshold time for fast swipe in seconds")]
        private float fastSwipeThresholdTime = 0.3f;
        [Tooltip("Threshold time for fast swipe in (unscaled) pixels")]
        private int fastSwipeThresholdDistance = 100;
        [Tooltip("How fast will page lerp to target position")]
        private float decelerationRate = 50f;
        [Tooltip("Button to go to the previous page (optional)")]
        public GameObject prevButton;
        [Tooltip("Button to go to the next page (optional)")]
        public GameObject nextButton;
        [Tooltip("Sprite for unselected page (optional)")]
        public Color unselectedColor;
        [Tooltip("Sprite for selected page (optional)")]
        public Color selectedColor;
        [Tooltip("Container with page images (optional)")]
        public Transform pageSelectionIcons;
        public GameObject paginationParent;
        //page selected event
        public static event System.Action<int> GetCurrentPage;


        // fast swipes should be fast and short. If too long, then it is not fast swipe
        private int _fastSwipeThresholdMaxLimit;
        private bool isCurrentPage = false;
        private ScrollRect _scrollRectComponent;
        private RectTransform _scrollRectRect;
        private RectTransform _container;

        private bool _horizontal;

        // number of pages in container
        private int _pageCount;
        public int _currentPage;

        // whether lerping is in progress and target lerp position
        private bool _lerp;
        private Vector2 _lerpTo;

        // target position of every page
        private List<Vector2> _pagePositions = new List<Vector2>();

        // in draggging, when dragging started and where it started
        private bool _dragging;
        private float _timeStamp;
        private Vector2 _startPosition;

        // for showing small page icons
        private bool _showPageSelection;
        private int _previousPageSelectionIndex;
        // container with Image components - one Image for each page
        private List<Image> _pageSelectionImages;
        private List<ScrollSelector> _pageSelectorList;
        // Automatic banner movement
        private float waitTime = 3f;
        private float currentTime = 0;
        private bool canMove;
        private void Start()
        {
            SetScroll();
        }
        //------------------------------------------------------------------------
     
        private void OnDisable()
        {
            canMove = false;
            currentTime = 0;
        }
        public void SetInfoBanner(Image obj)
        {
            _pageSelectionImages.Add(obj);
        }
        public void SetScroll()
        {
            _scrollRectComponent = GetComponent<ScrollRect>();
            _scrollRectRect = GetComponent<RectTransform>();
            _container = _scrollRectComponent.content;
            //Image[] _pagenation = paginationParent.GetComponentsInChildren<Image>();
            ScrollSelector[] _pagenation = paginationParent.GetComponentsInChildren<ScrollSelector>();
            _pageSelectorList = new List<ScrollSelector>();
            foreach (var item in _pagenation)
            {
                _pageSelectorList.Add(item);
                item.SetPageIndex(_pageSelectorList.IndexOf(item));
                item.SetScrollSnapInstance(this);
            }
            // is it horizontal or vertical scrollrect
            if (_scrollRectComponent.horizontal && !_scrollRectComponent.vertical)
            {
                _horizontal = true;
            }
            else if (!_scrollRectComponent.horizontal && _scrollRectComponent.vertical)
            {
                _horizontal = false;
            }
            else
            {
                Debug.LogWarning("Confusing setting of horizontal/vertical direction. Default set to horizontal.");
                _horizontal = true;
            }

            _lerp = false;

            // init
            SetPagePositions();
            SetPage(startingPage);
            InitPageSelection();
            SetPageSelection(startingPage);

            // prev and next buttons
            if (nextButton)
                nextButton.GetComponent<Button>().onClick.AddListener(() => { NextScreen(); });

            if (prevButton)
                prevButton.GetComponent<Button>().onClick.AddListener(() => { PreviousScreen(); });
            
        }
        private void MoveInfoBanner()
        {
            if (_currentPage == _pageCount - 1)
            {
                PreviousScreen();
            }else if (_currentPage<_pageCount-1)
            {
                NextScreen();
            }
            
        }
        //------------------------------------------------------------------------
        void Update()
        {
           /* if(canMove)
            {
                if (currentTime <= waitTime)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= waitTime)
                    {
                        currentTime = 0;
                        MoveInfoBanner();
                    }
                }
            }*/
            // if moving to target position
            if (_lerp)
            {
                // prevent overshooting with values greater than 1
                float decelerate = Mathf.Min(decelerationRate * Time.deltaTime, 1f);
                _container.anchoredPosition = Vector2.Lerp(_container.anchoredPosition, _lerpTo, decelerate);
                // time to stop lerping?
                if (Vector2.SqrMagnitude(_container.anchoredPosition - _lerpTo) < 0.25f)
                {
                    // snap to target and stop lerping
                    _container.anchoredPosition = _lerpTo;
                    _lerp = false;
                    // clear also any scrollrect move that may interfere with our lerping
                    _scrollRectComponent.velocity = Vector2.zero;
                }

                // switches selection icon exactly to correct page
                if (_showPageSelection)
                {
                    SetPageSelection(GetNearestPage());
                }
            }
        }

        //------------------------------------------------------------------------
        private void SetPagePositions()
        {
            int width = 0;
            int height = 0;
            int offsetX = 0;
            int offsetY = 0;
            int containerWidth = 0;
            int containerHeight = 0;
            _pageCount = _container.childCount;
            if (_horizontal)
            {
                // screen width in pixels of scrollrect window
                width = (int)_scrollRectRect.rect.width;
                // center position of all pages
                offsetX = width / 2;
                // total width
                containerWidth = width * _pageCount;
                // limit fast swipe length - beyond this length it is fast swipe no more
                _fastSwipeThresholdMaxLimit = width;
            }
            else
            {
                height = (int)_scrollRectRect.rect.height;
                offsetY = height / 2;
                containerHeight = height * _pageCount;
                _fastSwipeThresholdMaxLimit = height;
            }

            // set width of container
            Vector2 newSize = new Vector2(containerWidth, containerHeight);
            _container.sizeDelta = newSize;
            Vector2 newPosition = new Vector2(containerWidth / 2, containerHeight / 2);
            _container.anchoredPosition = newPosition;

            // delete any previous settings
            _pagePositions.Clear();

            // iterate through all container childern and set their positions
            for (int i = 0; i < _pageCount; i++)
            {
                RectTransform child = _container.GetChild(i).GetComponent<RectTransform>();
                child.anchorMax = new Vector2(0.5f,0.5f);
                child.anchorMin = new Vector2(0.5f,0.5f);
                child.pivot = new Vector2(0.5f,0.5f);
                child.anchoredPosition = Vector2.zero;
                Vector2 childPosition;
                if (_horizontal)
                {
                    childPosition = new Vector2(i * width - containerWidth / 2 + offsetX, 0f);
                }
                else
                {
                    childPosition = new Vector2(0f, -(i * height - containerHeight / 2 + offsetY));
                }
                child.anchoredPosition = childPosition;
                _pagePositions.Add(-childPosition);
            }
        }

        //------------------------------------------------------------------------
        private void SetPage(int aPageIndex)
        {
            aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
            _container.anchoredPosition = _pagePositions[aPageIndex];
            _currentPage = aPageIndex;
            _pageSelectorList[_currentPage].SetSelector(true);
        }

        //------------------------------------------------------------------------
        public void LerpToPage(int aPageIndex)
        {
            aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
            _lerpTo = _pagePositions[aPageIndex];
            _lerp = true;
            _currentPage = aPageIndex;
            //callback for page changed.
            GetCurrentPage?.Invoke(_currentPage);

            foreach (var item in _pageSelectorList)
            {
                //item.color = unselectedColor;
                item.SetSelector(false);
            }
            //_pageSelectionImages[_currentPage].color = selectedColor;
            _pageSelectorList[_currentPage].SetSelector(true);
        }

        //------------------------------------------------------------------------
        private void InitPageSelection()
        {
            // page selection - only if defined sprites for selection icons
           // _showPageSelection = unselectedPage != null && selectedPage != null;
            if (_showPageSelection)
            {
                // also container with selection images must be defined and must have exatly the same amount of items as pages container
                if (pageSelectionIcons == null || pageSelectionIcons.childCount != _pageCount)
                {
                    Debug.LogWarning("Different count of pages and selection icons - will not show page selection");
                    _showPageSelection = false;
                }
                else
                {
                    _previousPageSelectionIndex = -1;
                }
            }
        }

        //------------------------------------------------------------------------
        private void SetPageSelection(int aPageIndex)
        {
            if (_previousPageSelectionIndex >= 0)
            {
                //_pageSelectionImages[aPageIndex].color = unselectedColor;
                _pageSelectorList[_currentPage].SetSelector(false);
                // _pageSelectionImages[_previousPageSelectionIndex].SetNativeSize();
            }

            // select new
            //_pageSelectionImages[aPageIndex].color = selectedColor;
            _pageSelectorList[_currentPage].SetSelector(true);
            _previousPageSelectionIndex = aPageIndex;
        }

        //------------------------------------------------------------------------
        private void NextScreen()
        {
            LerpToPage(_currentPage + 1);
        }

        //------------------------------------------------------------------------
        private void PreviousScreen()
        {
            LerpToPage(_currentPage - 1);
        }

        //------------------------------------------------------------------------
        private int GetNearestPage()
        {
            // based on distance from current position, find nearest page
            Vector2 currentPosition = _container.anchoredPosition;

            float distance = float.MaxValue;
            int nearestPage = _currentPage;

            for (int i = 0; i < _pagePositions.Count; i++)
            {
                float testDist = Vector2.SqrMagnitude(currentPosition - _pagePositions[i]);
                if (testDist < distance)
                {
                    distance = testDist;
                    nearestPage = i;
                }
            }

            return nearestPage;
        }

        //------------------------------------------------------------------------
        public void OnBeginDrag(PointerEventData aEventData)
        {
            // if currently lerping, then stop it as user is draging
            _lerp = false;
            // not dragging yet
            _dragging = false;
        }

        //------------------------------------------------------------------------
        public void OnEndDrag(PointerEventData aEventData)
        {
            // how much was container's content dragged
            float difference;
            if (_horizontal)
            {
                difference = _startPosition.x - _container.anchoredPosition.x;
            }
            else
            {
                difference = -(_startPosition.y - _container.anchoredPosition.y);
            }

            // test for fast swipe - swipe that moves only +/-1 item
            if (Time.unscaledTime - _timeStamp < fastSwipeThresholdTime &&
                Mathf.Abs(difference) > fastSwipeThresholdDistance &&
                Mathf.Abs(difference) < _fastSwipeThresholdMaxLimit)
            {
                if (difference > 0)
                {
                    NextScreen();
                }
                else
                {
                    PreviousScreen();
                }
            }
            else
            {
                // if not fast time, look to which page we got to
                LerpToPage(GetNearestPage());
            }

            _dragging = false;
        }

        //------------------------------------------------------------------------
        public void OnDrag(PointerEventData aEventData)
        {
            if (!_dragging)
            {
                // dragging started
                _dragging = true;
                // save time - unscaled so pausing with Time.scale should not affect it
                _timeStamp = Time.unscaledTime;
                // save current position of cointainer
                _startPosition = _container.anchoredPosition;
            }
            else
            {
                if (_showPageSelection)
                {
                    SetPageSelection(GetNearestPage());
                }
            }
        }
    }
}