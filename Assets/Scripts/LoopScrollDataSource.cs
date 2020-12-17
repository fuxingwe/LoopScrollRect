using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    public interface IScrollCell
    {
        void UpdateNormalizedPos(float normalizedPos);
    }
    
    public abstract class LoopScrollDataSource
    {
        public abstract void ProvideData(Transform transform, int idx, bool bCircled);
        
        public void UpdateCellNormalizedPos(Transform transform, float normalizedPos)
        {
            IScrollCell cell = transform.GetComponent<IScrollCell>();
            if (cell != null)
            {
                cell.UpdateNormalizedPos(normalizedPos);
            }
        }
    }

	public class LoopScrollSendIndexSource : LoopScrollDataSource
    {
		public static readonly LoopScrollSendIndexSource Instance = new LoopScrollSendIndexSource();

		LoopScrollSendIndexSource(){}

        public override void ProvideData(Transform transform, int idx ,bool bCircled)
        {
            transform.SendMessage("ScrollCellIndex", idx);
        }
    }

	public class LoopScrollArraySource<T> : LoopScrollDataSource
    {
        T[] objectsToFill;

		public LoopScrollArraySource(T[] objectsToFill)
        {
            this.objectsToFill = objectsToFill;
        }

        public override void ProvideData(Transform transform, int idx ,bool bCircled)
        {
            if (bCircled && objectsToFill != null && objectsToFill.Length > 0 && idx > objectsToFill.Length)
                idx = idx % objectsToFill.Length;
            transform.SendMessage("ScrollCellContent", objectsToFill[idx]);
        }
    }
}