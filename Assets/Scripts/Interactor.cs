using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _interactionPointRadius = 0.5f;

    [SerializeField] private LayerMask _interactableMask;
    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numFound;

    private void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(gameObject.transform.position, _interactionPointRadius, _colliders,
            _interactableMask);
        if(_numFound >0 && Input.GetKey("e"))
            dointeraction();
    }

    private void dointeraction()
    {
        Debug.Log("Dointeraction");
        switch (_colliders[0].name)
        {
            case "ChessInteract":
                Debug.Log("inot chessinteract");
                SceneManager.LoadScene("ChessScene");
                break;
            
        }
    }
}
