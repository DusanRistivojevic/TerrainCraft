using UnityEngine;
using System.Collections;
using TerrainCraft;
namespace TerrainCraft
{
    public class CameraCtrl : MonoBehaviour
    {
        [SerializeField]
        private TerrainCraft terrain;
        [SerializeField]
        private float moveSpeed = 10;

        private Vector2 moveBoundsX;
        private Vector2 moveBoundsY;

        // Use this for initialization
        void Start()
        {
            moveBoundsX = new Vector2(terrain.transform.position.x + 12, terrain.transform.position.x + terrain.Width - 12);
            moveBoundsY = new Vector2(terrain.transform.position.z - 6, terrain.transform.position.z + terrain.Height - 25);
            pos = new Vector3(moveBoundsX.x, transform.position.y, moveBoundsY.x);
        }

        private Vector3 pos;
        // Update is called once per frame
        void Update()
        {
            Movement();
        }

        void Movement()
        {
            pos += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * moveSpeed;
            pos.x = Mathf.Clamp(pos.x, moveBoundsX.x, moveBoundsX.y);
            pos.z = Mathf.Clamp(pos.z, moveBoundsY.x, moveBoundsY.y);

            if (Vector3.Distance(transform.position, pos) > Time.deltaTime)
            {
                transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 10);

            }
        }

        public void MoveByClickingOnMinimap(float x, float y)
        {
            pos = new Vector3(Mathf.Lerp(moveBoundsX.x, moveBoundsX.y, x), 15, Mathf.Lerp(moveBoundsY.x, moveBoundsY.y, y));
            if (pos.x < moveBoundsX.x)
                pos = new Vector3(moveBoundsX.x, pos.y, transform.position.z);
            if (pos.x > moveBoundsX.y)
                pos = new Vector3(moveBoundsX.y, pos.y, transform.position.z);
            if (pos.z < moveBoundsY.x)
                pos = new Vector3(transform.position.x, pos.y, moveBoundsY.x);
            if (pos.z > moveBoundsY.y)
                pos = new Vector3(transform.position.x, pos.y, moveBoundsY.y);
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 10);
        }
    }
}
