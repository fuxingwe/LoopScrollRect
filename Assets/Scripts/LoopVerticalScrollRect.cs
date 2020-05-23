using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Loop Vertical Scroll Rect", 51)]
    [DisallowMultipleComponent]
    public class LoopVerticalScrollRect : LoopScrollRect
    {
        protected override float GetSize(RectTransform item)
        {
            float size = contentSpacing;
            if (m_GridLayout != null)
            {
                size += m_GridLayout.cellSize.y;
            }
            else
            {
                size += LayoutUtility.GetPreferredHeight(item);
            }
            return size;
        }

        protected override float GetDimension(Vector2 vector)
        {
            return vector.y;
        }

        protected override Vector2 GetVector(float value)
        {
            return new Vector2(0, value);
        }

        protected override void Awake()
        {
            base.Awake();
            directionSign = -1;

            GridLayoutGroup layout = content.GetComponent<GridLayoutGroup>();
            if (layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedColumnCount)
            {
                Debug.LogError("[LoopHorizontalScrollRect] unsupported GridLayoutGroup constraint");
            }
        }

        protected override bool UpdateItems(Bounds viewBounds, Bounds contentBounds, bool bForward)
        {
            bool changed = false;
            bool bDeleted = false;//删除过的话，不需要判断增加了，删除会判断是否直接把删除的作为另一端增加
            //Debug.Log(bForward+"============" + velocity.ToString());
            //根据运动方向进行处理，可以把删除的移到另一端（DeleteItemAtEnd里面处理），并且避免触发另一端的删除
            if (bForward)
            {
                if (viewBounds.min.y > contentBounds.min.y + threshold)
                {
                    float size = DeleteItemAtEnd(), totalSize = size;
                    while (size > 0 && viewBounds.min.y > contentBounds.min.y + threshold + totalSize)
                    {
                        size = DeleteItemAtEnd();
                        totalSize += size;
                    }
                    if (totalSize > 0)
                    {
                        changed = true;
                        bDeleted = true;
                    }
                }

                if (!bDeleted && viewBounds.max.y > contentBounds.max.y)
                {
                    float size = NewItemAtStart(), totalSize = size;
                    while (size > 0 && viewBounds.max.y > contentBounds.max.y + totalSize)
                    {
                        size = NewItemAtStart();
                        totalSize += size;
                    }
                    if (totalSize > 0)
                        changed = true;
                }
            }
            else
            {
                bDeleted = false;
                if (viewBounds.max.y < contentBounds.max.y - threshold)
                {
                    float size = DeleteItemAtStart(), totalSize = size;
                    while (size > 0 && viewBounds.max.y < contentBounds.max.y - threshold - totalSize)
                    {
                        size = DeleteItemAtStart();
                        totalSize += size;
                    }
                    if (totalSize > 0)
                    {
                        changed = true;
                        bDeleted = true;
                    }
                }

                if (!bDeleted && viewBounds.min.y < contentBounds.min.y)
                {
                    float size = NewItemAtEnd(), totalSize = size;
                    while (size > 0 && viewBounds.min.y < contentBounds.min.y - totalSize)
                    {
                        size = NewItemAtEnd();
                        totalSize += size;
                    }
                    if (totalSize > 0)
                        changed = true;
                }
            }

            return changed;
        }
    }
}