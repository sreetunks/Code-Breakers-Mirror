using TMPro;
using UnityEngine;

namespace Grid
{
    // Do we even need this anymore? we pretty much removed my debug code from GridSystem
    public class GridDebugObject : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMeshPro;
        private IGridObject _gridObject;
    
        public void SetGridObject(IGridObject gridObject)
        {
            _gridObject = gridObject;
        }

        private void Update()
        {
            textMeshPro.text = _gridObject.ToString();
        }
    }
}
