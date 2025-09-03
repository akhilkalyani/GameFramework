using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// Entry for the recycling system. Extends Unity's inbuilt ScrollRect.
/// </summary>
public class RecyclableScrollSnap : ScrollRect   // ✅ Make sure this extends ScrollRect
{
    [HideInInspector]
    public IRecyclableScrollRectDataSource DataSource;

    public bool IsGrid;
    public bool IsInfinite;
    public RectTransform PrototypeCell;
    public bool SelfInitialize = true;

    public enum DirectionType { Vertical, Horizontal }
    public DirectionType Direction;

    [SerializeField] private int _segments;
    public int Segments
    {
        set { _segments = Math.Max(value, 2); }
        get { return _segments; }
    }
    public int CurrentPageIndex { get;  set; } = 0;
    private RecyclingSystem _recyclingSystem;
    private Vector2 _prevAnchoredPos;
    private int previousIndex = 0;
    protected override void Start()
    {
        vertical = true;
        horizontal = false;

        if (!Application.isPlaying) return;
        if (SelfInitialize) Initialize();
    }

    private void Initialize()
    {
        //Destroy old cells
        var childrens = content.GetComponentsInChildren<ICell>();
        foreach (var cell in childrens)
        {
            Destroy(cell.GetGameObject());
        }

        // Construct recycling system
        if (Direction == DirectionType.Vertical)
        {
            _recyclingSystem = new VerticalRecyclingSystem(PrototypeCell, viewport, content, DataSource, IsGrid, Segments);
        }
        else if (Direction == DirectionType.Horizontal)
        {
            _recyclingSystem = new HorizontalRecyclingSystem(PrototypeCell, viewport, content, DataSource, IsGrid, Segments);
        }

        _recyclingSystem.IsInfinite = IsInfinite;
        vertical = Direction == DirectionType.Vertical;
        horizontal = Direction == DirectionType.Horizontal;

        _prevAnchoredPos = content.anchoredPosition;
        onValueChanged.RemoveListener(OnValueChangedListener);

        // Add recycling listener after pool init
        StartCoroutine(_recyclingSystem.InitCoroutine(() =>
            onValueChanged.AddListener(OnValueChangedListener)
        ));
    }

    public void Initialize(IRecyclableScrollRectDataSource dataSource)
    {
        DataSource = dataSource;
        Initialize();
    }

    public void OnValueChangedListener(Vector2 normalizedPos)
    {
        Vector2 dir = content.anchoredPosition - _prevAnchoredPos;
        m_ContentStartPosition += _recyclingSystem.OnValueChangedListener(dir);
        _prevAnchoredPos = content.anchoredPosition;
    }

    public void ReloadData()
    {
        ReloadData(DataSource);
    }

    public void ReloadData(IRecyclableScrollRectDataSource dataSource)
    {
        if (_recyclingSystem != null)
        {
            StopMovement();
            onValueChanged.RemoveListener(OnValueChangedListener);
            _recyclingSystem.DataSource = dataSource;
            StartCoroutine(_recyclingSystem.InitCoroutine(() =>
                onValueChanged.AddListener(OnValueChangedListener)
            ));
            _prevAnchoredPos = content.anchoredPosition;
        }
    }

    // ===========================
    // 🚀 NEW SNAPPING CODE BELOW
    // ===========================

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        SnapToNearestCell();
    }

    private void SnapToNearestCell()
    {
        if (content.childCount == 0) return;

        RectTransform cell = content.GetChild(0) as RectTransform;
        float cellWidth = cell.sizeDelta.x;
        float cellHeight = cell.sizeDelta.y;

        Vector2 targetPos = content.anchoredPosition;
        int index = 0;

        if (vertical)
        {
            float offset = -content.anchoredPosition.y;
            index = Mathf.RoundToInt(offset / cellHeight);
            index = Mathf.Clamp(index, 0, DataSource.GetItemCount() - 1);
            targetPos.y = -index * cellHeight;
        }
        else if (horizontal)
        {
            float offset = -content.anchoredPosition.x;
            index = Mathf.RoundToInt(offset / cellWidth);
            index = Mathf.Clamp(index, 0, DataSource.GetItemCount() - 1);
            targetPos.x = -index * cellWidth;
        }

        // ✅ Store page index now
        if (index > previousIndex)
        {
            CurrentPageIndex++;
        }
        else if (index < previousIndex)
        {
            CurrentPageIndex--;
        }
        previousIndex = index;
        StopMovement();
        onValueChanged.RemoveListener(OnValueChangedListener);
        StartCoroutine(SnapCoroutine(content.anchoredPosition, targetPos, 0.25f));
    }

    private IEnumerator SnapCoroutine(Vector2 startPos, Vector2 endPos, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            content.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        content.anchoredPosition = endPos;

        // // ✅ Notify listeners when snapping is done
        // OnPageSnapped?.Invoke(CurrentPageIndex);
        DataSource.PageChanged(CurrentPageIndex);

        // Resume recycling
        onValueChanged.AddListener(OnValueChangedListener);
    }
}

