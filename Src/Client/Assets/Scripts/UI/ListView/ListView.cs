using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ListView : MonoBehaviour
{
    public UnityAction<ListViewItem> onItemSelected;
    public GridLayoutGroup gridLayoutGroup;
    [SerializeField]
    private float startHight;



    public class ListViewItem : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                onSelected(selected);
            }
        }
        public virtual void onSelected(bool selected)
        {
        }

        public ListView owner;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!this.selected)
            {
                this.Selected = true;
            }
            if (owner != null && owner.SelectedItem != this)
            {
                owner.SelectedItem = this;
            }
        }
    }



    public List<ListViewItem> items = new List<ListViewItem>();
    [SerializeField]
    private ListViewItem selectedItem = null;

    private void Start()
    {
        this.startHight = GetComponent<RectTransform>().sizeDelta.y;
    }
    public ListViewItem SelectedItem

    {
        get { return selectedItem; }
        set
        {
            
            if (selectedItem!=null && selectedItem != value)
            {
                selectedItem.Selected = false;
            }
            selectedItem = value;

            if (onItemSelected != null && value != null)
                onItemSelected.Invoke((ListViewItem)value);

        }
    }

    public void AddItem(ListViewItem item)
    {
        item.owner = this;
        this.items.Add(item);
        SetContentSize();
    }

    public void RemoveAll()
    {
        foreach(var it in items)
        {
            Destroy(it.gameObject);
        }
        items.Clear();
    }
    public void SetContentSize()
    {
        if (gridLayoutGroup==null)
        {
            return;
        }
        RectTransform rt = this.gameObject.GetComponent<RectTransform>();
        float wigth = rt.sizeDelta.x;
        float hight = gridLayoutGroup.cellSize.y * items.Count;
        if (hight>startHight)
        {
            rt.sizeDelta = new Vector3(wigth, gridLayoutGroup.cellSize.y * items.Count);
        }
        
    }
}
