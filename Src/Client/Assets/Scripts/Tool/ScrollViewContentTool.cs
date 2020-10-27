/*
 * 说明：挂在UGUI中ScrollView中的Content游戏物体下(在Hierarchy面板中右键创建UI->ScrollView,在子物体中找到Content)
 * 
 * 功能：解决ScrollView中Content不能根据实际Content下的游戏物体的多少自动改变Content的宽高问题
 *      以至于在Content动态添加需要排序的游戏物体时ScrollBar滑条变更不正确的问题
 *      (Content Size Fitter组件是用于文本组件时自动根据文本变更大小的组件,这里不适用)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewContentTool : MonoBehaviour
{
    /// <summary>
    /// 根据ScrollBar的类型自动调整Content的宽或高
    /// </summary>
    public enum ScrollBarType
    {
        Vertical,      //为垂直状态时需设置RectTransform的Anchors为Min(0,1),Max(1,1)
        Horizontal,    //为水平向右延伸状态时需设置RectTransform的Anchors为Min(0,0),Max(0,1)
        HorizontalAndVertical
    }

    public ScrollBarType m_barType;

    /// <summary>
    /// 该Content表示实际宽高(这里宽高大小应该为Viewport遮罩层的大小)
    /// </summary>
    public float m_ContentWidth = 0;
    public float m_ContentHeight = 1000;

    /// <summary>
    /// Content下排列的游戏物体的宽和高的大小
    /// </summary>
    public Vector2 m_cellSize = Vector2.zero;

    /// <summary>
    /// Content下排列的游戏物体X轴和Y轴的间距
    /// </summary>
    public Vector2 m_Spacing = Vector2.zero;

    private RectTransform m_rectTransform = null;
    private int m_tempChildCount = 0;

    // Use this for initialization
    private void Awake()
    {
        m_rectTransform = this.GetComponent<RectTransform>();
        m_tempChildCount = this.transform.childCount;
        //if (m_tempChildCount > 0) m_ChildSize = this.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;

        if (m_barType == ScrollBarType.Horizontal)
        {
            if (m_ContentWidth == 0) Debug.LogError("请设置Content的Width!!");
            m_rectTransform.anchorMin = new Vector2(0, 0);
            m_rectTransform.anchorMax = new Vector2(0, 1);
            m_rectTransform.sizeDelta = new Vector2(m_ContentWidth, 0);
        }
        else if (m_barType == ScrollBarType.Vertical)
        {
            if (m_ContentHeight == 0) Debug.LogError("请设置Content的Height!!");
            m_rectTransform.anchorMin = new Vector2(0, 1);
            m_rectTransform.anchorMax = new Vector2(1, 1);
            m_rectTransform.sizeDelta = new Vector2(0, m_ContentHeight);
        }
        else if (m_barType == ScrollBarType.HorizontalAndVertical)
        {
            if (m_ContentHeight == 0 || m_ContentWidth == 0) Debug.LogError("请设置Content的Width和Height!!");
            m_rectTransform.anchorMin = new Vector2(0, 0);
            m_rectTransform.anchorMax = new Vector2(1, 1);
            m_rectTransform.sizeDelta = new Vector2(0, 0);
        }

        //Debug.Log(this.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta);
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_tempChildCount != this.transform.childCount)
        {
            Debug.Log(this.transform.parent.parent.parent.name + this.transform.childCount);
            m_tempChildCount = this.transform.childCount;
            UpdateContentSize(m_tempChildCount);
        }
    }

    /// <summary>
    /// 根据Content下子物体数量的变化更新Content的宽高
    /// </summary>
    private void UpdateContentSize(int _count)
    {
        if (m_barType == ScrollBarType.Horizontal)
        {
            if (_count * (m_cellSize.x + m_Spacing.x) > m_ContentWidth)
            {
                m_rectTransform.sizeDelta = new Vector2(_count * (m_cellSize.x + m_Spacing.x), 0);
                //Debug.Log(m_rectTransform.sizeDelta.x);
            }
        }
        else if (m_barType == ScrollBarType.Vertical)
        {
            if (_count * (m_cellSize.y + m_Spacing.y) > m_ContentHeight)
            {
                m_rectTransform.sizeDelta = new Vector2(0, _count * (m_cellSize.y + m_Spacing.y));
                //Debug.Log(m_rectTransform.sizeDelta.y);
            }
        }
        //此时的m_rectTransform.sizeDelta代表往右和往下的增量，为0时代表Content的初始大小
        else if (m_barType == ScrollBarType.HorizontalAndVertical)
        {
            if (_count * (m_cellSize.x + m_Spacing.x) > m_ContentWidth)
            {
                float width = Mathf.Abs(m_ContentWidth - _count * (m_cellSize.x + m_Spacing.x));
                m_rectTransform.sizeDelta = new Vector2(width, 0);
            }

            if (_count * (m_cellSize.y + m_Spacing.y) > m_ContentHeight)
            {
                float height = Mathf.Abs(m_ContentHeight - _count * (m_cellSize.y + m_Spacing.y));
                m_rectTransform.sizeDelta = new Vector2(0, -height);


                
            }
        }
    }
}

