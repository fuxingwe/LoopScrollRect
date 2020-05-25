using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Loop Horizontal Scroll Rect", 50)]
    [DisallowMultipleComponent]
    public class LoopHorizontalScrollRect : LoopScrollRect
    {
        protected override float GetSize(RectTransform item)
        {
            float size = contentSpacing;
            if (m_GridLayout != null)
            {
                size += m_GridLayout.cellSize.x;
            }
            else
            {
                size += LayoutUtility.GetPreferredWidth(item);
            }
            return size;
        }

        protected override float GetDimension(Vector2 vector)
        {
            return -vector.x;
        }

        protected override Vector2 GetVector(float value)
        {
            return new Vector2(-value, 0);
        }

        protected override void Awake()
        {
            base.Awake();
            directionSign = 1;

            GridLayoutGroup layout = content.GetComponent<GridLayoutGroup>();
            if (layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedRowCount)
            {
                Debug.LogError("[LoopHorizontalScrollRect] unsupported GridLayoutGroup constraint");
            }
        }

        protected override bool UpdateItems(Bounds viewBounds, Bounds contentBounds, bool bForwardAxis)
        {
            bool changed = false;
            bool bDeleted = false;//ɾ�����Ļ�������Ҫ�ж������ˣ�ɾ�����ж��Ƿ�ֱ�Ӱ�ɾ������Ϊ��һ������
            //Debug.Log(bForwardAxis + "============" + velocity.ToString());
            //�����˶�������д������԰�ɾ�����Ƶ���һ�ˣ�DeleteItemAtEnd���洦�������ұ��ⴥ����һ�˵�ɾ��
            if (bForwardAxis)
            {
                bDeleted = false;
                if (viewBounds.max.x < contentBounds.max.x - threshold)
                {
                    float size = DeleteItemAtEnd(), totalSize = size;
                    while (size > 0 && viewBounds.max.x < contentBounds.max.x - threshold - totalSize)
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

                if (!bDeleted && viewBounds.min.x < contentBounds.min.x)
                {
                    float size = NewItemAtStart(), totalSize = size;
                    while (size > 0 && viewBounds.min.x < contentBounds.min.x - totalSize)
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
                if (viewBounds.min.x > contentBounds.min.x + threshold)
                {
                    float size = DeleteItemAtStart(), totalSize = size;
                    while (size > 0 && viewBounds.min.x > contentBounds.min.x + threshold + totalSize)
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

                if (!bDeleted && viewBounds.max.x > contentBounds.max.x)
                {
                    float size = NewItemAtEnd(), totalSize = size;
                    while (size > 0 && viewBounds.max.x > contentBounds.max.x + totalSize)
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