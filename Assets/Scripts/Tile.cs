using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
    {
        [SerializeField] private Color _baseColor, _offsetColor;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private GameObject _highlight;
        [SerializeField] private Color _baseHighlightColor, _offsetHighlightColor;
        public bool _isMoveable = true;
        public GameObject StandingUnit = null;
        public bool _Moveable => _isMoveable && StandingUnit == null;

        public void Init(bool isOffset)
        {
            _renderer.color = isOffset ? _offsetColor : _baseColor;
            //_highlight.GetComponent<SpriteRenderer>().color = isOffset ? _offsetHighlightColor : _baseHighlightColor;
        }

        public void SetUnit(GameObject unit)
        {
            StandingUnit = unit;
        }

        public void RemoveUnit()
        {
            StandingUnit = null;
        }

        public void Moveable()
        {
            _highlight.SetActive(true);
        }

        public void Unmoveable()
        {
            _highlight.SetActive(false);
        }

        void OnMouseEnter()
        {
            //_highlight.SetActive(true);
        }

        void OnMouseExit()
        {
            //_highlight.SetActive(false);
        }
    }
