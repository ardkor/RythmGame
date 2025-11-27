
    using System;
    using UnityEngine;

    public class FallingBlock : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lifeTime = 10f;
        public bool IsUsed = false;
        private void Update()
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
                Destroy(gameObject);
            transform.Translate(Vector3.down * Time.deltaTime * speed);
        }
    }
