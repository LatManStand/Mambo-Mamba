using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{

    private Vector2 _direction = Vector2.right;

    private List<Transform> _segments = new List<Transform>();
    public Transform segmentPrefab;

    public int initialSize = 3;
    public GameObject effect;
    public GameObject deathFX;

    private enum haciaDondeMiro
    {
        arriba, abajo, izquierda, derecha
    }

    private haciaDondeMiro mirando = haciaDondeMiro.derecha;



    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        /* if (Input.GetKeyDown(KeyCode.W))
         {
             _direction = Vector2.up;
             mirando = haciaDondeMiro.arriba;
         }
         else if (Input.GetKeyDown(KeyCode.S))
         {
             _direction = Vector2.down;
             mirando = haciaDondeMiro.abajo;

         }
         else if (Input.GetKeyDown(KeyCode.D))
         {
             _direction = Vector2.right;
             mirando = haciaDondeMiro.derecha;
         }
         else if (Input.GetKeyDown(KeyCode.A))
         {
             _direction = Vector2.left;
             mirando = haciaDondeMiro.izquierda;
         }
        */
#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0) && Time.timeScale > 0.1f)
        {

            if (Input.mousePosition.x < Screen.width / 2)
            {
                if (mirando == haciaDondeMiro.derecha)
                {
                    _direction = Vector2.up;
                    mirando = haciaDondeMiro.arriba;
                }
                else if (mirando == haciaDondeMiro.arriba)
                {
                    _direction = Vector2.left;
                    mirando = haciaDondeMiro.izquierda;
                }
                else if (mirando == haciaDondeMiro.izquierda)
                {
                    _direction = Vector2.down;
                    mirando = haciaDondeMiro.abajo;
                }
                else if (mirando == haciaDondeMiro.abajo)
                {
                    _direction = Vector2.right;
                    mirando = haciaDondeMiro.derecha;
                }


            }
            else if (Input.mousePosition.x >= Screen.width / 2)
            {
                if (mirando == haciaDondeMiro.derecha)
                {
                    _direction = Vector2.down;
                    mirando = haciaDondeMiro.abajo;
                }
                else if (mirando == haciaDondeMiro.arriba)
                {
                    _direction = Vector2.right;
                    mirando = haciaDondeMiro.derecha;
                }
                else if (mirando == haciaDondeMiro.izquierda)
                {
                    _direction = Vector2.up;
                    mirando = haciaDondeMiro.arriba;
                }
                else if (mirando == haciaDondeMiro.abajo)
                {
                    _direction = Vector2.left;
                    mirando = haciaDondeMiro.izquierda;
                }


            }
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && Time.timeScale > 0.1f)
        {



            if (Input.GetTouch(0).position.x < Screen.width / 2)
            {
                if (mirando == haciaDondeMiro.derecha)
                {
                    _direction = Vector2.up;
                    mirando = haciaDondeMiro.arriba;
                }
                else if (mirando == haciaDondeMiro.arriba)
                {
                    _direction = Vector2.left;
                    mirando = haciaDondeMiro.izquierda;
                }
                else if (mirando == haciaDondeMiro.izquierda)
                {
                    _direction = Vector2.down;
                    mirando = haciaDondeMiro.abajo;
                }
                else if (mirando == haciaDondeMiro.abajo)
                {
                    _direction = Vector2.right;
                    mirando = haciaDondeMiro.derecha;
                }


            }
            else if (Input.GetTouch(0).position.x >= Screen.width / 2)
            {
                if (mirando == haciaDondeMiro.derecha)
                {
                    _direction = Vector2.down;
                    mirando = haciaDondeMiro.abajo;
                }
                else if (mirando == haciaDondeMiro.arriba)
                {
                    _direction = Vector2.right;
                    mirando = haciaDondeMiro.derecha;
                }
                else if (mirando == haciaDondeMiro.izquierda)
                {
                    _direction = Vector2.up;
                    mirando = haciaDondeMiro.arriba;
                }
                else if (mirando == haciaDondeMiro.abajo)
                {
                    _direction = Vector2.left;
                    mirando = haciaDondeMiro.izquierda;
                }


            }
        }
#endif

    }

    private void FixedUpdate()
    {

        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
        }

        this.transform.position = new Vector3(
            Mathf.Round(this.transform.position.x) + _direction.x,
            Mathf.Round(this.transform.position.y) + _direction.y,
            0.0f);

    }

    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;

        _segments.Add(segment);
    }

    private void ResetState()
    {

        for (int i = 1; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }

        _segments.Clear();
        _segments.Add(this.transform);

        for (int i = 1; i < this.initialSize; i++)
        {
            _segments.Add(Instantiate(this.segmentPrefab));
        }

        //this.transform.position = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
        {
            Grow();
            GameManager.instance.GenerateFruit();
            Instantiate(effect, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
        }
        else if (other.tag == "Obstacle" && Time.timeScale > 0.1f)
        {
            GameManager.instance.EndMatch();
            Instantiate(deathFX, transform.position, Quaternion.identity);
            //ResetState();
        }
    }

    public void LookUpwards()
    {
        _direction = Vector2.up;
        mirando = haciaDondeMiro.arriba;
    }

    public void LookRight()
    {
        _direction = Vector2.right;
        mirando = haciaDondeMiro.derecha;
    }



}
