using UnityEngine;

namespace TEC
{
    public class PickupItemInteractor : MonoBehaviour
    {
        [SerializeField] private float pickupRadius = 1.5f;

        // NonAlloc 용 버퍼 (필요시 크기 조절)
        private readonly Collider[] overlapped = new Collider[32];

        public void TryPickupNearestDropItem()
        {
            // Player 위치 기준 반경 내 콜라이더를 NonAlloc으로 탐색
            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                pickupRadius,
                overlapped
            );


            if (count <= 0)
                return;

            DropItem nearest = null;
            float best = float.MaxValue;
            Vector3 origin = transform.position;

            for (int i = 0; i < count; i++)
            {
                var col = overlapped[i];
                if (col == null)
                    continue;

                // Collider가 자식에 있어도 부모의 DropItem을 찾도록
                DropItem dropItem = col.GetComponentInParent<DropItem>();
                if (dropItem == null)
                    continue;

                float d = (dropItem.transform.position - origin).sqrMagnitude;
                if (d < best)
                {
                    best = d;
                    nearest = dropItem;
                }
            }

            if (nearest == null)
                return;

            // 데이터 조작은 UserDataModel이 담당
            if (!UserDataModel.Singleton.AddItem(nearest.ItemID, nearest.Quantity))
                return;

            Destroy(nearest.gameObject);
        }
    }
}
